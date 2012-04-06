namespace TransitWP7.View
{
    using System.Windows;

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

        private void ResetAutoCompleteButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Resetting autocomplete data", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                AutoCompleteDataManager.ResetData();
            }
        }
    }
}