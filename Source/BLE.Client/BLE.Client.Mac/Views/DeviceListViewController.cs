using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using MvvmCross.Mac.Views;
using BLE.Client.ViewModels;
using System.Threading.Tasks;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platform;
using MvvmCross.Platform.WeakSubscription;
using MvvmCross.Binding.ExtensionMethods;
using BLE.Client.Mac.Views.Collections;

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

            var layout = new NSCollectionViewFlowLayout();
            layout.ItemSize = new CoreGraphics.CGSize(200, 50);
            layout.SectionInset = new NSEdgeInsets(10, 10, 10, 10);
            layout.MinimumInteritemSpacing = 20;
            layout.MinimumLineSpacing = 20;
            DeviceCollectionView.CollectionViewLayout = layout;
          
            var source = new CollectionViewSoruce<DeviceItemController>(DeviceCollectionView);

            DeviceCollectionView.DataSource = source;

            var collectionDeleagte = new CollectionViewDelegate(source);
            DeviceCollectionView.Delegate = collectionDeleagte;

            var set = this.CreateBindingSet<DeviceListViewController, DeviceListViewModel>();
            set.Bind(ScanButton).To(vm => vm.RefreshCommand);
            set.Bind(this).For(t => t.ShouldAnimateProgress).To(vm => vm.IsRefreshing);
            set.Bind(source).For(s => s.ItemsSource).To(vm => vm.Devices);
            //set.Bind(collectionDeleagte).For(d => d.SelectedItem).To(vm => vm.SelectedDevice).OneWayToSource();
            set.Apply();

            collectionDeleagte.SelectedItemChanged += (sender, e) =>
            {
                ((DeviceListViewModel)ViewModel).SelectedDevice = collectionDeleagte.SelectedItem;//DeviceLis;
            };
        }

        public bool ShouldAnimateProgress
        {
            set
            {
                if (value)
                {
                    ProgressIndicator.StartAnimation(this);
                }
                else
                {
                    ProgressIndicator.StopAnimation(this);
                }
            }
            get { return true; }
        }
    }
}
