using System.ComponentModel;
using System.Device.Location;
using System;
using System.Reactive.Linq;
using System.Windows;

namespace TransitWP7.ViewModels
{
    public class MainMapViewModel : ViewModelBase
    {
        //private WP7Contrib.Services.BingMaps.BingMapsService bingMapsService;

        public MainMapViewModel()
        {
            GeoLocation.Instance.GeoWatcher.PositionChanged += this.watcher_PositionChanged;
            //this.bingMapsService = new WP7Contrib.Services.BingMaps.BingMapsService(
            //    new WP7Contrib.Communications.ResourceClientFactory(),
            //    new WP7Contrib.Communications.UrlEncoder(),
            //    new WP7Contrib.Services.BingMaps.Settings(ApiKeys.BingMapsKey, ApiKeys.BingSearchKey));
        }

        // Event handler for the GeoCoordinateWatcher.PositionChanged event.
        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            TransitRequestContext.Current.UserGeoCoordinate = e.Position.Location;

            //this.mainMap.SetView(e.Position.Location, 15.0);

            // Poll bing maps about the location
            ProxyQuery.GetLocationAddress(TransitRequestContext.Current.UserGeoCoordinate, LocationCallback, null);
            //bingMapsService.SearchForLocationUsingPoint(
            //    WP7Contrib.Services.BingMaps.CriterionFactory.CreateLocationSearchForPoint(e.Position.Location))
            //    .ObserveOnDispatcher()
            //    .Subscribe(result =>
            //                   {
            //                       TransitRequestContext.Current.UserCurrentLocation =
            //                           new LocationDescription(result.Locations[0]);
            //                   }
            //    );
        }

        private static void LocationCallback(ProxyQueryResult result)
        {
            if (result.Error != null)
            {
                MessageBox.Show(result.Error.Message, "LocationCallback obtained an error!", MessageBoxButton.OK);
            }
            else
            {
                TransitRequestContext.Current.UserCurrentLocation = result.LocationDescriptions[0];
            }
        }

        public TransitRequestContext Context
        {
            get
            {
                return TransitRequestContext.Current;
            }
        }

        public void EnsureDateTimeSyncInContext(DateTime? datePart, DateTime? timePart)
        {
            if(datePart.HasValue)


            Context.DateTime = new DateTime(
                datePart.Value.Year,
                datePart.Value.Month,
                datePart.Value.Day,
                timePart.Value.Hour,
                timePart.Value.Minute,
                timePart.Value.Second
                );
        }
    }
}