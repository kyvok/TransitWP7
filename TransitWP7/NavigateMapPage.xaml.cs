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

            //HACK: this needs to be retrieved from a shared object.
            GeoCoordinate startCoord = (GeoCoordinate)PhoneApplicationService.Current.State["startCoord"];
            GeoCoordinate endCoord = (GeoCoordinate)PhoneApplicationService.Current.State["endCoord"];
            BingMapsRestApi.BingMapsQuery.GetTransitRoute(
                startCoord,
                endCoord,
                DateTime.Now,
                TransitWP7.BingMapsRestApi.TimeType.Departure,
                TransitRouteCalculated,
                new GeoCoordinate[] { startCoord, endCoord });
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
            BingMapsRestApi.BingMapsQuery.GetLocationInfo(this.currentLocation, LocationCallback, null);
        }

        private void LocationCallback(BingMapsRestApi.BingMapsQueryResult result)
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
                        TransitWP7.BingMapsRestApi.Location response = (TransitWP7.BingMapsRestApi.Location)(result.Response.ResourceSets[0].Resources[0]);
                        switch (response.Confidence)
                        {
                            case BingMapsRestApi.ConfidenceLevel.High:
                                myLocation.Foreground = new SolidColorBrush(Colors.Green);
                                break;
                            case BingMapsRestApi.ConfidenceLevel.Medium:
                                myLocation.Foreground = new SolidColorBrush(Colors.Yellow);
                                break;
                            case BingMapsRestApi.ConfidenceLevel.Low:
                                myLocation.Foreground = new SolidColorBrush(Colors.Red);
                                break;
                        }
                        myLocation.Text = String.Format("Current location: {0}",
                            response.Name);
                    });
            }
        }

        private void LocateButton_Click(object sender, EventArgs e)
        {
            // Recenter the map on current user location and preserve the zoom level
            this.mainMap.SetView(this.currentLocation, mainMap.ZoomLevel);
        }

        private void TransitRouteCalculated(BingMapsRestApi.BingMapsQueryResult result)
        {
            if (result.Error == null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    var oneRoute = ((TransitWP7.BingMapsRestApi.Route)(result.Response.ResourceSets[0].Resources[0]));
                    var oneLeg = oneRoute.RouteLegs[0];

                    startLocPushpin.Location = oneLeg.ActualStart.AsGeoCoordinate();
                    startLocPushpin.Content = "Start!";
                    endLocPushpin.Location = oneLeg.ActualEnd.AsGeoCoordinate();
                    endLocPushpin.Content = "End!";

                    foreach (var topLeg in oneLeg.ItineraryItems)
                    {
                        if (topLeg.Instruction != null && topLeg.ManeuverPoint != null)
                        {
                            mainMap.Children.Add(new Pushpin() { Location = topLeg.ManeuverPoint.AsGeoCoordinate(), Content = topLeg.Instruction.Value });
                        }

                        if (topLeg.ChildItineraryItems != null)
                        {
                            foreach (var childLeg in oneLeg.ItineraryItems)
                            {
                                if (childLeg.Instruction != null && childLeg.ManeuverPoint != null)
                                {
                                    mainMap.Children.Add(new Pushpin() { Location = childLeg.ManeuverPoint.AsGeoCoordinate(), Content = childLeg.Instruction.Value });
                                }
                            }
                        }
                    }

                    var path = oneRoute.RoutePaths[0].Line;
                    var pathIndices = oneRoute.RoutePaths[0].RoutePathGeneralization[0].PathIndices;

                    foreach (var index in pathIndices)
                    {
                        routePath.Locations.Add(path[index].AsGeoCoordinate());
                    }
                });
            }
            else
            {
                //TODO: Check what the error is, probably says Better to Walk. If so, calculate walking route.
                //Calculating Walking directions if UserState is not null... super HACKY!
                if (result.UserState != null)
                {
                    BingMapsRestApi.BingMapsQuery.GetWalkingRoute(
                        ((GeoCoordinate[])(result.UserState))[0],
                        ((GeoCoordinate[])(result.UserState))[1],
                        TransitRouteCalculated,
                        null);

                }
            }
        }
    }
}