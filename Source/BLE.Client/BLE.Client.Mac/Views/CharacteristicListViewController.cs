using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using MvvmCross.Mac.Views;

namespace BLE.Client.Mac.Views
{
    public partial class CharacteristicListViewController : MvxViewController
    {
        #region Constructors

        // Called when created from unmanaged code
        public CharacteristicListViewController(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public CharacteristicListViewController(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public CharacteristicListViewController() : base("CharacteristicListView", NSBundle.MainBundle)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        #endregion

        //strongly typed view accessor
        public new CharacteristicListView View
        {
            get
            {
                return (CharacteristicListView)base.View;
            }
        }
    }
}
