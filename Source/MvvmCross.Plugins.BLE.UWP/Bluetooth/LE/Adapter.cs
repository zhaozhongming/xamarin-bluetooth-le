using System;
using System.Collections.Generic;
using MvvmCross.Plugins.BLE.Bluetooth.LE;

namespace MvvmCross.Plugins.BLE.UWP.Bluetooth.LE
{
    public class Adapter : IAdapter
    {
        public event EventHandler<DeviceDiscoveredEventArgs> DeviceAdvertised = delegate { };
        public event EventHandler<DeviceDiscoveredEventArgs> DeviceDiscovered = delegate { };
        public event EventHandler<DeviceConnectionEventArgs> DeviceConnected = delegate { };
        public event EventHandler<DeviceBondStateChangedEventArgs> DeviceBondStateChanged = delegate { };
        public event EventHandler<DeviceConnectionEventArgs> DeviceDisconnected = delegate { };
        public event EventHandler<DeviceConnectionEventArgs> DeviceConnectionLost = delegate { };
        public event EventHandler<DeviceConnectionEventArgs> DeviceConnectionError = delegate { };
        public event EventHandler ScanTimeoutElapsed = delegate { };
        public bool IsScanning { get; }
        public int ScanTimeout { get; set; } = 10000;
        public IList<IDevice> DiscoveredDevices { get; } = new List<IDevice>();
        public IList<IDevice> ConnectedDevices { get; } = new List<IDevice>();

        public void StartScanningForDevices()
        {
            throw new NotImplementedException();
        }

        public void StartScanningForDevices(Guid[] serviceUuids)
        {
            throw new NotImplementedException();
        }

        public void StopScanningForDevices()
        {
            throw new NotImplementedException();
        }

        public void ConnectToDevice(IDevice device, bool autoconnect = false)
        {
            throw new NotImplementedException();
        }

        public void CreateBondToDevice(IDevice device)
        {
            throw new NotImplementedException();
        }

        public void DisconnectDevice(IDevice device)
        {
            throw new NotImplementedException();
        }
    }
}

