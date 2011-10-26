// Copyright info

namespace TransitWP7
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Device.Location;
    using TransitWP7.BingMapsRestApi;

    public class ItineraryStep : INotifyPropertyChanged
    {
        private GeoCoordinate geoCoordinate;
        private ObservableCollection<ItineraryStep> childItinerarySteps;
        private string instruction;
        private ObservableCollection<string> hints;
        private DateTime time;
        private string travelMode;
        private string busNumber;
        private string iconType;

        public ItineraryStep() { }

        public ItineraryStep(ItineraryItem item)
        {
            this.GeoCoordinate = item.ManeuverPoint.AsGeoCoordinate();
            this.Instruction = item.Instruction.Value;
            this.Time = item.Time;
            this.TravelMode = item.Detail.Mode != null ? item.Detail.Mode : "";
            this.BusNumber = item.TransitLine != null ? item.TransitLine.AbbreviatedName : "";
            this.IconType = item.IconType.ToString().StartsWith("N") ? "" : item.IconType.ToString();

            this.hints = new ObservableCollection<string>();
            if (item.Hint != null)
            {
                foreach (var hint in item.Hint)
                {
                    this.hints.Add(hint);
                }
            }

            this.ChildItinerarySteps = new ObservableCollection<ItineraryStep>();
            if (item.ChildItineraryItems != null)
            {
                foreach (var itemStep in item.ChildItineraryItems)
                {
                    this.ChildItinerarySteps.Add(new ItineraryStep(itemStep));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        public string Instruction
        {
            get
            {
                return this.instruction;
            }
            set
            {
                if (value != this.instruction)
                {
                    this.instruction = value;
                    this.RaisePropertyChanged("Instruction");
                }
            }
        }

        public ObservableCollection<string> Hints
        {
            get
            {
                return this.hints;
            }
            set
            {
                if (value != this.hints)
                {
                    this.hints = value;
                    this.RaisePropertyChanged("Hints");
                }
            }
        }

        public DateTime Time
        {
            get
            {
                return this.time;
            }
            set
            {
                if (value != this.time)
                {
                    this.time = value;
                    this.RaisePropertyChanged("Time");
                }
            }
        }

        public string TravelMode
        {
            get
            {
                return this.travelMode;
            }
            set
            {
                if (value != this.travelMode)
                {
                    this.travelMode = value;
                    this.RaisePropertyChanged("TravelMode");
                }
            }
        }

        public string BusNumber
        {
            get
            {
                return this.busNumber;
            }
            set
            {
                if (value != this.busNumber)
                {
                    this.busNumber = value;
                    this.RaisePropertyChanged("BusNumber");
                }
            }
        }

        public string IconType
        {
            get
            {
                return this.iconType;
            }
            set
            {
                if (value != this.iconType)
                {
                    this.iconType = value;
                    this.RaisePropertyChanged("IconType");
                }
            }
        }

        public ObservableCollection<ItineraryStep> ChildItinerarySteps
        {
            get
            {
                return this.childItinerarySteps;
            }
            set
            {
                if (value != this.childItinerarySteps)
                {
                    this.childItinerarySteps = value;
                    this.RaisePropertyChanged("ChildItinerarySteps");
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
