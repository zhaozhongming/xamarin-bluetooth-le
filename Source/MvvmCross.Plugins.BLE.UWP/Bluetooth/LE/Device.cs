using System;
using Windows.Devices.Bluetooth.Advertisement;
using MvvmCross.Plugins.BLE.Bluetooth.LE;

namespace MvvmCross.Plugins.BLE.UWP.Bluetooth.LE
{
    public class Device : DeviceBase
    {
        public override Guid ID { get; }
        public override string Name { get; }
        public override int Rssi { get; }

        public Device(ulong address, BluetoothLEAdvertisement advertisement, short rssi)
        {
            ID = new Guid(BitConverter.GetBytes(address));
            Name = advertisement.LocalName;
            Rssi = rssi;
        }
    }
}