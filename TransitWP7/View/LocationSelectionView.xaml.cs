namespace TransitWP7.View
{
    using System.Globalization;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using TransitWP7.ViewModel;

    public partial class LocationSelectionView : PhoneApplicationPage
    {
        private readonly LocationSelectionViewModel _viewModel;
        private const string PageTitleStringFormat = "Which '{0}' did you want?";

        public LocationSelectionView()
        {
            this.InitializeComponent();
            this._viewModel = ViewModelLocator.LocationSelectionViewModelStatic;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this._viewModel.EndpointName = this.NavigationContext.QueryString["endpoint"];
            this.PageTitle.Text = string.Format(CultureInfo.InvariantCulture, PageTitleStringFormat, this.NavigationContext.QueryString["query"]);
        }

        private void ResultsListSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            this._viewModel.SelectionMade(this.resultsList.SelectedIndex);
            this.NavigationService.GoBack();
        }
    }
}