using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Device.Location;

namespace TransitWP7
{
    public class TransitRequestContext : INotifyPropertyChanged
    {
        private GeoCoordinate userGeoCoordinate;
        private DateTime dateTime;
        private TimeCondition timeType;
        private TransitDescription selectedTransitTrip;
        private LocationDescription userCurrentLocation;
        private LocationDescription selectedStartingLocation;
        private LocationDescription selectedEndingLocation;

        public static TransitRequestContext Current = new TransitRequestContext();

        public event PropertyChangedEventHandler PropertyChanged;

        public GeoCoordinate UserGeoCoordinate
        {
            get
            {
                return this.userGeoCoordinate;
            }

            set
            {
                if (value != this.userGeoCoordinate)
                {
                    this.userGeoCoordinate = value;
                    this.RaisePropertyChanged("UserGeoCoordinate");
                }
            }
        }

        public DateTime DateTime
        {
            get
            {
                return this.dateTime;
            }

            set
            {
                if (value != this.dateTime)
                {
                    this.dateTime = value;
                    this.RaisePropertyChanged("DateTime");
                }
            }
        }
        
        public TimeCondition TimeType
        {
            get
            {
                return this.timeType;
            }

            set
            {
                if (value != this.timeType)
                {
                    this.timeType = value;
                    this.RaisePropertyChanged("TimeType");
                }
            }
        }

        public TransitDescription SelectedTransitTrip
        {
            get
            {
                return this.selectedTransitTrip;
            }

            set
            {
                if (value != this.selectedTransitTrip)
                {
                    this.selectedTransitTrip = value;
                    this.RaisePropertyChanged("SelectedTransitTrip");
                }
            }
        }

        public LocationDescription SelectedStartingLocation
        {
            get
            {
                return this.selectedStartingLocation;
            }

            set
            {
                if (value != this.selectedStartingLocation)
                {
                    this.selectedStartingLocation = value;
                    this.RaisePropertyChanged("SelectedStartingLocation");
                }
            }
        }

        public LocationDescription SelectedEndingLocation
        {
            get
            {
                return this.selectedEndingLocation;
            }

            set
            {
                if (value != this.selectedEndingLocation)
                {
                    this.selectedEndingLocation = value;
                    this.RaisePropertyChanged("SelectedEndingLocation");
                }
            }
        }

        public LocationDescription UserCurrentLocation
        {
            get
            {
                return this.userCurrentLocation;
            }

            set
            {
                if (value != this.userCurrentLocation)
                {
                    this.userCurrentLocation = value;
                    this.RaisePropertyChanged("UserCurrentLocation");
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