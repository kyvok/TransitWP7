// Copyright info

namespace TransitWP7
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Device.Location;
    using BingApisLib.BingMapsRestApi;
    using Microsoft.Phone.Controls.Maps;

    public class TransitDescription : INotifyPropertyChanged
    {
        private DirectionType transitType;
        private GeoCoordinate startLocation;
        private GeoCoordinate endLocation;
        private LocationRect mapView;
        private double travelDuration;
        private string arrivalTime;
        private string departureTime;
        private ObservableCollection<ItineraryStep> itinerarySteps;
        private LocationCollection pathPoints;

        public TransitDescription()
        {
        }

        public TransitDescription(Route route, DirectionType transitType)
        {
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
                return this.transitType;
            }

            set
            {
                if (value != this.transitType)
                {
                    this.transitType = value;
                    this.RaisePropertyChanged("TransitType");
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

        public LocationRect MapView
        {
            get
            {
                return this.mapView;
            }

            set
            {
                if (value != this.mapView)
                {
                    this.mapView = value;
                    this.RaisePropertyChanged("MapView");
                }
            }
        }

        public double TravelDuration
        {
            get
            {
                return this.travelDuration;
            }

            set
            {
                if (value != this.travelDuration)
                {
                    this.travelDuration = value;
                    this.RaisePropertyChanged("TravelDuration");
                }
            }
        }

        public string ArrivalTime
        {
            get
            {
                return this.arrivalTime;
            }

            set
            {
                if (value != this.arrivalTime)
                {
                    this.arrivalTime = value;
                    this.RaisePropertyChanged("ArrivalTime");
                }
            }
        }

        public string DepartureTime
        {
            get
            {
                return this.departureTime;
            }

            set
            {
                if (value != this.departureTime)
                {
                    this.departureTime = value;
                    this.RaisePropertyChanged("DepartureTime");
                }
            }
        }

        public ObservableCollection<ItineraryStep> ItinerarySteps
        {
            get
            {
                return this.itinerarySteps;
            }

            set
            {
                if (value != this.itinerarySteps)
                {
                    this.itinerarySteps = value;
                    this.RaisePropertyChanged("ItinerarySteps");
                }
            }
        }

        public LocationCollection PathPoints
        {
            get
            {
                return this.pathPoints;
            }

            set
            {
                if (value != this.pathPoints)
                {
                    this.pathPoints = value;
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
