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

namespace BLE.Client.ViewModels
{
    public class CharacteristicDetailViewModel : BaseViewModel
    {
        const string wid = "00031234-0000-1000-8000-00805F9B0131";
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

                var data = GetBytes(result.Text);

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
            //return text.Split(' ').Where(token => !string.IsNullOrEmpty(token)).Select(token => Convert.ToByte(token, 16)).ToArray();
            return System.Text.Encoding.UTF8.GetBytes(text);
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
            dataString = dataString.Replace("\r", "\r\n");
            ConsoleText += dataString;

            RaisePropertyChanged(() => ConsoleText);
        }
    }
}