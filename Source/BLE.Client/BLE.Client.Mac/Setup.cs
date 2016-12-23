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

            Mvx.RegisterSingleton<IUserDialogs>(new UserDialogsMock());
            Mvx.RegisterSingleton<ISettings>(new SettingsMock());
        }

        public class SettingsMock : ISettings
        {
            public bool AddOrUpdateValue<T>(string key, T value)
            {
                throw new NotImplementedException();
            }

            public T GetValueOrDefault<T>(string key, T defaultValue = default(T))
            {
                throw new NotImplementedException();
            }

            public void Remove(string key)
            {
                throw new NotImplementedException();
            }
        }

        public class UserDialogsMock : IUserDialogs
        {
            public IDisposable ActionSheet(ActionSheetConfig config)
            {
                throw new NotImplementedException();
            }

            public Task<string> ActionSheetAsync(string title, string cancel, string destructive, CancellationToken? cancelToken = default(CancellationToken?), params string[] buttons)
            {
                throw new NotImplementedException();
            }

            public IDisposable Alert(AlertConfig config)
            {
                throw new NotImplementedException();
            }

            public IDisposable Alert(string message, string title = null, string okText = null)
            {
                throw new NotImplementedException();
            }

            public Task AlertAsync(AlertConfig config, CancellationToken? cancelToken = default(CancellationToken?))
            {
                throw new NotImplementedException();
            }

            public Task AlertAsync(string message, string title = null, string okText = null, CancellationToken? cancelToken = default(CancellationToken?))
            {
                throw new NotImplementedException();
            }

            public IDisposable Confirm(ConfirmConfig config)
            {
                throw new NotImplementedException();
            }

            public Task<bool> ConfirmAsync(ConfirmConfig config, CancellationToken? cancelToken = default(CancellationToken?))
            {
                throw new NotImplementedException();
            }

            public Task<bool> ConfirmAsync(string message, string title = null, string okText = null, string cancelText = null, CancellationToken? cancelToken = default(CancellationToken?))
            {
                throw new NotImplementedException();
            }

            public IDisposable DatePrompt(DatePromptConfig config)
            {
                throw new NotImplementedException();
            }

            public Task<DatePromptResult> DatePromptAsync(DatePromptConfig config, CancellationToken? cancelToken = default(CancellationToken?))
            {
                throw new NotImplementedException();
            }

            public Task<DatePromptResult> DatePromptAsync(string title = null, DateTime? selectedDate = default(DateTime?), CancellationToken? cancelToken = default(CancellationToken?))
            {
                throw new NotImplementedException();
            }

            public void HideLoading()
            {
                throw new NotImplementedException();
            }

            public IProgressDialog Loading(string title = null, Action onCancel = null, string cancelText = null, bool show = true, MaskType? maskType = default(MaskType?))
            {
                throw new NotImplementedException();
            }

            public IDisposable Login(LoginConfig config)
            {
                throw new NotImplementedException();
            }

            public Task<LoginResult> LoginAsync(LoginConfig config, CancellationToken? cancelToken = default(CancellationToken?))
            {
                throw new NotImplementedException();
            }

            public Task<LoginResult> LoginAsync(string title = null, string message = null, CancellationToken? cancelToken = default(CancellationToken?))
            {
                throw new NotImplementedException();
            }

            public IProgressDialog Progress(ProgressDialogConfig config)
            {
                throw new NotImplementedException();
            }

            public IProgressDialog Progress(string title = null, Action onCancel = null, string cancelText = null, bool show = true, MaskType? maskType = default(MaskType?))
            {
                throw new NotImplementedException();
            }

            public IDisposable Prompt(PromptConfig config)
            {
                throw new NotImplementedException();
            }

            public Task<PromptResult> PromptAsync(PromptConfig config, CancellationToken? cancelToken = default(CancellationToken?))
            {
                throw new NotImplementedException();
            }

            public Task<PromptResult> PromptAsync(string message, string title = null, string okText = null, string cancelText = null, string placeholder = "", InputType inputType = InputType.Default, CancellationToken? cancelToken = default(CancellationToken?))
            {
                throw new NotImplementedException();
            }

            public void ShowError(string message, int timeoutMillis = 2000)
            {
                throw new NotImplementedException();
            }

            public void ShowImage(global::Splat.IBitmap image, string message, int timeoutMillis = 2000)
            {
                throw new NotImplementedException();
            }

            public void ShowLoading(string title = null, MaskType? maskType = default(MaskType?))
            {
                throw new NotImplementedException();
            }

            public void ShowSuccess(string message, int timeoutMillis = 2000)
            {
                throw new NotImplementedException();
            }

            public IDisposable TimePrompt(TimePromptConfig config)
            {
                throw new NotImplementedException();
            }

            public Task<TimePromptResult> TimePromptAsync(TimePromptConfig config, CancellationToken? cancelToken = default(CancellationToken?))
            {
                throw new NotImplementedException();
            }

            public Task<TimePromptResult> TimePromptAsync(string title = null, TimeSpan? selectedTime = default(TimeSpan?), CancellationToken? cancelToken = default(CancellationToken?))
            {
                throw new NotImplementedException();
            }

            public IDisposable Toast(ToastConfig cfg)
            {
                throw new NotImplementedException();
            }

            public IDisposable Toast(string title, TimeSpan? dismissTimer = default(TimeSpan?))
            {
                throw new NotImplementedException();
            }
        }
    }
}
