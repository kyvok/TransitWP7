// Copyright info

namespace TransitWP7
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Device.Location;
    using Microsoft.Phone.Controls.Maps;
    using TransitWP7.BingMapsRestApi;

    public class TransitDescription : INotifyPropertyChanged
    {
        private GeoCoordinate startLocation;
        private GeoCoordinate endLocation;
        private LocationRect mapView;
        private double travelDuration;
        private string arrivalTime;
        private string departureTime;
        private ObservableCollection<ItineraryStep> itinerarySteps;
        private ObservableCollection<GeoCoordinate> pathPoints;

        public TransitDescription() { }

        public TransitDescription(Route route)
        {
            this.StartLocation = route.RouteLegs[0].ActualStart.AsGeoCoordinate();
            this.EndLocation = route.RouteLegs[0].ActualEnd.AsGeoCoordinate();
            this.TravelDuration = route.TravelDuration;
            this.ArrivalTime = route.RouteLegs[0].EndTime.ToShortTimeString();
            this.DepartureTime = route.RouteLegs[0].StartTime.ToShortTimeString();

            //TODO: need to add endpoint to the step list?
            this.ItinerarySteps = new ObservableCollection<ItineraryStep>();
            foreach (var topLeg in route.RouteLegs[0].ItineraryItems)
            {
                this.ItinerarySteps.Add(new ItineraryStep(topLeg));
            }

            this.PathPoints = new ObservableCollection<GeoCoordinate>();
            foreach (var pathPoint in route.RoutePaths[0].Line)
            {
                this.PathPoints.Add(pathPoint.AsGeoCoordinate());
            }

            this.MapView = route.BoundingBox.AsLocationRect();
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        public ObservableCollection<GeoCoordinate> PathPoints
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
