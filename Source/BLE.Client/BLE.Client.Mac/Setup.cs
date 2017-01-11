using System;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using MvvmCross.Core.ViewModels;
using MvvmCross.Mac.Platform;
using MvvmCross.Mac.Views.Presenters;
using MvvmCross.Platform;
using Plugin.BLE;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace BLE.Client.Mac
{
    public class Setup : MvxMacSetup
    {
       public Setup(MvxApplicationDelegate applicationDelegate, IMvxMacViewPresenter presenter)
            : base(applicationDelegate, presenter)
        {
        }

        protected override IMvxApplication CreateApp()
        {
            return new BleMvxApplication();
        }

        protected override void InitializeIoC()
        {
            base.InitializeIoC();

            Mvx.RegisterSingleton(() => CrossBluetoothLE.Current);
            Mvx.RegisterSingleton(() => CrossBluetoothLE.Current.Adapter);

            Mvx.RegisterSingleton<IUserDialogs>(new UserDialogsImpl());
            Mvx.RegisterSingleton<ISettings>(new SettingsMock());
        }

        public class SettingsMock : ISettings
        {
            public bool AddOrUpdateValue<T>(string key, T value)
            {
                return true;
                //throw new NotImplementedException();
            }

            public T GetValueOrDefault<T>(string key, T defaultValue = default(T))
            {
                return defaultValue;
                //throw new NotImplementedException();
            }

            public void Remove(string key)
            {
                //throw new NotImplementedException();
            }
        }


    }
}
