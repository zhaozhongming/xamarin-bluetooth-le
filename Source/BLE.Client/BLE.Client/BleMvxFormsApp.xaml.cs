using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using MvvmCross.Forms.Core;
using MvvmCross.Platform;

namespace BLE.Client
{
    public partial class BleMvxFormsApp : MvxFormsApplication
    {
        public BleMvxFormsApp()
        {
            InitializeComponent();
        }

        protected override void OnStart()
        {
            AppCenter.Start("android=99d4ceb38634964fb48412f4d99fd84f",
               typeof(Analytics),
               typeof(Crashes));

            base.OnStart();
            Mvx.Trace("App Start");
        }

        protected override void OnResume()
        {
            base.OnResume();
            Mvx.Trace("App Resume");
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            Mvx.Trace("App Sleep");
        }
    }
}
