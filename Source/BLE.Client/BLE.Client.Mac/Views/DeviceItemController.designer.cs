// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace BLE.Client.Mac.Views
{
	[Register ("DeviceItemController")]
	partial class DeviceItemController
	{
		[Outlet]
		AppKit.NSTextField Id { get; set; }

		[Outlet]
		AppKit.NSTextField Name { get; set; }

		[Outlet]
		AppKit.NSTextField Rssi { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Id != null) {
				Id.Dispose ();
				Id = null;
			}

			if (Name != null) {
				Name.Dispose ();
				Name = null;
			}

			if (Rssi != null) {
				Rssi.Dispose ();
				Rssi = null;
			}
		}
	}
}
