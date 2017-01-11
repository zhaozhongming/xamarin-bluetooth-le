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
	[Register ("DeviceListViewController")]
	partial class DeviceListViewController
	{
		[Outlet]
		AppKit.NSTextField BleStatusText { get; set; }

		[Outlet]
		AppKit.NSCollectionView DeviceCollectionView { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator ProgressIndicator { get; set; }

		[Outlet]
		AppKit.NSButton ScanButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (DeviceCollectionView != null) {
				DeviceCollectionView.Dispose ();
				DeviceCollectionView = null;
			}

			if (ScanButton != null) {
				ScanButton.Dispose ();
				ScanButton = null;
			}

			if (BleStatusText != null) {
				BleStatusText.Dispose ();
				BleStatusText = null;
			}

			if (ProgressIndicator != null) {
				ProgressIndicator.Dispose ();
				ProgressIndicator = null;
			}
		}
	}
}
