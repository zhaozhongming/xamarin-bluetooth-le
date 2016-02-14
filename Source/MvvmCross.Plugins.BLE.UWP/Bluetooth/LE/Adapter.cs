using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using MvvmCross.Platform;
using MvvmCross.Plugins.BLE.Bluetooth.LE;
using MvvmCross.Plugins.BLE.Extensions;

namespace MvvmCross.Plugins.BLE.UWP.Bluetooth.LE
{
    public class Adapter : AdapterBase
    {
        private readonly BluetoothLEAdvertisementWatcher _bleWatcher = new BluetoothLEAdvertisementWatcher();

        public Adapter()
        {
            _bleWatcher.Received += BleWatcherOnReceived;
        }

        private void BleWatcherOnReceived(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {

            var device = new Device(args.BluetoothAddress, args.Advertisement, args.RawSignalStrengthInDBm);
            HandleDiscoveredDevice(device);
        }

        protected override void StartScanningForDevicesNative(Guid[] serviceUuids)
        {
            _bleWatcher.AdvertisementFilter.Advertisement.ServiceUuids.Clear();
            foreach (var uuid in serviceUuids)
            {
                _bleWatcher.AdvertisementFilter.Advertisement.ServiceUuids.Add(uuid);
            }
            _bleWatcher.Start();
        }

        protected override void StopScanNative()
        {
            _bleWatcher.Stop();
        }

        public override void ConnectToDevice(IDevice device, bool autoconnect = false)
        {
            throw new NotImplementedException();
        }

        public override void CreateBondToDevice(IDevice device)
        {
            throw new NotImplementedException();
        }

        public override void DisconnectDevice(IDevice device)
        {
            throw new NotImplementedException();
        }
    }
}

