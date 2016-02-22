using System.Collections.ObjectModel;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.BLE.Bluetooth.LE;

namespace BLE.Client.ViewModels
{
    public class CharacteristicListViewModel : MvxViewModel
    {
        public ObservableCollection<ICharacteristic> Characteristics { get; set; } = new ObservableCollection<ICharacteristic>();
        public CharacteristicListViewModel()
        {

        }
    }
}