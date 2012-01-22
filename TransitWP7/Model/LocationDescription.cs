// Copyright info

namespace TransitWP7
{
    using System;
    using System.ComponentModel;
    using System.Device.Location;
    using BingApisLib.BingMapsRestApi;
    using BingApisLib.BingSearchRestApi;

    public class LocationDescription : INotifyPropertyChanged
    {
        private string displayName;
        private string formattedAddress;
        private string city;
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
            this.FormattedAddress = result.Address.FormattedAddress;
            this.City = result.Address.Locality;
            this.Confidence = result.Confidence.ToString();
            this.StateOrProvince = result.Address.AdminDistrict;
        }

        public LocationDescription(PhonebookResult result)
        {
            this.DisplayName = result.Title;
            this.GeoCoordinate = new GeoCoordinate(result.Latitude, result.Longitude);
            this.PostalCode = result.PostalCode;
            this.FormattedAddress = String.Format(
                        "{0}, {1}, {2}, {3}",
                        result.Address,
                        result.City,
                        result.StateOrProvince,
                        result.PostalCode);
            this.City = result.City;
            this.Confidence = "High";
            this.StateOrProvince = result.StateOrProvince;
        }

        public LocationDescription(GeoCoordinate coordinate)
        {
            this.GeoCoordinate = coordinate;
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

        public string FormattedAddress
        {
            get
            {
                return this.formattedAddress;
            }
            set
            {
                if (value != this.formattedAddress)
                {
                    this.formattedAddress = value;
                    this.RaisePropertyChanged("FormattedAddress");
                }
            }
        }

        public string City
        {
            get
            {
                return this.city;
            }
            set
            {
                if (value != this.city)
                {
                    this.city = value;
                    this.RaisePropertyChanged("City");
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
