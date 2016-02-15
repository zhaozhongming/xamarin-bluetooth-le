using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Controls;
using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;
using MvvmCross.Forms.Presenter.Core;
using MvvmCross.Forms.Presenter.WindowsUWP;
using MvvmCross.Platform;
using MvvmCross.Platform.Platform;
using MvvmCross.WindowsUWP.Platform;
using MvvmCross.WindowsUWP.Views;

namespace BLE.Client.UWP
{
    public class Setup : MvxWindowsSetup
    {
        private readonly IActivatedEventArgs _args;

        public Setup(Frame rootFrame, IActivatedEventArgs args) : base(rootFrame)
        {
            _args = args;
        }

        protected override IMvxApplication CreateApp()
        {
            return new Client.App();
        }

        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }

        protected override IMvxWindowsViewPresenter CreateViewPresenter(IMvxWindowsFrame rootFrame)
        {
            Xamarin.Forms.Forms.Init(_args);

            var xamarinFormsApp = new MvxFormsApp();
            var presenter = new MvxFormsWindowsUWPPagePresenter(rootFrame, xamarinFormsApp);
            Mvx.RegisterSingleton<IMvxViewPresenter>(presenter);

            return presenter;
        }
    }
}