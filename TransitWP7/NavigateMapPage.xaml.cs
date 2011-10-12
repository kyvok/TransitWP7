using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;

namespace TransitWP7
{
    public partial class NavigateMapPage : PhoneApplicationPage
    {
        GeoCoordinate currentLocation = null;
        private GeoCoordinateWatcher highGeowatcher = null;

        public NavigateMapPage()
        {
            InitializeComponent();
            // GeoLocation.Instance.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(this.watcher_PositionChanged);

            currentLocation = new GeoCoordinate(0, 0);

            // initialize gps data
            this.highGeowatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            this.highGeowatcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(this.watcher_PositionChanged);
            this.highGeowatcher.MovementThreshold = 20;
            this.highGeowatcher.Start();

            // set the credentials correctly
            Microsoft.Phone.Controls.Maps.ApplicationIdCredentialsProvider credProvider = new Microsoft.Phone.Controls.Maps.ApplicationIdCredentialsProvider(BingMapsRestApi.BingMapsKey.Key);
            this.mainMap.CredentialsProvider = credProvider;

            // TODO: figure out how to do this in xaml
            // this.mainMap.Width = this.LayoutRoot.ColumnDefinitions[0].ActualWidth;
            // this.mainMap.Height = this.LayoutRoot.RowDefinitions[0].ActualHeight;
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
            BingMapsRestApi.BingMapsQuery.GetLocationInfo(this.currentLocation, LocationCallback);
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
                        switch(response.Confidence)
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
    }
}