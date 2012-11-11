namespace Transitive2.Models
{
    using System;
    using System.Collections.ObjectModel;
    using System.Device.Location;
    using Microsoft.Phone.Controls.Maps;


    public static class GoogleMapsApiExtensions
    {
        //public static Route AsRoute(this GoogleApisLib.GoogleMapsApi.DirectionsRoute googleRoute)
        //{
        //    var route = new Route();
        //    route.OverviewPath = googleRoute.overview_path.AsLocationCollection();

        //    route.Directions = new ObservableCollection<Direction>();
        //    foreach (var leg in googleRoute.legs)
        //    {
        //        route.Directions.Add(leg.AsDirection());
        //    }

        //    return route;
        //}

        //public static Direction AsDirection(this GoogleApisLib.GoogleMapsApi.DirectionsLeg googleLeg)
        //{
        //    var direction = new Direction
        //        {
        //            Distance = googleLeg.distance.value,
        //            Duration = TimeSpan.FromSeconds(googleLeg.duration.value),
        //            StartLocation = googleLeg.start_location.AsGeoCoordinate(),
        //            EndLocation = googleLeg.end_location.AsGeoCoordinate(),
        //            StartAddress = googleLeg.start_address,
        //            EndAddress = googleLeg.end_address,
        //            ////StartTime = DateTime.Parse(googleLeg.departure_time.value),
        //            ////EndTime = DateTime.Parse(googleLeg.arrival_time.value)
        //        };

        //    if (googleLeg.steps != null)
        //    {
        //        direction.Steps = new ObservableCollection<DirectionStep>();
        //        foreach (var googleStep in googleLeg.steps)
        //        {
        //            direction.Steps.Add(googleStep.AsDirectionStep());
        //        }
        //    }

        //    return direction;
        //}

        //public static DirectionStep AsDirectionStep(this GoogleApisLib.GoogleMapsApi.DirectionsStep googleStep)
        //{
        //    var directionStep = new DirectionStep
        //        {
        //            Instructions = googleStep.instructions,
        //            Distance = googleStep.distance.value,
        //            Duration = TimeSpan.FromSeconds(googleStep.duration.value),
        //            StartLocation = googleStep.start_location.AsGeoCoordinate(),
        //            EndLocation = googleStep.end_location.AsGeoCoordinate(),
        //            Mode = googleStep.travel_mode.ToString(),
        //            OverviewPath = googleStep.path.AsLocationCollection()
        //        };

        //    if (googleStep.steps != null)
        //    {
        //        directionStep.Steps = new ObservableCollection<DirectionStep>();
        //        foreach (var innerStep in googleStep.steps)
        //        {
        //            directionStep.Steps.Add(innerStep.AsDirectionStep());
        //        }
        //    }

        //    return directionStep;
        //}

        public static LocationCollection AsLocationCollection(this GoogleApisLib.GoogleMapsApi.LatLng[] coordinates)
        {
            var collection = new LocationCollection();
            foreach (var coordinate in coordinates)
            {
                collection.Add(coordinate.AsGeoCoordinate());
            }

            return collection;
        }

        public static GeoCoordinate AsGeoCoordinate(this GoogleApisLib.GoogleMapsApi.LatLng coordinate)
        {
            return new GeoCoordinate
                {
                    Latitude = coordinate.lat,
                    Longitude = coordinate.lng,
                    Altitude = 0,
                    Speed = 0,
                    Course = 0,
                    HorizontalAccuracy = 0,
                    VerticalAccuracy = 0
                };
        }
    }
}