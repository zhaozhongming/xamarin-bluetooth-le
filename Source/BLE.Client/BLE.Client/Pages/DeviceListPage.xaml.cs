using BLE.Client.ViewModels;

namespace BLE.Client.Pages
{
    public partial class DeviceListPage
    {
        public DeviceListPage()
        {
            InitializeComponent();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            var bindingContext = BindingContext as BaseViewModel;

            if (bindingContext != null)
                bindingContext.ViewDisappearing();

        }
      
    }
}
