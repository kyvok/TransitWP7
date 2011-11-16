//copyright

using System.Globalization;
using System;
using System.Device.Location;
using System.Windows;
using System.ComponentModel;
namespace TransitWP7.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private string endName;
        private string startName;
        private string endAddress;
        private string startAddress;

        public MainPageViewModel()
        {
            GeoLocation.Instance.GeoWatcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(this.watcher_PositionChanged);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TransitRequestContext Context
        {
            get
            {
                return TransitRequestContext.Current;
            }
        }

        // Event handler for the GeoCoordinateWatcher.PositionChanged event.
        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            TransitRequestContext.Current.UserGeoCoordinate = e.Position.Location;

            // Poll bing maps about the location
            ProxyQuery.GetLocationAddress(TransitRequestContext.Current.UserGeoCoordinate, LocationCallback, null);
        }

        private void LocationCallback(ProxyQueryResult result)
        {
            if (result.Error != null)
            {
                MessageBox.Show(result.Error.Message, "LocationCallback obtained an error!", MessageBoxButton.OK);
            }
            else
            {
                TransitRequestContext.Current.UserCurrentLocation = result.LocationDescriptions[0];
            }
        }

        public void SwapEndStartLocations()
        {
            string tempSwap = string.Empty;

            tempSwap = this.StartName;
            this.StartName = this.EndName;
            this.EndName = tempSwap;

            tempSwap = this.StartAddress;
            this.StartAddress = this.EndAddress;
            this.EndAddress = tempSwap;
        }

        public string EndName
        {
            get
            {
                return this.endName;
            }
            set
            {
                if (value != this.endName)
                {
                    this.endName = value;
                    if (string.Empty.Equals(this.endName))
                    {
                        this.endName = Globals.MyCurrentLocationText;
                        this.EndAddress = this.Context.UserCurrentLocation.Address;
                        this.Context.SelectedEndingLocation = this.Context.UserCurrentLocation;
                    }
                    else
                    {
                        this.EndAddress = string.Empty;
                        this.Context.SelectedEndingLocation = null;
                    }
                    this.RaisePropertyChanged("EndName");
                }
            }
        }

        public string StartName
        {
            get
            {
                return this.startName;
            }
            set
            {
                if (value != this.startName)
                {
                    this.startName = value;
                    if (string.Empty.Equals(this.startName))
                    {
                        this.startName = Globals.MyCurrentLocationText;
                        this.StartAddress = this.Context.UserCurrentLocation.Address;
                        this.Context.SelectedStartingLocation = this.Context.UserCurrentLocation;
                    }
                    else
                    {
                        this.StartAddress = string.Empty;
                        this.Context.SelectedStartingLocation = null;
                    }
                    this.RaisePropertyChanged("StartName");
                }
            }
        }

        public string EndAddress
        {
            get
            {
                return this.endAddress;
            }
            set
            {
                if (value != this.endAddress)
                {
                    this.endAddress = value;
                    this.RaisePropertyChanged("EndAddress");
                }
            }
        }

        public string StartAddress
        {
            get
            {
                return this.startAddress;
            }
            set
            {
                if (value != this.startAddress)
                {
                    this.startAddress = value;
                    this.RaisePropertyChanged("StartAddress");
                }
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
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