using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;

namespace TransitWP7.Views
{
    public partial class TransitTripMapOverview : UserControl
    {
        GeoCoordinate currentLocation = null;

        public TransitTripMapOverview()
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


            TransitRequestContext.Current.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Current_PropertyChanged);
        }

        void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            TransitDescription description = TransitRequestContext.Current.SelectedTransitTrip;

            foreach (var step in description.ItinerarySteps)
            {
                string instructContent = string.Empty;
                if (step.IconType != "")
                {
                    instructContent = step.IconType.Substring(0, 1);
                    if (step.IconType.StartsWith("B"))
                    {
                        instructContent += step.BusNumber;
                    }
                }

                mainMap.Children.Add(new Pushpin() { Location = step.GeoCoordinate, Content = instructContent });
            }

            routePath.Locations.Clear();
            foreach (var pathPoint in description.PathPoints)
            {
                routePath.Locations.Add(pathPoint);
            }

            mainMap.SetView(description.MapView);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            GeoLocation.Instance.GeoWatcher.PositionChanged -= new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(this.watcher_PositionChanged);
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
                        //switch (response.Confidence)
                        //{
                        //    case "High":
                        //        myLocation.Foreground = new SolidColorBrush(Colors.Green);
                        //        break;
                        //    case "Medium":
                        //        myLocation.Foreground = new SolidColorBrush(Colors.Yellow);
                        //        break;
                        //    case "Low":
                        //        myLocation.Foreground = new SolidColorBrush(Colors.Red);
                        //        break;
                        //}
                        //myLocation.Text = String.Format("Current location: {0}",
                        //    response.DisplayName);
                    });
            }
        }

        private void LocateButton_Click(object sender, EventArgs e)
        {
            // Recenter the map on current user location and preserve the zoom level
            this.mainMap.SetView(this.currentLocation, mainMap.ZoomLevel);
        }

        //private void TransitDirectionListMenuItem_Click(object sender, EventArgs e)
        //{
        //    NavigationService.Navigate(new Uri("/TransitStepsPage.xaml", UriKind.Relative));
        //}
    }
}
