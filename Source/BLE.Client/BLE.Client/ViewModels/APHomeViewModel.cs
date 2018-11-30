using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    class APHomeViewModel : BaseViewModel
    {
        public string ConsoleText { get; private set; }

        private readonly IBluetoothLE _bluetoothLe;
        private readonly IUserDialogs _userDialogs;
        private readonly ISettings _settings;
        private Guid _previousGuid;
        private CancellationTokenSource _cancellationTokenSource;
        public ObservableCollection<DeviceListItemViewModel> Devices { get; set; } = new ObservableCollection<DeviceListItemViewModel>();
        public bool IsRefreshing => Adapter.IsScanning;
        public bool IsStateOn => _bluetoothLe.IsOn;

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

        public string StateText => GetStateText();

        public MvxCommand StopScanCommand => new MvxCommand(() =>
        {
            _cancellationTokenSource.Cancel();
            CleanupCancellationToken();
            RaisePropertyChanged(() => IsRefreshing);
        }, () => _cancellationTokenSource != null);

        readonly IPermissions _permissions;

        public MvxCommand RefreshCommand => new MvxCommand(() => TryStartScanning(true));



        public APHomeViewModel(IBluetoothLE bluetoothLe, IAdapter adapter, IUserDialogs userDialogs, ISettings settings, IPermissions permissions) : base(adapter)
        {
            _permissions = permissions;
            _bluetoothLe = bluetoothLe;
            _userDialogs = userDialogs;
            _settings = settings;

            // quick and dirty :>
            _bluetoothLe.StateChanged += OnStateChanged;
            Adapter.DeviceDiscovered += OnDeviceDiscovered;
            Adapter.ScanTimeoutElapsed += Adapter_ScanTimeoutElapsed;
            Adapter.DeviceDisconnected += OnDeviceDisconnected;
            Adapter.DeviceConnectionLost += OnDeviceConnectionLost;

            ConsoleText = "Connecting Bluetooth device...";

            RefreshCommand.Execute();
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

        private void OnDeviceDiscovered(object sender, DeviceEventArgs args)
        {
            if (args.Device != null 
                    && args.Device.Name != null 
                    && (args.Device.Name.Contains("HC") || args.Device.Name.Contains("BLE")))
            {
                ConsoleText += "found device " + args.Device.Name;
                ConsoleText += "stop scaning...";

                RaisePropertyChanged(() => ConsoleText);

                StopScanCommand.Execute();
            }
        }

        private void OnDeviceDisconnected(object sender, DeviceEventArgs e)
        {
            Devices.FirstOrDefault(d => d.Id == e.Device.Id)?.Update();
            _userDialogs.HideLoading();
            _userDialogs.Toast($"Disconnected {e.Device.Name}");
        }

        private void Adapter_ScanTimeoutElapsed(object sender, EventArgs e)
        {
            RaisePropertyChanged(() => IsRefreshing);

            CleanupCancellationToken();
        }

        private async void ScanForDevices()
        {
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

                ConsoleText += connectedDevice.Name + "\r\n";
            }

            _cancellationTokenSource = new CancellationTokenSource();
            RaisePropertyChanged(() => StopScanCommand);

            RaisePropertyChanged(() => IsRefreshing);
            Adapter.ScanMode = ScanMode.LowLatency;
            await Adapter.StartScanningForDevicesAsync(_cancellationTokenSource.Token);
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

        private void CleanupCancellationToken()
        {
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
            RaisePropertyChanged(() => StopScanCommand);
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
    }
}
