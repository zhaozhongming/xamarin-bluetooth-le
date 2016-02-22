using System;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using MvvmCross.Plugins.BLE.Bluetooth.LE;

namespace MvvmCross.Plugins.BLE.WindowsUWP.Bluetooth.LE
{
    public class Adapter : AdapterBase
    {
        private readonly BluetoothLEAdvertisementWatcher _bleWatcher = new BluetoothLEAdvertisementWatcher();

        public Adapter()
        {
        }

        private async void BleWatcherOnReceived(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            var dev = await BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress);
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
            _bleWatcher.Received += BleWatcherOnReceived;
            _bleWatcher.Start();
        }

        protected override void StopScanNative()
        {
            _bleWatcher.Stop();
            _bleWatcher.Received -= BleWatcherOnReceived;
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

