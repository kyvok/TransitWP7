namespace TransitWP7.Model
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Device.Location;
    using BingApisLib.BingMapsRestApi;
    using Microsoft.Phone.Controls.Maps;

    public class TransitDescription : INotifyPropertyChanged
    {
        private DirectionType _transitType;
        private GeoCoordinate _startLocation;
        private GeoCoordinate _endLocation;
        private LocationRect _mapView;
        private double _travelDuration;
        private string _arrivalTime;
        private string _departureTime;
        private ObservableCollection<ItineraryStep> _itinerarySteps;
        private LocationCollection _pathPoints;

        public TransitDescription()
        {
        }

        public TransitDescription(Route route, DirectionType transitType)
        {
            if (route == null)
            {
                throw new ArgumentNullException("route");
            }

            this.TransitType = transitType;
            this.StartLocation = route.RouteLegs[0].ActualStart.AsGeoCoordinate();
            this.EndLocation = route.RouteLegs[0].ActualEnd.AsGeoCoordinate();
            this.TravelDuration = route.TravelDuration;
            this.ArrivalTime = route.RouteLegs[0].EndTime.ToShortTimeString();
            this.DepartureTime = route.RouteLegs[0].StartTime.ToShortTimeString();

            // TODO: need to add endpoint to the step list?
            this.ItinerarySteps = new ObservableCollection<ItineraryStep>();
            var stepNumber = 0;
            foreach (var topLeg in route.RouteLegs[0].ItineraryItems)
            {
                this.ItinerarySteps.Add(new ItineraryStep(topLeg, ++stepNumber));

                // calculate the walking start and end times
                if (this.ItinerarySteps[stepNumber - 1].IconType == "Walk")
                {
                    if (stepNumber == 1)
                    {
                        // WalkOnly directions needs to be set a default time. Using current time
                        this.ItinerarySteps[stepNumber - 1].StartTime = this.TransitType == DirectionType.WalkOnly ? DateTime.Now : route.RouteLegs[0].StartTime;
                    }
                    else
                    {
                        this.ItinerarySteps[stepNumber - 1].StartTime = this.ItinerarySteps[stepNumber - 2].EndTime;
                    }

                    this.ItinerarySteps[stepNumber - 1].EndTime = this.ItinerarySteps[stepNumber - 1].StartTime + TimeSpan.FromSeconds(topLeg.TravelDuration);
                }
            }

            this.ItinerarySteps[0].StepType = ItineraryStep.ItineraryStepType.FirstStep;
            this.ItinerarySteps[this.ItinerarySteps.Count - 1].StepType = ItineraryStep.ItineraryStepType.LastStep;

            this.PathPoints = new LocationCollection();
            foreach (var pathPoint in route.RoutePaths[0].Line)
            {
                this.PathPoints.Add(pathPoint.AsGeoCoordinate());
            }

            this.MapView = route.BoundingBox.AsLocationRect();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public enum DirectionType
        {
            Transit,
            WalkOnly
        }

        public DirectionType TransitType
        {
            get
            {
                return this._transitType;
            }

            set
            {
                if (value != this._transitType)
                {
                    this._transitType = value;
                    this.RaisePropertyChanged("TransitType");
                }
            }
        }

        public GeoCoordinate StartLocation
        {
            get
            {
                return this._startLocation;
            }

            set
            {
                if (value != this._startLocation)
                {
                    this._startLocation = value;
                    this.RaisePropertyChanged("StartLocation");
                }
            }
        }

        public GeoCoordinate EndLocation
        {
            get
            {
                return this._endLocation;
            }

            set
            {
                if (value != this._endLocation)
                {
                    this._endLocation = value;
                    this.RaisePropertyChanged("EndLocation");
                }
            }
        }

        public LocationRect MapView
        {
            get
            {
                return this._mapView;
            }

            set
            {
                if (value != this._mapView)
                {
                    this._mapView = value;
                    this.RaisePropertyChanged("MapView");
                }
            }
        }

        public double TravelDuration
        {
            get
            {
                return this._travelDuration;
            }

            set
            {
                if (value != this._travelDuration)
                {
                    this._travelDuration = value;
                    this.RaisePropertyChanged("TravelDuration");
                }
            }
        }

        public string ArrivalTime
        {
            get
            {
                return this._arrivalTime;
            }

            set
            {
                if (value != this._arrivalTime)
                {
                    this._arrivalTime = value;
                    this.RaisePropertyChanged("ArrivalTime");
                }
            }
        }

        public string DepartureTime
        {
            get
            {
                return this._departureTime;
            }

            set
            {
                if (value != this._departureTime)
                {
                    this._departureTime = value;
                    this.RaisePropertyChanged("DepartureTime");
                }
            }
        }

        public ObservableCollection<ItineraryStep> ItinerarySteps
        {
            get
            {
                return this._itinerarySteps;
            }

            set
            {
                if (value != this._itinerarySteps)
                {
                    this._itinerarySteps = value;
                    this.RaisePropertyChanged("ItinerarySteps");
                }
            }
        }

        public LocationCollection PathPoints
        {
            get
            {
                return this._pathPoints;
            }

            set
            {
                if (value != this._pathPoints)
                {
                    this._pathPoints = value;
                    this.RaisePropertyChanged("PathPoints");
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
