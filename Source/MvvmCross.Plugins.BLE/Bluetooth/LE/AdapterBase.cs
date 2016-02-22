using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MvvmCross.Platform;

namespace MvvmCross.Plugins.BLE.Bluetooth.LE
{
    public abstract class AdapterBase : IAdapter
    {
        public event EventHandler<DeviceDiscoveredEventArgs> DeviceAdvertised = delegate { };
        public event EventHandler<DeviceDiscoveredEventArgs> DeviceDiscovered = delegate { };
        public event EventHandler<DeviceConnectionEventArgs> DeviceConnected = delegate { };
        public event EventHandler<DeviceBondStateChangedEventArgs> DeviceBondStateChanged = delegate { };
        public event EventHandler<DeviceConnectionEventArgs> DeviceDisconnected = delegate { };
        public event EventHandler<DeviceConnectionEventArgs> DeviceConnectionLost = delegate { };
        public event EventHandler<DeviceConnectionEventArgs> DeviceConnectionError = delegate { };
        public event EventHandler ScanTimeoutElapsed = delegate { };
        public bool IsScanning { get; private set; }
        public int ScanTimeout { get; set; }

        public virtual IList<IDevice> DiscoveredDevices
        {
            get { return _discoveredDevices; }
        }

        public virtual IList<IDevice> ConnectedDevices
        {
            get { return _connectedDevices; }
        }

        private CancellationTokenSource _scanCancellationTokenSource;
        private readonly IList<IDevice> _discoveredDevices;
        private readonly IList<IDevice> _connectedDevices;

        protected AdapterBase()
        {
            ScanTimeout = 10000;
            _discoveredDevices = new List<IDevice>();
            _connectedDevices = new List<IDevice>();
        }

        public void StartScanningForDevices()
        {
            StartScanningForDevices(new Guid[0]);
        }

        public async void StartScanningForDevices(Guid[] serviceUuids)
        {
            if (IsScanning)
            {
                Mvx.Trace("Adapter: Already scanning!");
                return;
            }

            IsScanning = true;
            _scanCancellationTokenSource = new CancellationTokenSource();

            try
            {
                StartScanningForDevicesNative(serviceUuids);

                await Task.Delay(ScanTimeout, _scanCancellationTokenSource.Token);
                Mvx.Trace("Adapter: Scan timeout has elapsed.");
                StopScan();
                ScanTimeoutElapsed(this, new EventArgs());
            }
            catch (TaskCanceledException)
            {
                Mvx.Trace("Adapter: Scan was cancelled.");
                StopScan();
            }
        }

        private void StopScan()
        {
            StopScanNative();

            if (_scanCancellationTokenSource != null)
            {
                _scanCancellationTokenSource.Dispose();
                _scanCancellationTokenSource = null;
            }
            
            IsScanning = false;
        }

        public void StopScanningForDevices()
        {
            if (_scanCancellationTokenSource != null && !_scanCancellationTokenSource.IsCancellationRequested)
            {
                _scanCancellationTokenSource.Cancel();
            }
            else
            {
                Mvx.Trace("Adapter: Already cancelled scan.");
            }
        }

        protected void HandleDiscoveredDevice(IDevice device)
        {
            DeviceAdvertised(this, new DeviceDiscoveredEventArgs { Device = device });

            if (_discoveredDevices.Contains(device))
                return;

            _discoveredDevices.Add(device);

            DeviceDiscovered(this, new DeviceDiscoveredEventArgs { Device = device });
        }

        protected abstract void StartScanningForDevicesNative(Guid[] serviceUuids);
        protected abstract void StopScanNative();
        public abstract void ConnectToDevice(IDevice device, bool autoconnect = false);
        public abstract void CreateBondToDevice(IDevice device);
        public abstract void DisconnectDevice(IDevice device);
    }
}