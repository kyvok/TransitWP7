//TODO: Copyright info

namespace TransitWP7
{
    using System;
    using System.Collections.Generic;
    using System.Device.Location;
    using TransitWP7.BingMapsRestApi;
    using TransitWP7.BingSearchRestApi;

    public class TransitDescription
    {
        public TransitDescription() { }

        public TransitDescription(Route route)
        {
            this.StartLocation = route.RouteLegs[0].ActualStart.AsGeoCoordinate();
            this.EndLocation = route.RouteLegs[0].ActualEnd.AsGeoCoordinate();
            this.TravelDuration = route.TravelDuration;
            this.ArrivalTime = route.RouteLegs[0].EndTime.ToShortTimeString();
            this.DepartureTime = route.RouteLegs[0].StartTime.ToShortTimeString();

            //TODO: need to add endpoint to the step list?
            this.ItinerarySteps = new List<ItineraryStep>();
            foreach (var topLeg in route.RouteLegs[0].ItineraryItems)
            {
                this.ItinerarySteps.Add(new ItineraryStep(topLeg));
            }

            this.PathPoints = new List<GeoCoordinate>();
            foreach (var pathPoint in route.RoutePaths[0].Line)
            {
                this.PathPoints.Add(pathPoint.AsGeoCoordinate());
            }

            this.MapView = route.BoundingBox.AsLocationRect();
        }

        public Microsoft.Phone.Controls.Maps.LocationRect MapView { get; set; }
        public IList<GeoCoordinate> PathPoints { get; set; }
        public GeoCoordinate StartLocation { get; set; }
        public GeoCoordinate EndLocation { get; set; }
        public double TravelDuration { get; set; }
        public IList<ItineraryStep> ItinerarySteps { get; set; }
        public string ArrivalTime { get; set; }
        public string DepartureTime { get; set; }
    }

    public class ItineraryStep
    {
        public ItineraryStep() { }

        public ItineraryStep(ItineraryItem item)
        {
            this.GeoCoordinate = item.ManeuverPoint.AsGeoCoordinate();
            this.Instruction = item.Instruction.Value;
            this.Hints = item.Hint;
            this.Time = item.Time;
            this.TravelMode = item.Detail.Mode != null ? item.Detail.Mode : "";
            this.BusNumber = item.TransitLine != null ? item.TransitLine.AbbreviatedName : "";
            this.IconType = item.IconType.ToString().StartsWith("N") ? "" : item.IconType.ToString();
            this.ChildItinerarySteps = new List<ItineraryStep>();
            if (item.ChildItineraryItems != null)
            {
                foreach (var itemStep in item.ChildItineraryItems)
                {
                    this.ChildItinerarySteps.Add(new ItineraryStep(itemStep));
                }
            }
        }

        public GeoCoordinate GeoCoordinate { get; set; }
        public IList<ItineraryStep> ChildItinerarySteps { get; set; }
        public string Instruction { get; set; }
        public string[] Hints { get; set; }
        public DateTime Time { get; set; }
        public string TravelMode { get; set; }
        public string BusNumber { get; set; }
        public string IconType { get; set; }
    }

    public class LocationDescription
    {
        public LocationDescription() { }

        public LocationDescription(Location result)
        {
            this.DisplayName = result.Name;
            this.GeoCoordinate = result.Point.AsGeoCoordinate();
            this.PostalCode = result.Address.PostalCode;
            this.Address = result.Address.FormattedAddress;
            this.Confidence = result.Confidence.ToString();
            this.StateOrProvince = result.Address.AdminDistrict;
        }

        public LocationDescription(PhonebookResult result)
        {
            this.DisplayName = result.Title;
            this.GeoCoordinate = new GeoCoordinate(result.Latitude, result.Longitude);
            this.PostalCode = result.PostalCode;
            this.Address = String.Format(
                        "{0} {1}, {2} {3}",
                        result.Address,
                        result.City,
                        result.StateOrProvince,
                        result.PostalCode);
            this.Confidence = "High";
            this.StateOrProvince = result.StateOrProvince;
        }

        public string DisplayName { get; set; }
        public GeoCoordinate GeoCoordinate { get; set; }
        public string PostalCode { get; set; }
        public string Address { get; set; }
        public string Confidence { get; set; }
        public string StateOrProvince { get; set; }
    }

    public enum TimeCondition
    {
        ArrivingAt,
        DepartingAt,
        LastArrivalTime,
        Now
    }
}
