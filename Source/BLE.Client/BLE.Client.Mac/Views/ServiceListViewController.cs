using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using MvvmCross.Mac.Views;
using MvvmCross.Binding.BindingContext;
using BLE.Client.ViewModels;

namespace BLE.Client.Mac.Views
{
    public partial class ServiceListViewController : MvxViewController
    {
        #region Constructors

        // Called when created from unmanaged code
        public ServiceListViewController(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public ServiceListViewController(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public ServiceListViewController() : base("ServiceListView", NSBundle.MainBundle)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        #endregion

        //strongly typed view accessor
        public new ServiceListView View
        {
            get
            {
                return (ServiceListView)base.View;
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var set = this.CreateBindingSet<ServiceListViewController, ServiceListViewModel>();
            set.Bind(BackButton).To(vm => vm.CloseCommand);
            set.Apply();
        }
    }
}
