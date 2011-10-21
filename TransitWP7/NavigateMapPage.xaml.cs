//TODO: copyright info

namespace TransitWP7
{
    using System;
    using System.Device.Location;
    using System.Windows.Media;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Controls.Maps;
    using Microsoft.Phone.Shell;
    using System.Windows;

    public partial class NavigateMapPage : PhoneApplicationPage
    {
        GeoCoordinate currentLocation = null;

        public NavigateMapPage()
        {
            InitializeComponent();
            this.currentLocation = GeoLocation.Instance.GeoWatcher.Position.Location;
            this.meIndicator.Location = this.currentLocation;
            this.mainMap.SetView(this.currentLocation, mainMap.ZoomLevel);

            // initialize gps data
            GeoLocation.Instance.GeoWatcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(this.watcher_PositionChanged);

            // set the credentials correctly
            ApplicationIdCredentialsProvider credProvider = new ApplicationIdCredentialsProvider(ApiKeys.BingMapsKey);
            this.mainMap.CredentialsProvider = credProvider;

            // TODO: figure out how to do this in xaml
            // this.mainMap.Width = this.LayoutRoot.ColumnDefinitions[0].ActualWidth;
            // this.mainMap.Height = this.LayoutRoot.RowDefinitions[0].ActualHeight;
        }

        //TODO: Navigation is messed up.
        //protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        //{
        //    base.OnBackKeyPress(e);
        //    NavigationService.Navigate(new Uri("/SelectTransitResultPage.xaml", UriKind.Relative));
        //}

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            TransitDescription description = (TransitDescription)PhoneApplicationService.Current.State["transitToDisplay"];

            startLocPushpin.Location = description.StartLocation;
            startLocPushpin.Content = "Start!";
            endLocPushpin.Location = description.EndLocation;
            endLocPushpin.Content = "End!";

            foreach (var step in description.ItinerarySteps)
            {
                mainMap.Children.Add(new Pushpin() { Location = step.GeoCoordinate, Content = step.Instruction });
            }

            routePath.Locations.Clear();
            foreach (var gc in description.GetMapPolyline().Locations)
            {
                routePath.Locations.Add(gc);
            }

            mainMap.SetView(description.GetMapView());
        }

        // Event handler for the GeoCoordinateWatcher.PositionChanged event.
        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            // determine if we should continue tracking
            if (Math.Abs(this.currentLocation.Latitude - this.mainMap.TargetCenter.Latitude) <= 0.0000000000001
                && Math.Abs(this.currentLocation.Longitude - this.mainMap.TargetCenter.Longitude) <= 0.0000000000001)
            {
                this.mainMap.SetView(e.Position.Location, mainMap.ZoomLevel);
            }

            this.currentLocation = e.Position.Location;

            // update my location
            // TODO: switch this to databinding
            this.meIndicator.Location = this.currentLocation;

            // Poll bing maps about the location
            ProxyQuery.GetLocationAddress(this.currentLocation, LocationCallback, null);
        }

        private void LocationCallback(ProxyQueryResult result)
        {
            if (result.Error != null)
            {
                Console.WriteLine("obtained an error!");
                Console.WriteLine(result.Error.Message);
            }
            else
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        LocationDescription response = result.LocationDescriptions[0];
                        switch (response.Confidence)
                        {
                            case "High":
                                myLocation.Foreground = new SolidColorBrush(Colors.Green);
                                break;
                            case "Medium":
                                myLocation.Foreground = new SolidColorBrush(Colors.Yellow);
                                break;
                            case "Low":
                                myLocation.Foreground = new SolidColorBrush(Colors.Red);
                                break;
                        }
                        myLocation.Text = String.Format("Current location: {0}",
                            response.DisplayName);
                    });
            }
        }

        private void LocateButton_Click(object sender, EventArgs e)
        {
            // Recenter the map on current user location and preserve the zoom level
            this.mainMap.SetView(this.currentLocation, mainMap.ZoomLevel);
        }
    }
}