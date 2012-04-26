namespace TransitWP7.Model
{
    using System;
    using System.ComponentModel;
    using System.Device.Location;
    using System.Globalization;

    using BingApisLib.BingMapsRestApi;
    using BingApisLib.BingSearchRestApi;

    public class LocationDescription : INotifyPropertyChanged
    {
        private string _displayName;
        private string _formattedAddress;
        private string _city;
        private string _stateOrProvince;
        private string _postalCode;
        private string _confidence;
        private GeoCoordinate _geoCoordinate;

        public LocationDescription()
        {
        }

        public LocationDescription(Location result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

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
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            this.DisplayName = result.Title;
            this.GeoCoordinate = new GeoCoordinate(result.Latitude, result.Longitude)
            {
                // Setting these value to zero to improve deserialization perf.
                Altitude = 0,
                Course = 0,
                HorizontalAccuracy = 0,
                VerticalAccuracy = 0,
                Speed = 0
            };
            this.PostalCode = result.PostalCode;
            this.FormattedAddress = string.Format(
                        CultureInfo.InvariantCulture,
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
                return this._displayName;
            }

            set
            {
                if (value != this._displayName)
                {
                    this._displayName = value;
                    this.RaisePropertyChanged("DisplayName");
                }
            }
        }

        public string FormattedAddress
        {
            get
            {
                return this._formattedAddress;
            }

            set
            {
                if (value != this._formattedAddress)
                {
                    this._formattedAddress = value;
                    this.RaisePropertyChanged("FormattedAddress");
                }
            }
        }

        public string City
        {
            get
            {
                return this._city;
            }

            set
            {
                if (value != this._city)
                {
                    this._city = value;
                    this.RaisePropertyChanged("City");
                }
            }
        }

        public string StateOrProvince
        {
            get
            {
                return this._stateOrProvince;
            }

            set
            {
                if (value != this._stateOrProvince)
                {
                    this._stateOrProvince = value;
                    this.RaisePropertyChanged("StateOrProvince");
                }
            }
        }

        public string PostalCode
        {
            get
            {
                return this._postalCode;
            }

            set
            {
                if (value != this._postalCode)
                {
                    this._postalCode = value;
                    this.RaisePropertyChanged("PostalCode");
                }
            }
        }

        public string Confidence
        {
            get
            {
                return this._confidence;
            }

            set
            {
                if (value != this._confidence)
                {
                    this._confidence = value;
                    this.RaisePropertyChanged("Confidence");
                }
            }
        }

        public GeoCoordinate GeoCoordinate
        {
            get
            {
                return this._geoCoordinate;
            }

            set
            {
                if (value != this._geoCoordinate)
                {
                    this._geoCoordinate = value;
                    this.RaisePropertyChanged("GeoCoordinate");
                }
            }
        }

        /// <summary>
        /// Overriding ToString to show the DisplayName, this is to behave nicely with the AutoCompleteBox
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.DisplayName;
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
