// Copyright info

namespace TransitWP7
{
    using System;
    using System.ComponentModel;
    using System.Device.Location;
    using TransitWP7.BingMapsRestApi;
    using TransitWP7.BingSearchRestApi;

    public class LocationDescription : INotifyPropertyChanged
    {
        private string displayName;
        private string address;
        private string stateOrProvince;
        private string postalCode;
        private string confidence;
        private GeoCoordinate geoCoordinate;

        public LocationDescription() { }

        public LocationDescription(Location result)
        {
            this.DisplayName = result.Name;
            this.GeoCoordinate = result.Point.AsGeoCoordinate();
            this.PostalCode = result.Address.PostalCode;
            this.Address = result.Address.FormattedAddress;
            this.Confidence = result.Confidence.ToString();
            this.StateOrProvince = result.Address.AdminDistrict;
        }

        public LocationDescription(PhonebookResult result)
        {
            this.DisplayName = result.Title;
            this.GeoCoordinate = new GeoCoordinate(result.Latitude, result.Longitude);
            this.PostalCode = result.PostalCode;
            this.Address = String.Format(
                        "{0} {1}, {2} {3}",
                        result.Address,
                        result.City,
                        result.StateOrProvince,
                        result.PostalCode);
            this.Confidence = "High";
            this.StateOrProvince = result.StateOrProvince;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string DisplayName
        {
            get
            {
                return this.displayName;
            }
            set
            {
                if (value != this.displayName)
                {
                    this.displayName = value;
                    this.RaisePropertyChanged("DisplayName");
                }
            }
        }

        public string Address
        {
            get
            {
                return this.address;
            }
            set
            {
                if (value != this.address)
                {
                    this.address = value;
                    this.RaisePropertyChanged("Address");
                }
            }
        }

        public string StateOrProvince
        {
            get
            {
                return this.stateOrProvince;
            }
            set
            {
                if (value != this.stateOrProvince)
                {
                    this.stateOrProvince = value;
                    this.RaisePropertyChanged("StateOrProvince");
                }
            }
        }

        public string PostalCode
        {
            get
            {
                return this.postalCode;
            }
            set
            {
                if (value != this.postalCode)
                {
                    this.postalCode = value;
                    this.RaisePropertyChanged("PostalCode");
                }
            }
        }

        public string Confidence
        {
            get
            {
                return this.confidence;
            }
            set
            {
                if (value != this.confidence)
                {
                    this.confidence = value;
                    this.RaisePropertyChanged("Confidence");
                }
            }
        }

        public GeoCoordinate GeoCoordinate
        {
            get
            {
                return this.geoCoordinate;
            }
            set
            {
                if (value != this.geoCoordinate)
                {
                    this.geoCoordinate = value;
                    this.RaisePropertyChanged("GeoCoordinate");
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
}
