using System;

using AppKit;
using Foundation;

namespace BLE.Client.Mac
{
    public partial class MainWindowController : NSViewController
    {
        public MainWindowController(IntPtr handle) : base(handle)
        {
        }

           public MainWindowController() : base()
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.
        }

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }
    }
}
