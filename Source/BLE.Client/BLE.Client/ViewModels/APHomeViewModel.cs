using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BLE.Client.Extensions;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Extensions;
using Plugin.Permissions.Abstractions;
using Plugin.Settings.Abstractions;

namespace BLE.Client.ViewModels
{
    class APHomeViewModel : BaseViewModel
    {

        public MvxCommand<ModelType> DoWork => new MvxCommand<ModelType>((mtype) => OnDoWork(mtype));

        public APHomeViewModel(IBluetoothLE bluetoothLe, IAdapter adapter, IUserDialogs userDialogs, ISettings settings, IPermissions permissions) : base(adapter)
        {

        }

        private void OnDoWork(ModelType selectedModelType)
        {
            ShowViewModel<DeviceListViewModel>(new MvxBundle(new Dictionary<string, string> { { "SelectedModelType", selectedModelType.ToString() } }));
            
        }
    }
}
