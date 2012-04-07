namespace TransitWP7.Model
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Device.Location;
    using System.Globalization;

    using BingApisLib.BingMapsRestApi;

    public class ItineraryStep : INotifyPropertyChanged
    {
        private GeoCoordinate _geoCoordinate;
        private ObservableCollection<ItineraryStep> _childItinerarySteps;
        private string _instruction;
        private ObservableCollection<string> _hints;
        private DateTime _starttime;
        private DateTime _endtime;
        private string _travelMode;
        private string _busNumber;
        private string _iconType;
        private int _stepNumber;
        private ItineraryStepType _stepType;

        public ItineraryStep()
        {
        }

        public ItineraryStep(ItineraryItem item, int stepNumber)
        {
            this.GeoCoordinate = item.ManeuverPoint.AsGeoCoordinate();
            this.Instruction = item.Instruction.Value;
            this.TravelMode = item.Detail.Mode ?? string.Empty;
            this.BusNumber = item.TransitLine != null ? item.TransitLine.AbbreviatedName : string.Empty;
            this.IconType = item.IconType.ToString().StartsWith("N", StringComparison.Ordinal) ? string.Empty : item.IconType.ToString();
            this.StartTime = item.Time;
            this.EndTime = item.Time;

            this._hints = new ObservableCollection<string>();
            if (item.Hint != null)
            {
                foreach (var hint in item.Hint)
                {
                    this._hints.Add(hint);
                }
            }

            this._stepNumber = stepNumber;

            this.ChildItinerarySteps = new ObservableCollection<ItineraryStep>();
            if (item.ChildItineraryItems != null)
            {
                this.StartTime = item.ChildItineraryItems[0].Time;
                this.EndTime = item.ChildItineraryItems[item.ChildItineraryItems.Length - 1].Time;
                foreach (var itemStep in item.ChildItineraryItems)
                {
                    this.ChildItinerarySteps.Add(new ItineraryStep(itemStep, 0));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public enum ItineraryStepType
        {
            MiddleStep,
            FirstStep,
            LastStep
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

        public string Instruction
        {
            get
            {
                return this._instruction;
            }

            set
            {
                if (value != this._instruction)
                {
                    this._instruction = value;
                    this.RaisePropertyChanged("Instruction");
                }
            }
        }

        public ObservableCollection<string> Hints
        {
            get
            {
                return this._hints;
            }

            set
            {
                if (value != this._hints)
                {
                    this._hints = value;
                    this.RaisePropertyChanged("Hints");
                }
            }
        }

        public DateTime StartTime
        {
            get
            {
                return this._starttime;
            }

            set
            {
                if (value != this._starttime)
                {
                    this._starttime = value;
                    this.RaisePropertyChanged("StartTime");
                }
            }
        }

        public DateTime EndTime
        {
            get
            {
                return this._endtime;
            }

            set
            {
                if (value != this._endtime)
                {
                    this._endtime = value;
                    this.RaisePropertyChanged("EndTime");
                }
            }
        }

        public TimeSpan TravelDuration
        {
            get
            {
                return this.StartTime - this.EndTime;
            }
        }

        public string TravelMode
        {
            get
            {
                return this._travelMode;
            }

            set
            {
                if (value != this._travelMode)
                {
                    this._travelMode = value;
                    this.RaisePropertyChanged("TravelMode");
                }
            }
        }

        public string BusNumber
        {
            get
            {
                return this._busNumber;
            }

            set
            {
                if (value != this._busNumber)
                {
                    this._busNumber = value;
                    this.RaisePropertyChanged("BusNumber");
                }
            }
        }

        public string IconType
        {
            get
            {
                return this._iconType;
            }

            set
            {
                if (value != this._iconType)
                {
                    this._iconType = value;
                    this.RaisePropertyChanged("IconType");
                }
            }
        }

        public int StepNumber
        {
            get
            {
                return this._stepNumber;
            }

            set
            {
                if (value != this._stepNumber)
                {
                    this._stepNumber = value;
                    this.RaisePropertyChanged("StepNumber");
                }
            }
        }

        public ItineraryStepType StepType
        {
            get
            {
                return this._stepType;
            }

            set
            {
                if (value != this._stepType)
                {
                    this._stepType = value;
                    this.RaisePropertyChanged("StepType");
                }
            }
        }

        public ObservableCollection<ItineraryStep> ChildItinerarySteps
        {
            get
            {
                return this._childItinerarySteps;
            }

            set
            {
                if (value != this._childItinerarySteps)
                {
                    this._childItinerarySteps = value;
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
