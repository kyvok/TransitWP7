using Microsoft.Phone.Controls;
using TransitWP7.ViewModel;

namespace TransitWP7.View
{
    public partial class SettingsView : PhoneApplicationPage
    {
        private readonly SettingsViewModel _viewModel;

        public SettingsView()
        {
            this.InitializeComponent();
            this._viewModel = ViewModelLocator.SettingsViewModelStatic;
            this.DataContext = this._viewModel;
        }
    }
}