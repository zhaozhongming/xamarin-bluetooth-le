using System;
using AppKit;
using BLE.Client.Mac.Views.Collections;
using Foundation;
using MvvmCross.Binding.ExtensionMethods;

namespace BLE.Client.Mac.Views.Collections
{
     public class CollectionViewDelegate : NSCollectionViewDelegate
    {
        readonly ICollectionViewSource source;

        public CollectionViewDelegate(ICollectionViewSource source)
        {
            this.source = source;
        }
        public override void ItemsSelected(NSCollectionView collectionView, NSSet indexPaths)
        {
            var path = indexPaths.ElementAt(0) as NSIndexPath;

            if (path != null)
            {
                SelectedItem = source.ItemsSource?.ElementAt((int)path.Item);
                SelectedItemChanged?.Invoke(collectionView, EventArgs.Empty);
            }
        }

        public object SelectedItem
        {
            get;
            set;
        }

        public event EventHandler SelectedItemChanged;
    }
}
