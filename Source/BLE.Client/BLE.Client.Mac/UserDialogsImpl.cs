using System;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using AppKit;

namespace BLE.Client.Mac
{
    public class UserDialogsImpl : IUserDialogs
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
            AlertAsync(config);
            return null;
        }

        public IDisposable Alert(string message, string title = null, string okText = null)
        {
            AlertAsync(message, title, okText);
            return null; 
        }



        public Task AlertAsync(AlertConfig config, CancellationToken? cancelToken = default(CancellationToken?))
        {
            var alert = new NSAlert()
            {
                AlertStyle = NSAlertStyle.Informational,
                InformativeText = config.Message ?? string.Empty,
                MessageText = config.Title ?? string.Empty,


            };
            alert.AddButton(config.OkText ?? "OK");
            alert.RunModal();
            return Task.FromResult(true);
        }

      

        public Task AlertAsync(string message, string title = null, string okText = null, CancellationToken? cancelToken = default(CancellationToken?))
        {
            return AlertAsync(new AlertConfig() { Message = message, Title = title, OkText = okText }, cancelToken);
        }

        public IDisposable Confirm(ConfirmConfig config)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ConfirmAsync(ConfirmConfig config, CancellationToken? cancelToken = default(CancellationToken?))
        {
            var alert = new NSAlert()
            {
                AlertStyle = NSAlertStyle.Informational,
                InformativeText = config.Message ?? string.Empty,
                MessageText = config.Title ?? string.Empty,

            };
            alert.AddButton(config.OkText ?? "OK");
            alert.AddButton(config.CancelText ?? "Cancel");
            var result = alert.RunModal();
            return Task.FromResult(result == 1000);
        }

        public Task<bool> ConfirmAsync(string message, string title = null, string okText = null, string cancelText = null, CancellationToken? cancelToken = default(CancellationToken?))
        {
            return ConfirmAsync(new ConfirmConfig() { Message = message, Title = title, OkText = okText, CancelText = cancelText }, cancelToken);
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
            return new ProgressDialog(config);
        }

        private class ProgressDialog : NSAlert, IProgressDialog
        {
            public ProgressDialog(ProgressDialogConfig config)
            {
                Title = config.Title;
                MessageText = config.Title;

                if (!string.IsNullOrEmpty(config.CancelText))
                {
                    AddButton(config.CancelText);
                }
            }

            public bool IsShowing
            {
                get
                {
                    return true;
                }
            }

            public int PercentComplete
            {
                get; set;
            }

            public string Title { get; set; }
          
            public void Hide()
            {
                //throw new NotImplementedException();
            }

            public void Show()
            {
                //BeginSheetAsync(NSApplication.SharedApplication.KeyWindow);
            }

            public new void Dispose()
            {
                base.Dispose();
            }

          
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
            var alert = new NSAlert()
            {
                MessageText = message
                    
            };

            alert.BeginSheetAsync(NSApplication.SharedApplication.KeyWindow);
        }

        public void ShowImage(global::Splat.IBitmap image, string message, int timeoutMillis = 2000)
        {
            throw new NotImplementedException();
        }

        public void ShowLoading(string title = null, MaskType? maskType = default(MaskType?))
        {
            
        }

        public void ShowSuccess(string message, int timeoutMillis = 2000)
        {
            var alert = new NSAlert()
            {
                MessageText = message

            };

            alert.BeginSheetAsync(NSApplication.SharedApplication.KeyWindow);
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
            var alert = new NSAlert()
            {
                MessageText = cfg.Message

            };

            alert.BeginSheetAsync(NSApplication.SharedApplication.KeyWindow);
            return null;
        }

        public IDisposable Toast(string title, TimeSpan? dismissTimer = default(TimeSpan?))
        {
            var alert = new NSAlert()
            {
                MessageText = title

            };

            alert.BeginSheetAsync(NSApplication.SharedApplication.KeyWindow);
            return null;
        }

       
    }
}
