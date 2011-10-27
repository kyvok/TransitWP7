//copyright

using System.Globalization;
using System;
using System.Device.Location;
using System.Windows;
namespace TransitWP7.ViewModels
{
    public class MainPageViewModel
    {
        public MainPageViewModel()
        {
            //GeoLocation.Instance.GeoWatcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(this.watcher_PositionChanged);
        }

        public TransitRequestContext Context
        {
            get
            {
                return TransitRequestContext.Current;
            }
        }

        // Event handler for the GeoCoordinateWatcher.PositionChanged event.
        //void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        //{
        //    TransitRequestContext.Current.UserLocation = e.Position.Location;

        //    // Poll bing maps about the location
        //    ProxyQuery.GetLocationAddress(TransitRequestContext.Current.UserLocation, LocationCallback, null);
        //}

        //private void LocationCallback(ProxyQueryResult result)
        //{
        //    if (result.Error != null)
        //    {
        //        MessageBox.Show(result.Error.Message, "LocationCallback obtained an error!", MessageBoxButton.OK);
        //    }
        //    else
        //    {
        //        LocationDescription locationDesc = result.LocationDescriptions[0];
        //        this.currentAddress = locationDesc.DisplayName;
        //        this.currentConfidence = locationDesc.Confidence;
        //        System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
        //        {
        //            ImageBrush image = new ImageBrush();
        //            image.ImageSource = (ImageSource)new ImageSourceConverter().ConvertFromString(LocationImage.GetImagePath(locationDesc.StateOrProvince));
        //            this.LayoutRoot.Background = image;

        //            if (this.startingInput.Text == Globals.MyCurrentLocationText)
        //            {
        //                switch (this.currentConfidence)
        //                {
        //                    case "High":
        //                        this.startAddress.Foreground = new SolidColorBrush(Colors.Green);
        //                        break;
        //                    case "Medium":
        //                        this.startAddress.Foreground = new SolidColorBrush(Colors.Yellow);
        //                        break;
        //                    case "Low":
        //                        this.startAddress.Foreground = new SolidColorBrush(Colors.Red);
        //                        break;
        //                }
        //                this.startAddress.Text = String.Format("Address: {0}",
        //                    this.currentAddress);
        //            }
        //        });
        //    }
        //}

        public void SwapEndStartLocations()
        {
            string tempSwap = string.Empty;

            tempSwap = this.Context.StartName;
            this.Context.StartName = this.Context.EndName;
            this.Context.EndName = tempSwap;

            tempSwap = this.Context.StartAddress;
            this.Context.StartAddress = this.Context.EndAddress;
            this.Context.EndAddress = tempSwap;
        }
    }

    //TODO a converter based on attribute on enum to display in the list.
    //public class TimeConditionEnumConverter : System.Windows.Data.IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        TimeCondition tc = (TimeCondition)value;
    //        return tc;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        string s = value as string;
    //        bool b;

    //        if (bool.TryParse(s, out b))
    //        {
    //            return !b;
    //        }
    //        return false;
    //    }
    //}

}