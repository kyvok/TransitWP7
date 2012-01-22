using System;
using Microsoft.Phone.Controls;

namespace TransitWP7
{
    //TODO: distance of the item from origin
    //TODO: calculate results from origin for endpoint, not current userlocation!!!
    public partial class LocationSelectionView : PhoneApplicationPage
    {
        private readonly ViewModels.LocationSelectionViewModel _viewModel = new ViewModels.LocationSelectionViewModel();
        private const string PageTitleStringFormat = "Which {0} location did you mean?";

        public LocationSelectionView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs arg)
        {
            base.OnNavigatedTo(arg);

            _viewModel.endpointName = this.NavigationContext.QueryString["endpoint"];

            this.PageTitle.Text = String.Format(PageTitleStringFormat, _viewModel.endpointName);
            this.resultsList.ItemsSource = _viewModel.endpointName == "start"
                                               ? _viewModel.Context._possibleStartLocations
                                               : _viewModel.Context._possibleEndLocations;
        }

        //TODO: backing up from here what happens?

        private void resultsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _viewModel.SelectionMade(this.resultsList.SelectedIndex);
            this.NavigationService.GoBack();
        }
    }
}