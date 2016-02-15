using System;
using Windows.Devices.Bluetooth.Advertisement;
using MvvmCross.Plugins.BLE.Bluetooth.LE;

namespace MvvmCross.Plugins.BLE.WindowsUWP.Bluetooth.LE
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

