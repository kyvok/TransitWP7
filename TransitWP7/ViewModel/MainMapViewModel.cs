using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

namespace TransitWP7.ViewModel
{
    public class MainMapViewModel : ViewModelBase
    {
        private string _startLocationText;
        private bool _isStartLocationStale = true;
        private string _endLocationText;
        private bool _isEndLocationStale = true;

        public List<TransitDescription> TransitTrips;
        public ObservableCollection<TransitDescription> TransitDescriptionCollection = new ObservableCollection<TransitDescription>();

        public MainMapViewModel()
        {
            GeoLocation.Instance.GeoWatcher.PositionChanged += this.watcher_PositionChanged;

            Messenger.Default.Register<NotificationMessage<LocationDescription>>(
                this,
                MessengerToken.SelectedEndpoint,
                notificationMessage =>
                {
                    if (notificationMessage.Notification.Equals("start"))
                    {
                        this.UpdateLocation("start", notificationMessage.Content);
                    }
                    else
                    {
                        this.UpdateLocation("end", notificationMessage.Content);
                    }
                });

            this.Context.DateTime = DateTime.Now;
            this.Context.TimeType = TimeCondition.Now;
        }

        // Event handler for the GeoCoordinateWatcher.PositionChanged event.
        private void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            TransitRequestContext.Current.UserGeoCoordinate = e.Position.Location;
            ////this.mainMap.SetView(e.Position.Location, 15.0);
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
            get
            {
                return this._startLocationText;
            }

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
            get
            {
                return this._endLocationText;
            }

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
            this.EnsureDateTimeSyncInContext(datePart, timePart, this.Context.TimeType);
        }

        public void EnsureDateTimeSyncInContext(DateTime? datePart, DateTime? timePart, TimeCondition timeCondition)
        {
            this.Context.DateTime = new DateTime(
                datePart.Value.Year,
                datePart.Value.Month,
                datePart.Value.Day,
                timePart.Value.Hour,
                timePart.Value.Minute,
                timePart.Value.Second);

            this.Context.TimeType = timeCondition;
        }

        // TODO: When pressing back button from location selection page, the progress indicator stays on. Need better decoupling.
        public void TryResolveEndpoints()
        {
            // Notify calcul in progress
            Messenger.Default.Send(new NotificationMessage<bool>(true, "Resolving endpoints..."), MessengerToken.MainMapProgressIndicator);

            if (string.IsNullOrWhiteSpace(this.StartLocationText))
            {
                ProcessErrorMessage("Where are you starting from?");
                return;
            }

            if (string.IsNullOrWhiteSpace(this.EndLocationText))
            {
                ProcessErrorMessage("Where do you want to go?");
                return;
            }

            if (this._isStartLocationStale && Globals.MyCurrentLocationText.Equals(this.StartLocationText, StringComparison.OrdinalIgnoreCase))
            {
                this.UpdateLocation("start", new LocationDescription(this.Context.UserGeoCoordinate) { DisplayName = Globals.MyCurrentLocationText });
                return;
            }

            if (this._isEndLocationStale && Globals.MyCurrentLocationText.Equals(this.EndLocationText, StringComparison.OrdinalIgnoreCase))
            {
                this.UpdateLocation("end", new LocationDescription(this.Context.UserGeoCoordinate) { DisplayName = Globals.MyCurrentLocationText });
                return;
            }

            if (this._isStartLocationStale)
            {
                ProxyQuery.GetLocationsAndBusiness(
                    this.StartLocationText,
                    TransitRequestContext.Current.UserGeoCoordinate,
                    this.GetLocationsAndBusinessCallback,
                    "start");
                return;
            }

            if (this._isEndLocationStale)
            {
                ProxyQuery.GetLocationsAndBusiness(
                    this.EndLocationText,
                    TransitRequestContext.Current.UserGeoCoordinate,
                    this.GetLocationsAndBusinessCallback,
                    "end");
                return;
            }

            // Notify calcul in progress
            Messenger.Default.Send(new NotificationMessage<bool>(true, "Calculating transit trips..."), MessengerToken.MainMapProgressIndicator);

            // TODO: fix initial context state not set. Hacked up in view startup.
            ProxyQuery.GetTransitDirections(
                this.Context.SelectedStartingLocation.GeoCoordinate,
                this.Context.SelectedEndingLocation.GeoCoordinate,
                this.Context.DateTime,
                this.Context.TimeType,
                this.GetTransitDirectionsCallback,
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
                this.UpdateLocation(result.UserState as string, result.LocationDescriptions[0]);
            }
            else
            {
                Messenger.Default.Send(new NotificationMessage<List<LocationDescription>>(result.LocationDescriptions, result.UserState as string), MessengerToken.EndpointResolutionPopup);
                Messenger.Default.Send(new NotificationMessage(result.UserState as string), MessengerToken.EndpointResolutionPopup);
            }
        }

        private void UpdateLocation(string endpoint, LocationDescription location)
        {
            DispatcherHelper.UIDispatcher.BeginInvoke(
                    () =>
                    {
                        if (endpoint == "start")
                        {
                            this.StartLocationText = location.DisplayName;
                            this._isStartLocationStale = false;
                            this.Context.SelectedStartingLocation = location;
                        }
                        else
                        {
                            this.EndLocationText = location.DisplayName;
                            this._isEndLocationStale = false;
                            this.Context.SelectedEndingLocation = location;
                        }

                        TryResolveEndpoints();
                    });
        }

        private void GetTransitDirectionsCallback(ProxyQueryResult result)
        {
            // Notify progress bar calcul is done
            Messenger.Default.Send(new NotificationMessage<bool>(false, string.Empty), MessengerToken.MainMapProgressIndicator);

            if (result.Error != null)
            {
                ProcessErrorMessage(result.Error.Message);
                return;
            }

            this.TransitDescriptionCollection.Clear();
            foreach (var transit in result.TransitDescriptions)
            {
                this.TransitDescriptionCollection.Add(transit);
            }

            Messenger.Default.Send(new NotificationMessage(string.Empty), MessengerToken.TransitTripsReady);
        }

        private static void ProcessErrorMessage(string errorMsg)
        {
            var dialogMessage = new DialogMessage(errorMsg, null)
                         {
                             Caption = "Oups!",
                             Button = MessageBoxButton.OK
                         };
            Messenger.Default.Send(dialogMessage, MessengerToken.ErrorPopup);

            Messenger.Default.Send(new NotificationMessage<bool>(false, string.Empty), MessengerToken.MainMapProgressIndicator);
        }
    }
}