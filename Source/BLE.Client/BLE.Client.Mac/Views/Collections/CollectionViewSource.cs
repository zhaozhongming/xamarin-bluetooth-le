using System;
using System.Collections;
using System.Collections.Specialized;
using AppKit;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.ExtensionMethods;
using MvvmCross.Platform;
using MvvmCross.Platform.WeakSubscription;

namespace BLE.Client.Mac.Views.Collections
{
    public interface ICollectionViewSource
    {
        IEnumerable ItemsSource { get; }
    }
    
    public class CollectionViewSoruce<T> : NSCollectionViewDataSource, ICollectionViewSource where T: NSCollectionViewItem, IMvxBindingContextOwner, new()
    {
        private IEnumerable _itemsSource;
        private IDisposable _subscription;
        readonly NSCollectionView _collectionView;
   
        public CollectionViewSoruce(NSCollectionView collectionView)
        {
            _collectionView = collectionView;
        }

        public override NSCollectionViewItem GetItem(NSCollectionView collectionView, NSIndexPath indexPath)
        {
            var collectionItem = new T();
            //ToDo var item = collectionView.MakeItem("DeviceItem", indexPath);

            var vm = this.ItemsSource?.ElementAt((int)indexPath.Item);
            collectionItem.BindingContext.DataContext = vm;
            return collectionItem;
        }

        public override nint GetNumberofItems(NSCollectionView collectionView, nint section)
        {
            if (ItemsSource == null)
                return 0;

            return ItemsSource.Count();
        }

        public override nint GetNumberOfSections(NSCollectionView collectionView)
        {
            return 1;
        }

        protected NSCollectionView CollectionView => this._collectionView;

        public virtual void ReloadData()
        {
            try
            {
                this._collectionView.ReloadData();
            }
            catch (Exception exception)
            {
                Mvx.Warning("Exception masked during CollectionView ReloadData {0}", exception.ToString());
            }
        }

        //[MvxSetToNullAfterBinding]
        public virtual IEnumerable ItemsSource
        {
            get { return this._itemsSource; }
            set
            {
                if (Object.ReferenceEquals(this._itemsSource, value))
                    return;

                if (this._subscription != null)
                {
                    this._subscription.Dispose();
                    this._subscription = null;
                }
                this._itemsSource = value;
                var collectionChanged = this._itemsSource as INotifyCollectionChanged;
                if (collectionChanged != null)
                {
                    this._subscription = collectionChanged.WeakSubscribe(CollectionChangedOnCollectionChanged);
                }
                this.ReloadData();
            }
        }

        protected virtual void CollectionChangedOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            ReloadData();
        }

    }
}
