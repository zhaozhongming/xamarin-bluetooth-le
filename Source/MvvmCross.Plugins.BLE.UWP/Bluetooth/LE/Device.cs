using System;
using System.Linq;
using Windows.Devices.Bluetooth.Advertisement;
using MvvmCross.Plugins.BLE.Bluetooth.LE;

namespace MvvmCross.Plugins.BLE.WindowsUWP.Bluetooth.LE
{
    public class Device : DeviceBase
    {
        public override Guid ID { get; }
        public override string Name { get; }
        public override int Rssi { get; }

        public Device(ulong address, BluetoothLEAdvertisement advertisement, short rssi)
        {
            var guidBytes = new byte[16];
            var addressBytes = BitConverter.GetBytes(address).Reverse().ToArray();
            Array.Copy(addressBytes, 0, guidBytes, guidBytes.Length - addressBytes.Length, addressBytes.Length);

            ID = new Guid(guidBytes);
            Name = advertisement.LocalName;
            Rssi = rssi;
        }
    }
}