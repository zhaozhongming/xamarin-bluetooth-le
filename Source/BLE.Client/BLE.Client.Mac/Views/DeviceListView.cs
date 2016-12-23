using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace BLE.Client.Mac.Views
{
    public partial class DeviceListView : AppKit.NSView
    {
        #region Constructors

        // Called when created from unmanaged code
        public DeviceListView(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public DeviceListView(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        #endregion
    }
}
