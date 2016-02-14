using MvvmCross.Platform;
using MvvmCross.Platform.Plugins;
using MvvmCross.Plugins.BLE.Bluetooth.LE;
using MvvmCross.Plugins.BLE.UWP.Bluetooth.LE;

namespace MvvmCross.Plugins.BLE.UWP
{
    public class Plugin
     : IMvxPlugin
    {
        public void Load()
        {
            Mvx.LazyConstructAndRegisterSingleton<IAdapter>(() => new Adapter());
        }
    }
}