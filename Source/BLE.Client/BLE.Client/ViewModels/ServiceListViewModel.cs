using System.Collections.ObjectModel;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.BLE.Bluetooth.LE;

namespace BLE.Client.ViewModels
{
    public class ServiceListViewModel : MvxViewModel
    {
        public ObservableCollection<IService> Services { get; set; } = new ObservableCollection<IService>();
        public ServiceListViewModel()
        {
            
        }
    }
}