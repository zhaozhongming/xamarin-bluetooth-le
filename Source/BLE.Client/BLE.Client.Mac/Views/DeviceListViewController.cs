using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using MvvmCross.Mac.Views;
using BLE.Client.ViewModels;
using System.Threading.Tasks;

namespace BLE.Client.Mac.Views
{
    public partial class DeviceListViewController : MvxViewController
    {
        #region Constructors

        // Called when created from unmanaged code
        public DeviceListViewController(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public DeviceListViewController(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public DeviceListViewController() : base("DeviceListView", NSBundle.MainBundle)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        #endregion

        //strongly typed view accessor
        public new DeviceListView View
        {
            get
            {
                return (DeviceListView)base.View;
            }
        }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();

            var vm = ViewModel as DeviceListViewModel;

            vm?.RefreshCommand.Execute();

            while (true)
            {
                await Task.Delay(1000);
                vm?.RefreshCommand.Execute();
                await Task.Delay(5000);
            }



        }
    }
}
