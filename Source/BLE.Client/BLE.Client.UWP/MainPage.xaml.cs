using MvvmCross.Core.Views;
using MvvmCross.Forms.Presenter.WindowsUWP;
using MvvmCross.Platform;
using Xamarin.Forms.Platform.UWP;

namespace BLE.Client.UWP
{
    public sealed partial class MainPage : WindowsPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            var start = Mvx.Resolve<MvvmCross.Core.ViewModels.IMvxAppStart>();
            start.Start();

            var presenter = (MvxFormsWindowsUWPPagePresenter) Mvx.Resolve<IMvxViewPresenter>();
            LoadApplication(presenter.MvxFormsApp);
        }
    }
}
