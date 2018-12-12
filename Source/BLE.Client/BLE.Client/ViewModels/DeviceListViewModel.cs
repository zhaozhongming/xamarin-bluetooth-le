using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BLE.Client.Extensions;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Extensions;
using Plugin.Permissions.Abstractions;
using Plugin.Settings.Abstractions;

namespace BLE.Client.ViewModels
{
    public class DeviceListViewModel : BaseViewModel
    {
        private readonly IBluetoothLE _bluetoothLe;
        private readonly IUserDialogs _userDialogs;
        private readonly ISettings _settings;
        private Guid _previousGuid;
        private CancellationTokenSource _cancellationTokenSource;

        public Guid PreviousGuid
        {
            get { return _previousGuid; }
            set
            {
                _previousGuid = value;
                _settings.AddOrUpdateValue("lastguid", _previousGuid.ToString());
                RaisePropertyChanged();
                RaisePropertyChanged(() => ConnectToPreviousCommand);
            }
        }

        private Guid knownId = new Guid("00000000-0000-0000-0000-cc81d47f7569 ");

        public MvxCommand RefreshCommand => new MvxCommand(() => TryStartScanning(true));
        public MvxCommand<DeviceListItemViewModel> DisconnectCommand => new MvxCommand<DeviceListItemViewModel>(DisconnectDevice);

        public MvxCommand DisconnectKnownCommandSliently => new MvxCommand(DisconnectKnownDeviceSliently);
        
        public MvxCommand ConnectKnownCommand => new MvxCommand(GetReadings);
        
        public MvxCommand<DeviceListItemViewModel> ConnectDisposeCommand => new MvxCommand<DeviceListItemViewModel>(ConnectAndDisposeDevice);
        
        public ObservableCollection<DeviceListItemViewModel> Devices { get; set; } = new ObservableCollection<DeviceListItemViewModel>();
        public bool IsRefreshing => Adapter.IsScanning;
        public bool IsStateOn => _bluetoothLe.IsOn;
        public string StateText => GetStateText();
        public DeviceListItemViewModel SelectedDevice
        {
            get { return null; }
            set
            {
                if (value != null)
                {
                    HandleSelectedDevice(value);
                }

                RaisePropertyChanged();
            }
        }

        #region 3000
        protected byte address = 0x00;

        public static readonly byte RXMODE = 0x70;
        public static readonly byte TXMODE = 0x68;

        public static readonly byte NUL = 0x00;
        public static readonly byte STX = 0x02;
        public static readonly byte ETX = 0x03;
        public static readonly byte EOT = 0x04;
        public static readonly byte ENQ = 0x05;
        public static readonly byte ACK = 0x06;
        public static readonly byte CR = 0x0D;
        public static readonly byte DC1 = 0x11;
        public static readonly byte DC2 = 0x12;
        public static readonly byte NAK = 0x15;
        public static readonly byte ESC = 0x1B;


        public static readonly byte PUT_RELEASE = 0x8d;
        public static readonly byte PUT_DATA = 0x85;

        public static readonly byte PUT_TRANSACTION_RESULT = 0x88;

        #endregion

        bool _useAutoConnect;
        public bool UseAutoConnect
        {
            get
            {
                return _useAutoConnect;
            }

            set
            {
                if (_useAutoConnect == value)
                    return;
                
                _useAutoConnect = value;
                RaisePropertyChanged();
            }
        }

        public string ConsoleText { get; private set; }

        private IDevice knownDevice { get; set; }

        public IList<IService> Services { get; private set; }

        const string serviceidHC = "0000ffe0-0000-1000-8000-00805f9b34fb";//taobeo HC-42
        static readonly Guid serviceguidHC = new Guid(serviceidHC);
        const string cidHC = "0000ffe1-0000-1000-8000-00805f9b34fb";//taobao bluetooth HC-42
        static readonly Guid wguid = new Guid(cidHC);

        const string serviceidBLE = "0000ffe0-0000-1000-8000-00805f9b34fb";//taobeo HC-42
        static readonly Guid serviceguidBLE = new Guid(serviceidBLE);
        const string cidBLE = "00031234-0000-1000-8000-00805F9B0130";//ble
        static readonly Guid cguidBLE = new Guid(cidBLE);
        const string widBLE = "00031234-0000-1000-8000-00805F9B0131";//ble
        static readonly Guid wguidBLE = new Guid(widBLE);

        public float Reading { get; set; }
        private IService _service;

        public ICharacteristic Characteristic { get; private set; }

        private IList<ICharacteristic> _characteristics;

        public IList<ICharacteristic> Characteristics
        {
            get { return _characteristics; }
            private set { SetProperty(ref _characteristics, value); }
        }
        public ICharacteristic CharacteristicWrite { get; private set; }

        public string CharacteristicValue => Characteristic?.Value.ToHexString().Replace("-", " ");//new string(Encoding.UTF8.GetChars(Characteristic?.Value));//

        public MvxCommand StopScanCommand => new MvxCommand(() =>
        {
            ConsoleOutput("stop scanning.... ");
            _cancellationTokenSource.Cancel();
            CleanupCancellationToken();
            RaisePropertyChanged(() => IsRefreshing);
        }, () => _cancellationTokenSource != null);

        readonly IPermissions _permissions;

        public DeviceListViewModel(IBluetoothLE bluetoothLe, IAdapter adapter, IUserDialogs userDialogs, ISettings settings, IPermissions permissions) : base(adapter)
        {
            _permissions = permissions;
            _bluetoothLe = bluetoothLe;
            _userDialogs = userDialogs;
            _settings = settings;
            // quick and dirty :>
            _bluetoothLe.StateChanged -= OnStateChanged;
            Adapter.DeviceDiscovered -= OnDeviceDiscovered;
            Adapter.ScanTimeoutElapsed -= Adapter_ScanTimeoutElapsed;
            Adapter.DeviceDisconnected -= OnDeviceDisconnected;
            Adapter.DeviceConnectionLost -= OnDeviceConnectionLost;

            _bluetoothLe.StateChanged += OnStateChanged;
            Adapter.DeviceDiscovered += OnDeviceDiscovered;
            Adapter.ScanTimeoutElapsed += Adapter_ScanTimeoutElapsed;
            Adapter.DeviceDisconnected += OnDeviceDisconnected;
            Adapter.DeviceConnectionLost += OnDeviceConnectionLost;
            //Adapter.DeviceConnected += (sender, e) => Adapter.DisconnectDeviceAsync(e.Device);

        }

        public override void ViewDisappearing()
        {
            DisconnectKnownDeviceSliently();
        }

        protected override void InitFromBundle(IMvxBundle parameters)
        {
            base.InitFromBundle(parameters);

            SelectedModelType = GetSelectedModelTypeFromBundle(parameters);
        }

        protected ModelType GetSelectedModelTypeFromBundle(IMvxBundle parameters)
        {
            if (!parameters.Data.ContainsKey(ModelTypeKey)) return ModelType.Flowcom3000;
            var modelType = parameters.Data[ModelTypeKey];

            if (modelType.Equals(ModelType.Flowcom3000.ToString()))
                return ModelType.Flowcom3000;
            else
                return ModelType.FlowcomS8;
        }

        private Task GetPreviousGuidAsync()
        {
            return Task.Run(() =>
            {
                var guidString = _settings.GetValueOrDefault("lastguid", string.Empty);
                PreviousGuid = !string.IsNullOrEmpty(guidString) ? Guid.Parse(guidString) : Guid.Empty;
            });
        }

        private void OnDeviceConnectionLost(object sender, DeviceErrorEventArgs e)
        {
            Devices.FirstOrDefault(d => d.Id == e.Device.Id)?.Update();

            _userDialogs.HideLoading();
            _userDialogs.ErrorToast("Error", $"Connection LOST {e.Device.Name}", TimeSpan.FromMilliseconds(6000));
        }

        private void OnStateChanged(object sender, BluetoothStateChangedArgs e)
        {
            RaisePropertyChanged(nameof(IsStateOn));
            RaisePropertyChanged(nameof(StateText));
            //TryStartScanning();
        }

        private string GetStateText()
        {
            switch (_bluetoothLe.State)
            {
                case BluetoothState.Unknown:
                    return "Unknown BLE state.";
                case BluetoothState.Unavailable:
                    return "BLE is not available on this device.";
                case BluetoothState.Unauthorized:
                    return "You are not allowed to use BLE.";
                case BluetoothState.TurningOn:
                    return "BLE is warming up, please wait.";
                case BluetoothState.On:
                    return "BLE is on.";
                case BluetoothState.TurningOff:
                    return "BLE is turning off. That's sad!";
                case BluetoothState.Off:
                    return "BLE is off. Turn it on!";
                default:
                    return "Unknown BLE state.";
            }
        }

        private void Adapter_ScanTimeoutElapsed(object sender, EventArgs e)
        {
            RaisePropertyChanged(() => IsRefreshing);

            CleanupCancellationToken();
        }

        private void OnDeviceDiscovered(object sender, DeviceEventArgs args)
        {
            if (args.Device != null && args.Device.Name != null
                && (args.Device.Name.Contains("HC") || args.Device.Name.Contains("BLE")))
            {
                StopScanCommand.Execute();
                AddOrUpdateDevice(args.Device);
            }
        }

     
        public override async void Resume()
        {
            base.Resume();

            await GetPreviousGuidAsync();
            //TryStartScanning();

            GetSystemConnectedOrPairedDevices();

        }

        private void GetSystemConnectedOrPairedDevices()
        {
            try
            {
                //heart rate
                var guid = Guid.Parse("0000180d-0000-1000-8000-00805f9b34fb");

                // SystemDevices = Adapter.GetSystemConnectedOrPairedDevices(new[] { guid }).Select(d => new DeviceListItemViewModel(d)).ToList();
                // remove the GUID filter for test
                // Avoid to loose already IDevice with a connection, otherwise you can't close it
                // Keep the reference of already known devices and drop all not in returned list.
                var pairedOrConnectedDeviceWithNullGatt = Adapter.GetSystemConnectedOrPairedDevices();
                SystemDevices.RemoveAll(sd => !pairedOrConnectedDeviceWithNullGatt.Any(p => p.Id == sd.Id));
                SystemDevices.AddRange(pairedOrConnectedDeviceWithNullGatt.Where(d => !SystemDevices.Any(sd => sd.Id == d.Id)).Select(d => new DeviceListItemViewModel(d)));
                RaisePropertyChanged(() => SystemDevices);
            }
            catch (Exception ex)
            {
                Trace.Message("Failed to retreive system connected devices. {0}", ex.Message);
            }
        }

        public List<DeviceListItemViewModel> SystemDevices { get; private set; } = new List<DeviceListItemViewModel>();

        public override void Suspend()
        {
            base.Suspend();

            Adapter.StopScanningForDevicesAsync();
            RaisePropertyChanged(() => IsRefreshing);
        }

        private async void TryStartScanning(bool refresh = false)
        {
            if (Xamarin.Forms.Device.OS == Xamarin.Forms.TargetPlatform.Android)
            {
                var status = await _permissions.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    var permissionResult = await _permissions.RequestPermissionsAsync(Permission.Location);

                    if (permissionResult.First().Value != PermissionStatus.Granted)
                    {
                        _userDialogs.ShowError("Permission denied. Not scanning.");
                        return;
                    }
                }
            }

            if (IsStateOn && (refresh || !Devices.Any()) && !IsRefreshing)
            {
                ScanForDevices();
            }
        }

        private async void ScanForDevices()
        {
            ConsoleOutput("start scanning devices.....");

            Devices.Clear();

            foreach (var connectedDevice in Adapter.ConnectedDevices)
            {
                //update rssi for already connected evices (so tha 0 is not shown in the list)
                try
                {
                    await connectedDevice.UpdateRssiAsync();
                }
                catch (Exception ex)
                {
                    Mvx.Trace(ex.Message);
                    _userDialogs.ShowError($"Failed to update RSSI for {connectedDevice.Name}");
                }

                AddOrUpdateDevice(connectedDevice);
            }

            _cancellationTokenSource = new CancellationTokenSource();
            RaisePropertyChanged(() => StopScanCommand);

            RaisePropertyChanged(() => IsRefreshing);
            Adapter.ScanMode = ScanMode.LowLatency;
            await Adapter.StartScanningForDevicesAsync(_cancellationTokenSource.Token);
        }

        private void CleanupCancellationToken()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
            RaisePropertyChanged(() => StopScanCommand);
        }

        private async void DisconnectDevice(DeviceListItemViewModel device)
        {
            try
            {
                if (!device.IsConnected)
                    return;

                _userDialogs.ShowLoading($"Disconnecting {device.Name}...");

                await Adapter.DisconnectDeviceAsync(device.Device);
            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message, "Disconnect error");
            }
            finally
            {
                device.Update();
                _userDialogs.HideLoading();
            }
        }

        private async void DisconnectKnownDeviceSliently()
        {
            try
            { 
                foreach (DeviceListItemViewModel cDevice in Devices)
                {
                    try
                    {
                        if (!cDevice.IsConnected)
                            return;
                        ConsoleOutput("Disconnect divice - " + cDevice.Name);
                        await Adapter.DisconnectDeviceAsync(cDevice.Device);

                    }
                    catch (Exception ex)
                    {
                        ConsoleOutput("Disconnect divice error: " + ex.Message);
                    }
                    finally
                    {
                        cDevice.Update();
                    }
                }

                if (knownDevice != null && (knownDevice.State.Equals(DeviceState.Connected) || knownDevice.State.Equals(DeviceState.Limited)))
                {
                    await Adapter.DisconnectDeviceAsync(knownDevice);

                    ConsoleOutput("Disconnect divice - " + knownDevice.Name);
                }

                foreach (var connectedDevice in Adapter.ConnectedDevices)
                {
                    await Adapter.DisconnectDeviceAsync(connectedDevice);
                }
            }
            catch (Exception ex)
            {
                ConsoleOutput("Disconnect divice error: " + ex.Message);
            }
        }

        private void HandleSelectedDevice(DeviceListItemViewModel device)
        {
            var config = new ActionSheetConfig();

            if (device.IsConnected)
            {
                config.Add("Update RSSI", async () =>
                {
                    try
                    {
                        _userDialogs.ShowLoading();

                        await device.Device.UpdateRssiAsync();
                        device.RaisePropertyChanged(nameof(device.Rssi));

                        _userDialogs.HideLoading();

                        _userDialogs.ShowSuccess($"RSSI updated {device.Rssi}", 1000);
                    }
                    catch (Exception ex)
                    {
                        _userDialogs.HideLoading();
                        _userDialogs.ShowError($"Failed to update rssi. Exception: {ex.Message}");
                    }
                });

                config.Destructive = new ActionSheetOption("Disconnect", () => DisconnectCommand.Execute(device));
            }
            else
            {
                config.Add("Connect", async () =>
                {
                    if (await ConnectDeviceAsync(device))
                    {
                        //ShowViewModel<ServiceListViewModel>(new MvxBundle(new Dictionary<string, string> { { DeviceIdKey, device.Device.Id.ToString() } }));
                    }
                });

                config.Add("Connect & Dispose", () => ConnectDisposeCommand.Execute(device));
            }

            config.Add("Copy GUID", () => CopyGuidCommand.Execute(device));
            config.Cancel = new ActionSheetOption("Cancel");
            config.SetTitle("Device Options");
            _userDialogs.ActionSheet(config);
        }

        private async Task<bool> ConnectDeviceAsync(DeviceListItemViewModel device, bool showPrompt = true)
        {
            if (showPrompt && !await _userDialogs.ConfirmAsync($"Connect to device '{device.Name}'?"))
            {
                return false;
            }
            try
            {
                CancellationTokenSource tokenSource = new CancellationTokenSource();

                var config = new ProgressDialogConfig()
                {
                    Title = $"Connecting to '{device.Id}'",
                    CancelText = "Cancel",
                    IsDeterministic = false,
                    OnCancel = tokenSource.Cancel
                };

                using (var progress = _userDialogs.Progress(config))
                {
                    progress.Show();

                    await Adapter.ConnectToDeviceAsync(device.Device, new ConnectParameters(autoConnect: UseAutoConnect, forceBleTransport: false), tokenSource.Token);
                }

                _userDialogs.ShowSuccess($"Connected to {device.Device.Name}.");

                ConsoleOutput("Connected to " + device.Device.Name);

                PreviousGuid = device.Device.Id;

                //knownDevice = device.Device;

                return true;

            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message, "Connection error");
                Mvx.Trace(ex.Message);
                return false;
            }
            finally
            {
                _userDialogs.HideLoading();
                device.Update();
            }
        }

        private async Task<bool> ConnectDeviceAsyncSliently(IDevice device)
        {
            try
            {
                CancellationTokenSource tokenSource = new CancellationTokenSource();

                await Adapter.ConnectToDeviceAsync(device, new ConnectParameters(autoConnect: UseAutoConnect, forceBleTransport: false), tokenSource.Token);

                ConsoleOutput(device.Name + " device connected.");
                return true;

            }
            catch (Exception ex)
            {
                Mvx.Trace(ex.Message);
                return false;
            }
        }

        private async void GetReadings()
        {
            bool alreadyConnected = false;

            if (Devices != null && Devices.Count > 0)
            {
                knownId = Devices.OrderBy(i => i.Rssi).Last().Id;

                IEnumerable<DeviceListItemViewModel> di = Devices.Where(r => r.IsConnected);
                if (di != null && di.Count() > 0)
                {
                    DeviceListItemViewModel alreadyConnectedDevice = di.First();
                    if (alreadyConnectedDevice != null)
                    {
                        alreadyConnected = true;
                        knownId = alreadyConnectedDevice.Id;
                        knownDevice = alreadyConnectedDevice.Device;
                    }
                }
            }
            
            if (!alreadyConnected)
            {
                CancellationTokenSource tokenSource = new CancellationTokenSource();

                knownDevice = await Adapter.ConnectToKnownDeviceAsync(knownId, new ConnectParameters(autoConnect: UseAutoConnect, forceBleTransport: false), tokenSource.Token);

                ConsoleOutput(knownDevice.Name + " device connected.");
            }

            if (await LoadServices(knownDevice))
            {
                if (knownDevice.Name.Contains("HC"))
                {
                    _service = await knownDevice.GetServiceAsync(serviceguidHC);

                    Characteristic = await _service.GetCharacteristicAsync(wguid);//notify

                    CharacteristicWrite = await _service.GetCharacteristicAsync(wguid);//write
                }

                if (knownDevice.Name.Contains("BLE"))
                {
                    _service = await knownDevice.GetServiceAsync(serviceguidBLE);

                    Characteristic = await _service.GetCharacteristicAsync(cguidBLE);//notify

                    CharacteristicWrite = await _service.GetCharacteristicAsync(wguidBLE);//write
                }

                ConsoleOutput("get characteristics successfully.");

                Characteristic.ValueUpdated -= CharacteristicOnValueUpdated;
                Characteristic.ValueUpdated += CharacteristicOnValueUpdated;


                await Characteristic.StartUpdatesAsync();

                ConsoleOutput("start getting notification from notify characteristics.");

                SendCommand();
            }
        }

        public MvxCommand ConnectToPreviousCommand => new MvxCommand(ConnectToPreviousDeviceAsync, CanConnectToPrevious);

        private async void ConnectToPreviousDeviceAsync()
        {
            //00000000-0000-0000-0000-cc81d47f7569 

            IDevice device;
            try
            {
                CancellationTokenSource tokenSource = new CancellationTokenSource();

                var config = new ProgressDialogConfig()
                {
                    Title = $"Searching for '{PreviousGuid}'",
                    CancelText = "Cancel",
                    IsDeterministic = false,
                    OnCancel = tokenSource.Cancel
                };

                using (var progress = _userDialogs.Progress(config))
                {
                    progress.Show();

                    device = await Adapter.ConnectToKnownDeviceAsync(PreviousGuid, new ConnectParameters(autoConnect: UseAutoConnect, forceBleTransport: false), tokenSource.Token);

                }

                _userDialogs.ShowSuccess($"Connected to {device.Name}.");

                var deviceItem = Devices.FirstOrDefault(d => d.Device.Id == device.Id);
                if (deviceItem == null)
                {
                    deviceItem = new DeviceListItemViewModel(device);
                    Devices.Add(deviceItem);
                }
                else
                {
                    deviceItem.Update(device);
                }
            }
            catch (Exception ex)
            {
                _userDialogs.ShowError(ex.Message, 5000);
                return;
            }
        }

        private bool CanConnectToPrevious()
        {
            return PreviousGuid != default(Guid);
        }

        private async void ConnectAndDisposeDevice(DeviceListItemViewModel item)
        {
            try
            {
                using (item.Device)
                {
                    _userDialogs.ShowLoading($"Connecting to {item.Name} ...");
                    await Adapter.ConnectToDeviceAsync(item.Device);

                    // TODO make this configurable
                    var resultMTU = await item.Device.RequestMtuAsync(60);
                    System.Diagnostics.Debug.WriteLine($"Requested MTU. Result is {resultMTU}");

                    // TODO make this configurable
                    var resultInterval = item.Device.UpdateConnectionInterval(ConnectionInterval.High);
                    System.Diagnostics.Debug.WriteLine($"Set Connection Interval. Result is {resultInterval}");

                    item.Update();
                    _userDialogs.ShowSuccess($"Connected {item.Device.Name}");

                    _userDialogs.HideLoading();
                    for (var i = 5; i >= 1; i--)
                    {
                        _userDialogs.ShowLoading($"Disconnect in {i}s...");

                        await Task.Delay(1000);

                        _userDialogs.HideLoading();
                    }
                }
            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message, "Failed to connect and dispose.");
            }
            finally
            {
                _userDialogs.HideLoading();
            }


        }

        private void OnDeviceDisconnected(object sender, DeviceEventArgs e)
        {
            Devices.FirstOrDefault(d => d.Id == e.Device.Id)?.Update();
            //_userDialogs.HideLoading();
            //_userDialogs.Toast($"Disconnected {e.Device.Name}");
        }

        public MvxCommand<DeviceListItemViewModel> CopyGuidCommand => new MvxCommand<DeviceListItemViewModel>(device =>
        {
            PreviousGuid = device.Id;
        });

        private void ConsoleOutput(string msg)
        {
            ConsoleText += msg + "\r\n";
            RaisePropertyChanged(() => ConsoleText);
        }


        private async Task<bool> LoadServices(IDevice device)
        {
            try
            {
                if (Services == null || Services.Count == 0)
                    Services = await device.GetServicesAsync();
                ConsoleOutput("services are loaded.");
                return true;
            }
            catch (Exception ex)
            {
                ConsoleOutput("services failed to load.");                
                Mvx.Trace(ex.Message);
                return false;
            }
        }

        private void CharacteristicOnValueUpdated(object sender, CharacteristicUpdatedEventArgs characteristicUpdatedEventArgs)
        {
            string dataString = new string(Encoding.UTF8.GetChars(Characteristic?.Value));
            dataString = "DATA: " + dataString.Replace("\r", "\r\n");// + dataString3.Replace("\r", "\r\n");
            ConsoleOutput(dataString);
        }

        private byte[] createHeader(byte mode, byte opcode)
        {
            byte[] header = new byte[4];
            header[0] = EOT;
            header[1] = (byte)(mode | this.address);
            header[2] = opcode;
            header[3] = ENQ;
            return header;
        }


        private static byte[] GetBytesForUTF8(string text)
        {
            //return text.Split(' ').Where(token => !string.IsNullOrEmpty(token)).Select(token => Convert.ToByte(token, 16)).ToArray();
            return System.Text.Encoding.UTF8.GetBytes(text);
            //return System.Text.Encoding.Unicode.GetBytes(text);
        }

        private static byte[] GetBytesForHEX(string text)
        {
            return text.Split(' ').Where(token => !string.IsNullOrEmpty(token)).Select(token => Convert.ToByte(token, 16)).ToArray();
            //return System.Text.Encoding.UTF8.GetBytes(text);
            //return System.Text.Encoding.Unicode.GetBytes(text);
        }

        private async void SendCommand()
        {
            //sending command
            byte[] cmd = new byte[] { };

            switch (SelectedModelType)
            {
                case ModelType.Flowcom3000:
                    cmd = createHeader(TXMODE, PUT_TRANSACTION_RESULT);//GetBytes(result.Text);

                    await CharacteristicWrite.WriteAsync(cmd);
                    break;
                case ModelType.FlowcomS8:
                    cmd = GetBytesForUTF8("L");

                    await CharacteristicWrite.WriteAsync(cmd);

                    break;
            }

            ConsoleOutput("command sent");
        }
        private async void AddOrUpdateDevice(IDevice device)
        {
            if (device != null && device.Name != null
                && (device.Name.Contains("HC") || device.Name.Contains("BLE")))
            {
                InvokeOnMainThread(() =>
                {
                    var vm = Devices.FirstOrDefault(d => d.Device.Id == device.Id);
                    if (vm != null)
                    {
                        vm.Update();
                    }
                    else
                    {
                        Devices.Add(new DeviceListItemViewModel(device));
                        ConsoleOutput("find devices " + device.Name);
                    }
                });
            }
        }

    }
}