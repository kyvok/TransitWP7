using System;
using Microsoft.Phone.Controls;
using TransitWP7.ViewModel;

namespace TransitWP7.View
{
    // TODO: distance of the item from origin
    // TODO: calculate results from origin for endpoint, not current userlocation!!!
    public partial class LocationSelectionView : PhoneApplicationPage
    {
        private readonly LocationSelectionViewModel _viewModel = new LocationSelectionViewModel();
        private const string PageTitleStringFormat = "Which {0} location did you mean?";

        public LocationSelectionView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs arg)
        {
            base.OnNavigatedTo(arg);

            this._viewModel.EndpointName = this.NavigationContext.QueryString["endpoint"];

            this.PageTitle.Text = string.Format(PageTitleStringFormat, this._viewModel.EndpointName);
            this.resultsList.ItemsSource = this._viewModel.EndpointName == "start"
                                               ? this._viewModel.Context._possibleStartLocations
                                               : this._viewModel.Context._possibleEndLocations;
        }

        // TODO: backing up from here what happens?
        private void resultsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            this._viewModel.SelectionMade(this.resultsList.SelectedIndex);
            this.NavigationService.GoBack();
        }
    }
}