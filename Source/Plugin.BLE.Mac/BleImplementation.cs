using System;
using CoreBluetooth;
using CoreFoundation;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Mac;
using Plugin.BLE.Extensions;

namespace Plugin.BLE
{
    internal class BleImplementation : BleImplementationBase
    {
        private CBCentralManager _centralManager;

        public BleImplementation()
        {
            Trace.TraceImplementation = Console.WriteLine;
        }

        protected override void InitializeNative()
        {
            _centralManager = new CBCentralManager(DispatchQueue.CurrentQueue);
            _centralManager.UpdatedState += (s, e) => State = GetState();
            State = GetState();
        }

        protected override BluetoothState GetInitialStateNative()
        {
            return GetState();
        }

        protected override IAdapter CreateNativeAdapter()
        {
            return new Adapter(_centralManager);
        }

        private BluetoothState GetState()
        {
            return _centralManager.State.ToBluetoothState();
        }
    }
}