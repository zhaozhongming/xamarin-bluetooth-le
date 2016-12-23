using AppKit;
using Foundation;
using MvvmCross.Mac.Views.Presenters;
using MvvmCross.Mac.Platform;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;

namespace BLE.Client.Mac
{
    [Register("AppDelegate")]
    public class AppDelegate : MvxApplicationDelegate
    {
        public AppDelegate()
        {
        }
        public override void DidFinishLaunching(NSNotification notification)
        {
            var presenter = new MvxMacViewPresenter(this, NSApplication.SharedApplication.KeyWindow);
            var setup = new Setup(this, presenter);
            setup.Initialize();

            var startup = Mvx.Resolve<IMvxAppStart>();
            startup.Start();
            //mainWindowController.Window.MakeKeyAndOrderFront(this);
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }
    }
}
