using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platform.Converters;
using BLE.Client.ViewModels;

namespace BLE.Client.Mac.Views
{
    public partial class DeviceItemController : NSCollectionViewItem, IMvxBindingContextOwner
    {
        #region Constructors

        // Called when created from unmanaged code
        public DeviceItemController(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public DeviceItemController(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public DeviceItemController() : base("DeviceItem", NSBundle.MainBundle)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
            this.CreateBindingContext();
        }



        #endregion

        //strongly typed view accessor
        public new DeviceItem View
        {
            get
            {
                return (DeviceItem)base.View;
            }
        }



        public IMvxBindingContext BindingContext
        {
            get;

            set;
        }

        public object DataContext
        {
            get { return this.BindingContext.DataContext; }
            set { this.BindingContext.DataContext = value; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //this.DelayBind(() =>
            //{
            var set = this.CreateBindingSet<DeviceItemController, DeviceListItemViewModel>();
            set.Bind(Name).To(vm => vm.Name);
            set.Bind(Id).To(vm => vm.Id).WithConversion(new GuidToStringConverter());
            set.Bind(Rssi).To(vm => vm.Rssi);
            set.Apply();
            //});
        }

        public class GuidToStringConverter : MvxValueConverter<Guid, string>
        {
            protected override string Convert(Guid value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return value.ToString();
            }
        }
    }
}
