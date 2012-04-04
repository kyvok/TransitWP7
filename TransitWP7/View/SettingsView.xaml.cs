namespace TransitWP7.View
{
    using Microsoft.Phone.Controls;
    using TransitWP7.ViewModel;

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