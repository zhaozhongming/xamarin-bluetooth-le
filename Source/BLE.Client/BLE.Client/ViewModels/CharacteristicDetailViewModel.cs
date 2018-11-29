using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acr.UserDialogs;
using MvvmCross.Core.ViewModels;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Extensions;
using BLE.Client.Helpers;

namespace BLE.Client.ViewModels
{
    public class CharacteristicDetailViewModel : BaseViewModel
    {
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
        //const string wid = "00031234-0000-1000-8000-00805F9B0131";
        //const string wid = "49535343-8841-43F4-A8D4-ECBE34729BB3";//taobao bluetooth HC-02
        const string wid = "0000ffe1-0000-1000-8000-00805f9b34fb";//taobao bluetooth HC-42

        static readonly Guid wguid = new Guid(wid);

        private readonly IUserDialogs _userDialogs;
        private bool _updatesStarted;
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

        public ObservableCollection<string> Messages { get; } = new ObservableCollection<string>();

        public string UpdateButtonText => _updatesStarted ? "Stop updates" : "Start updates";

        public string ConsoleText { get; private set; }

        public string Permissions
        {
            get
            {
                if (Characteristic == null)
                    return string.Empty;

                return (Characteristic.CanRead ? "Read " : "") +
                       (Characteristic.CanWrite ? "Write " : "") +
                       (Characteristic.CanUpdate ? "Update" : "");
            }
        }

        public CharacteristicDetailViewModel(IAdapter adapter, IUserDialogs userDialogs) : base(adapter)
        {
            _userDialogs = userDialogs;
        }

        protected override async void InitFromBundle(IMvxBundle parameters)
        {
            base.InitFromBundle(parameters);

            Characteristic = await GetCharacteristicFromBundleAsync(parameters);

            _service = await GetServiceFromBundleAsync(parameters);
        }

        public override void Resume()
        {
            base.Resume();

            if (Characteristic != null)
            {
                return;
            }

            Close(this);
        }
        public override void Suspend()
        {
            base.Suspend();

            if (Characteristic != null)
            {
                StopUpdates();
            }
            
        }

        public MvxCommand ReadCommand => new MvxCommand(ReadValueAsync);

        private async void ReadValueAsync()
        {
            if (Characteristic == null)
                return;

            try
            {
                _userDialogs.ShowLoading("Reading characteristic value...");

                await Characteristic.ReadAsync();

                RaisePropertyChanged(() => CharacteristicValue);

                Messages.Insert(0, $"Read value {CharacteristicValue}");
            }
            catch (Exception ex)
            {
                _userDialogs.HideLoading();
                _userDialogs.ShowError(ex.Message);

                Messages.Insert(0, $"Error {ex.Message}");

            }
            finally
            {
                _userDialogs.HideLoading();
            }

        }

        public MvxCommand WriteCommand => new MvxCommand(WriteValueAsync);

        private async void WriteValueAsync()
        {
            try
            {
                var result =
                    await
                        _userDialogs.PromptAsync("Input a value (as hex whitespace separated)", "Write value",
                            placeholder: CharacteristicValue);

                if (!result.Ok)
                    return;

                var cmd = GetBytes(result.Text);
               
                var data = createHeader(TXMODE, PUT_TRANSACTION_RESULT);//GetBytes(result.Text);

                data = cmd;//for s8


                _userDialogs.ShowLoading("Write characteristic value");

                Characteristics = await _service.GetCharacteristicsAsync();

                CharacteristicWrite = Characteristics.Where(x => x.Id == wguid).FirstOrDefault();

                await CharacteristicWrite.WriteAsync(data);
                _userDialogs.HideLoading();

                RaisePropertyChanged(() => CharacteristicValue);
                Messages.Insert(0, $"Wrote value {CharacteristicValue}");
            }
            catch (Exception ex)
            {
                _userDialogs.HideLoading();
                _userDialogs.ShowError(ex.Message);
            }

        }

        private static byte[] GetBytes(string text)
        {
            return text.Split(' ').Where(token => !string.IsNullOrEmpty(token)).Select(token => Convert.ToByte(token, 16)).ToArray();
            //return System.Text.Encoding.UTF8.GetBytes(text);
            //return System.Text.Encoding.Unicode.GetBytes(text);
        }

        public MvxCommand ToggleUpdatesCommand => new MvxCommand((() =>
        {
            if (_updatesStarted)
            {
                StopUpdates();
            }
            else
            {
                StartUpdates();
            }
        }));

        private async void StartUpdates()
        {
            try
            {
                _updatesStarted = true;

                Characteristic.ValueUpdated -= CharacteristicOnValueUpdated;
                Characteristic.ValueUpdated += CharacteristicOnValueUpdated;
                await Characteristic.StartUpdatesAsync();
         

                Messages.Insert(0, $"Start updates");

                RaisePropertyChanged(() => UpdateButtonText);

            }
            catch (Exception ex)
            {
                _userDialogs.ShowError(ex.Message);
            }
        }

        private async void StopUpdates()
        {
            try
            {
                _updatesStarted = false;

                await Characteristic.StopUpdatesAsync();
                Characteristic.ValueUpdated -= CharacteristicOnValueUpdated;

                Messages.Insert(0, $"Stop updates");

                RaisePropertyChanged(() => UpdateButtonText);

            }
            catch (Exception ex)
            {
                _userDialogs.ShowError(ex.Message);
            }
        }

        private void CharacteristicOnValueUpdated(object sender, CharacteristicUpdatedEventArgs characteristicUpdatedEventArgs)
        {
            string dataString = new string(Encoding.UTF8.GetChars(Characteristic?.Value));

            string dataString2 = new string(Encoding.Unicode.GetChars(Characteristic?.Value));

            string dataString3 = new string(Encoding.BigEndianUnicode.GetChars(Characteristic?.Value));

            dataString = dataString.Replace("\r", "\r\n");// + dataString3.Replace("\r", "\r\n");
            ConsoleText += dataString;

            RaisePropertyChanged(() => ConsoleText);
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
    }
}