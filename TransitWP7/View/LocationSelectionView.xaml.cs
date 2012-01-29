using System;
using Microsoft.Phone.Controls;
using TransitWP7.ViewModel;

namespace TransitWP7.View
{
    // TODO: distance of the item from origin
    // TODO: calculate results from origin for endpoint, not current userlocation!!!
    public partial class LocationSelectionView : PhoneApplicationPage
    {
        private readonly LocationSelectionViewModel _viewModel;
        private const string PageTitleStringFormat = "Which {0} location did you mean?";

        public LocationSelectionView()
        {
            this.InitializeComponent();
            this._viewModel = ViewModelLocator.LocationSelectionViewModelStatic;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs arg)
        {
            base.OnNavigatedTo(arg);

            this._viewModel.EndpointName = this.NavigationContext.QueryString["endpoint"];

            this.PageTitle.Text = string.Format(PageTitleStringFormat, this._viewModel.EndpointName);
            this.resultsList.ItemsSource = this._viewModel.LocationDescriptions;
        }

        // TODO: backing up from here what happens?
        private void ResultsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            this._viewModel.SelectionMade(this.resultsList.SelectedIndex);
            this.NavigationService.GoBack();
        }
    }
}