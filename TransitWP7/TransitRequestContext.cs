﻿//TODO: copyright info

namespace TransitWP7
{
    using System;
    using System.Collections.ObjectModel;
    using System.Device.Location;

    public class TransitRequestContext
    {
        public static TransitRequestContext Current = new TransitRequestContext();

        public string EndName { get; set; }
        public string StartName { get; set; }
        public string EndAddress { get; set; }
        public string StartAddress { get; set; }
        public GeoCoordinate UserLocation { get; set; }
        public GeoCoordinate StartLocation { get; set; }
        public GeoCoordinate EndLocation { get; set; }
        public DateTime DateTime { get; set; }
        public TimeCondition TimeType { get; set; }

        public TransitDescription SelectedTransitTrip { get; set; }
        public LocationDescription SelectedStartingLocation { get; set; }
        public LocationDescription SelectedEndingLocation { get; set; }

        public ObservableCollection<TransitDescription> TransitDescriptionCollection = new ObservableCollection<TransitDescription>();
        public ObservableCollection<LocationDescription> StartingLocationDescriptionCollection = new ObservableCollection<LocationDescription>();
        public ObservableCollection<LocationDescription> EndingLocationDescriptionCollection = new ObservableCollection<LocationDescription>();
    }
}