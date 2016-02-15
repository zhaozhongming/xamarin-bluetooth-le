using MvvmCross.Platform;
using MvvmCross.Platform.Plugins;
using MvvmCross.Plugins.BLE.Bluetooth.LE;
using MvvmCross.Plugins.BLE.WindowsUWP.Bluetooth.LE;

namespace MvvmCross.Plugins.BLE.WindowsUWP
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