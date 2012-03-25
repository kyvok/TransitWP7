// Copyright info

namespace TransitWP7
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Device.Location;
    using BingApisLib.BingMapsRestApi;

    public class ItineraryStep : INotifyPropertyChanged
    {
        private GeoCoordinate geoCoordinate;
        private ObservableCollection<ItineraryStep> childItinerarySteps;
        private string instruction;
        private ObservableCollection<string> hints;
        private DateTime starttime;
        private DateTime endtime;
        private string travelMode;
        private string busNumber;
        private string iconType;
        private int stepNumber;
        private ItineraryStepType stepType;

        public ItineraryStep()
        {
        }

        public ItineraryStep(ItineraryItem item, int stepNumber)
        {
            this.GeoCoordinate = item.ManeuverPoint.AsGeoCoordinate();
            this.Instruction = item.Instruction.Value;
            this.TravelMode = item.Detail.Mode != null ? item.Detail.Mode : string.Empty;
            this.BusNumber = item.TransitLine != null ? item.TransitLine.AbbreviatedName : string.Empty;
            this.IconType = item.IconType.ToString().StartsWith("N") ? string.Empty : item.IconType.ToString();
            this.StartTime = item.Time;
            this.EndTime = item.Time;

            this.hints = new ObservableCollection<string>();
            if (item.Hint != null)
            {
                foreach (var hint in item.Hint)
                {
                    this.hints.Add(hint);
                }
            }

            this.stepNumber = stepNumber;

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

        public DateTime StartTime
        {
            get
            {
                return this.starttime;
            }

            set
            {
                if (value != this.starttime)
                {
                    this.starttime = value;
                    this.RaisePropertyChanged("StartTime");
                }
            }
        }

        public DateTime EndTime
        {
            get
            {
                return this.endtime;
            }

            set
            {
                if (value != this.endtime)
                {
                    this.endtime = value;
                    this.RaisePropertyChanged("EndTime");
                }
            }
        }

        public TimeSpan TravelDuration
        {
            get
            {
                if (this.EndTime != null && this.StartTime != null)
                {
                    return this.StartTime - this.EndTime;
                }
                else
                {
                    return TimeSpan.FromSeconds(0);
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

        public int StepNumber
        {
            get
            {
                return this.stepNumber;
            }

            set
            {
                if (value != this.stepNumber)
                {
                    this.stepNumber = value;
                    this.RaisePropertyChanged("StepNumber");
                }
            }
        }

        public ItineraryStepType StepType
        {
            get
            {
                return this.stepType;
            }

            set
            {
                if (value != this.stepType)
                {
                    this.stepType = value;
                    this.RaisePropertyChanged("StepType");
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
