using System.ComponentModel;
using System.Device.Location;
using System;
using System.Reactive.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using System.Collections.Generic;

namespace TransitWP7.ViewModels
{
    public class MainMapViewModel : ViewModelBase
    {
        //private WP7Contrib.Services.BingMaps.BingMapsService bingMapsService;
        private string _startLocationText;
        private bool _isStartLocationStale = true;
        private string _endLocationText;
        private bool _isEndLocationStale = true;

        public List<TransitDescription> TransitTrips; 

        public MainMapViewModel()
        {
            GeoLocation.Instance.GeoWatcher.PositionChanged += this.watcher_PositionChanged;

            Messenger.Default.Register<NotificationMessage<int>>(this,
                selectedIndex =>
                {
                    if (selectedIndex.Notification.Equals("start"))
                    {
                        DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                        {
                            this.StartLocationText = this.Context._possibleStartLocations[selectedIndex.Content].DisplayName;
                            this._isStartLocationStale = false;
                            this.Context.SelectedStartingLocation = this.Context._possibleStartLocations[selectedIndex.Content];
                        });
                    }
                    else
                    {
                        DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                        {
                            this.EndLocationText = this.Context._possibleEndLocations[selectedIndex.Content].DisplayName;
                            this._isEndLocationStale = false;
                            this.Context.SelectedEndingLocation = this.Context._possibleEndLocations[selectedIndex.Content];
                        });
                    }

                    TryResolveEndpoints();
                });

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
            //ProxyQuery.GetLocationAddress(TransitRequestContext.Current.UserGeoCoordinate, LocationCallback, null);
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

        public string StartLocationText
        {
            get { return this._startLocationText; }
            set
            {
                if (value != this._startLocationText)
                {
                    this._startLocationText = value.Trim();
                    this._isStartLocationStale = true;
                    this.RaisePropertyChanged("StartLocationText");
                }
            }
        }

        public string EndLocationText
        {
            get { return this._endLocationText; }
            set
            {
                if (value != this._endLocationText)
                {
                    this._endLocationText = value.Trim();
                    this._isEndLocationStale = true;
                    this.RaisePropertyChanged("EndLocationText");
                }
            }
        }

        public void EnsureDateTimeSyncInContext(DateTime? datePart, DateTime? timePart)
        {
            EnsureDateTimeSyncInContext(datePart, timePart, Context.TimeType);
        }

        public void EnsureDateTimeSyncInContext(DateTime? datePart, DateTime? timePart, TimeCondition timeCondition)
        {
            Context.DateTime = new DateTime(
                datePart.Value.Year,
                datePart.Value.Month,
                datePart.Value.Day,
                timePart.Value.Hour,
                timePart.Value.Minute,
                timePart.Value.Second
                );

            Context.TimeType = timeCondition;
        }

        public void TryResolveEndpoints()
        {
            if (this._isStartLocationStale && String.IsNullOrWhiteSpace(this.StartLocationText))
            {
                ProcessErrorMessage("Where are you starting from?");
                return;
            }

            if (this._isEndLocationStale && String.IsNullOrWhiteSpace(this.EndLocationText))
            {
                ProcessErrorMessage("Where do you want to go?");
                return;
            }

            if (Globals.MyCurrentLocationText.Equals(this.StartLocationText, StringComparison.OrdinalIgnoreCase))
            {
                DispatcherHelper.UIDispatcher.BeginInvoke(
                    () =>
                    {
                        this.StartLocationText = Globals.MyCurrentLocationText;
                        this._isStartLocationStale = false;
                        //TODO: location in context not set.
                    });
            }

            if (Globals.MyCurrentLocationText.Equals(this.EndLocationText, StringComparison.OrdinalIgnoreCase))
            {
                DispatcherHelper.UIDispatcher.BeginInvoke(
                    () =>
                    {
                        this.EndLocationText = Globals.MyCurrentLocationText;
                        this._isEndLocationStale = false;
                        //TODO: location in context not set.
                    });
            }

            if (this._isStartLocationStale)
            {
                ProxyQuery.GetLocationsAndBusiness(this.StartLocationText,
                                                   TransitRequestContext.Current.UserGeoCoordinate,
                                                   GetLocationsAndBusinessCallback, "start");
                return;
            }

            if (this._isEndLocationStale)
            {
                ProxyQuery.GetLocationsAndBusiness(this.EndLocationText,
                                                   TransitRequestContext.Current.UserGeoCoordinate,
                                                   GetLocationsAndBusinessCallback, "end");
                return;
            }

            if (this.Context.SelectedStartingLocation.GeoCoordinate.Equals(this.Context.SelectedEndingLocation.GeoCoordinate))
            {
                ProcessErrorMessage("You already are at destination!");
                return;
            }

            ProxyQuery.GetTransitDirections(
                this.Context.SelectedStartingLocation.GeoCoordinate,
                this.Context.SelectedEndingLocation.GeoCoordinate,
                this.Context.DateTime,
                this.Context.TimeType,
                GetTransitDirectionsCallback,
                null);
        }

        private void GetLocationsAndBusinessCallback(ProxyQueryResult result)
        {
            if (result.Error != null)
            {
                ProcessErrorMessage(result.Error.Message);
                return;
            }

            if (result.LocationDescriptions.Count == 1)
            {
                if (result.UserState.Equals("start"))
                {
                    DispatcherHelper.UIDispatcher.BeginInvoke(
                        () =>
                        {
                            this.StartLocationText = result.LocationDescriptions[0].DisplayName;
                            this._isStartLocationStale = false;
                            this.Context.SelectedStartingLocation = result.LocationDescriptions[0];
                        });
                }
                else
                {
                    DispatcherHelper.UIDispatcher.BeginInvoke(
                        () =>
                        {
                            this.EndLocationText = result.LocationDescriptions[0].DisplayName;
                            this._isEndLocationStale = false;
                            this.Context.SelectedEndingLocation = result.LocationDescriptions[0];
                        });
                }
            }
            else
            {
                if (result.UserState.Equals("start"))
                {
                    this.Context._possibleStartLocations = result.LocationDescriptions;
                }
                else
                {
                    this.Context._possibleEndLocations = result.LocationDescriptions;
                }

                var nm = new NotificationMessage(result.UserState as string);

                Messenger.Default.Send(nm);
            }
        }

        private void GetTransitDirectionsCallback(ProxyQueryResult result)
        {
            if (result.Error != null)
            {
                ProcessErrorMessage(result.Error.Message);
                return;
            }

            this.TransitTrips = result.TransitDescriptions;
            //now display the results.
        }

        private static void ProcessErrorMessage(string errorMsg)
        {
            var dm = new DialogMessage(errorMsg, null)
                         {
                             Caption = "Oups!",
                             Button = MessageBoxButton.OK
                         };
            Messenger.Default.Send(dm);
        }
    }
}