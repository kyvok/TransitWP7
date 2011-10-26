//TODO: copyright info

namespace TransitWP7
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Device.Location;

    public class TransitRequestContext : INotifyPropertyChanged
    {
        private string endName;
        private string startName;
        private string endAddress;
        private string startAddress;
        private GeoCoordinate userLocation;
        private GeoCoordinate startLocation;
        private GeoCoordinate endLocation;
        private DateTime dateTime;
        private TimeCondition timeType;
        private TransitDescription selectedTransitTrip;
        private LocationDescription selectedStartingLocation;
        private LocationDescription selectedEndingLocation;

        public static TransitRequestContext Current = new TransitRequestContext();

        public event PropertyChangedEventHandler PropertyChanged;

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

        public GeoCoordinate UserLocation
        {
            get
            {
                return this.userLocation;
            }
            set
            {
                if (value != this.userLocation)
                {
                    this.userLocation = value;
                    this.RaisePropertyChanged("UserLocation");
                }
            }
        }

        public GeoCoordinate StartLocation
        {
            get
            {
                return this.startLocation;
            }
            set
            {
                if (value != this.startLocation)
                {
                    this.startLocation = value;
                    this.RaisePropertyChanged("StartLocation");
                }
            }
        }

        public GeoCoordinate EndLocation
        {
            get
            {
                return this.endLocation;
            }
            set
            {
                if (value != this.endLocation)
                {
                    this.endLocation = value;
                    this.RaisePropertyChanged("EndLocation");
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

        public ObservableCollection<TransitDescription> TransitDescriptionCollection = new ObservableCollection<TransitDescription>();
        public ObservableCollection<LocationDescription> StartingLocationDescriptionCollection = new ObservableCollection<LocationDescription>();
        public ObservableCollection<LocationDescription> EndingLocationDescriptionCollection = new ObservableCollection<LocationDescription>();

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}