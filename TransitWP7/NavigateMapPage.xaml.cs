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

namespace TransitWP7
{
    public partial class NavigateMapPage : PhoneApplicationPage
    {
        private double latitude = 0;
        private double longitude = 0;

        public NavigateMapPage()
        {
            InitializeComponent();
            GeoLocation.Instance.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(this.watcher_PositionChanged);
        }

        private void lowButton_Click(object sender, RoutedEventArgs e)
        {
            GeoLocation.Instance.Accuracy = Accuracy.Low;
        }

        private void mediumButton_Click(object sender, RoutedEventArgs e)
        {
            GeoLocation.Instance.Accuracy = Accuracy.Medium;
        }

        private void highButton_Click(object sender, RoutedEventArgs e)
        {
            GeoLocation.Instance.Accuracy = Accuracy.High;
        }

        // Event handler for the GeoCoordinateWatcher.PositionChanged event.
        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            // TODO: change this to databinding
            this.latitudeValue.Text = e.Position.Location.Latitude.ToString();
            this.longitudeValue.Text = e.Position.Location.Longitude.ToString();

            this.latitude = e.Position.Location.Latitude;
            this.longitude = e.Position.Location.Longitude;
        }

        public string Latitude
        {
            get
            {
                return this.latitude.ToString();
            }
        }

        public string Longitude
        {
            get
            {
                return this.longitude.ToString();
            }
        }
    }
}