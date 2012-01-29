using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

namespace TransitWP7.ViewModel
{
    public class MainMapViewModel : ViewModelBase
    {
        private ObservableCollection<TransitDescription> transitDescriptionCollection = new ObservableCollection<TransitDescription>();
        private string _startLocationText;
        private bool _isStartLocationStale = true;
        private string _endLocationText;
        private bool _isEndLocationStale = true;
        private LocationDescription _selectedStartLocation;
        private LocationDescription _selectedEndLocation;
        private TransitDescription _selectedTransitDescription;
        private DateTime _dateTime;
        private TimeCondition _timeType;
        private GeoCoordinate _userGeoCoordinate;

        public MainMapViewModel()
        {
            GeoLocation.Instance.GeoWatcher.PositionChanged += this.Watcher_PositionChanged;

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

            this.DateTime = DateTime.Now;
            this.TimeType = TimeCondition.Now;
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

        public DateTime DateTime
        {
            get
            {
                return this._dateTime;
            }

            set
            {
                if (value != this._dateTime)
                {
                    this._dateTime = value;
                    this.RaisePropertyChanged("DateTime");
                }
            }
        }

        public TimeCondition TimeType
        {
            get
            {
                return this._timeType;
            }

            set
            {
                if (value != this._timeType)
                {
                    this._timeType = value;
                    this.RaisePropertyChanged("TimeType");
                }
            }
        }

        public TransitDescription SelectedTransitTrip
        {
            get
            {
                return this._selectedTransitDescription;
            }

            set
            {
                if (value != this._selectedTransitDescription)
                {
                    this._selectedTransitDescription = value;
                    this.RaisePropertyChanged("SelectedTransitTrip");
                }
            }
        }

        public GeoCoordinate UserGeoCoordinate
        {
            get
            {
                return this._userGeoCoordinate;
            }

            set
            {
                if (value != this._userGeoCoordinate)
                {
                    this._userGeoCoordinate = value;
                    this.RaisePropertyChanged("UserGeoCoordinate");
                }
            }
        }

        public ObservableCollection<TransitDescription> TransitDescriptionCollection
        {
            get
            {
                return this.transitDescriptionCollection;
            }

            set
            {
                this.transitDescriptionCollection = value;
            }
        }

        public void EnsureDateTimeSyncInContext(DateTime? datePart, DateTime? timePart)
        {
            this.EnsureDateTimeSyncInContext(datePart, timePart, this.TimeType);
        }

        public void EnsureDateTimeSyncInContext(DateTime? datePart, DateTime? timePart, TimeCondition timeCondition)
        {
            this.DateTime = new DateTime(
                datePart.Value.Year,
                datePart.Value.Month,
                datePart.Value.Day,
                timePart.Value.Hour,
                timePart.Value.Minute,
                timePart.Value.Second);

            this.TimeType = timeCondition;
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
                this.UpdateLocation("start", new LocationDescription(this.UserGeoCoordinate) { DisplayName = Globals.MyCurrentLocationText });
                return;
            }

            if (this._isEndLocationStale && Globals.MyCurrentLocationText.Equals(this.EndLocationText, StringComparison.OrdinalIgnoreCase))
            {
                this.UpdateLocation("end", new LocationDescription(this.UserGeoCoordinate) { DisplayName = Globals.MyCurrentLocationText });
                return;
            }

            if (this._isStartLocationStale)
            {
                ProxyQuery.GetLocationsAndBusiness(
                    this.StartLocationText,
                    this.UserGeoCoordinate,
                    this.GetLocationsAndBusinessCallback,
                    "start");
                return;
            }

            if (this._isEndLocationStale)
            {
                ProxyQuery.GetLocationsAndBusiness(
                    this.EndLocationText,
                    this.UserGeoCoordinate,
                    this.GetLocationsAndBusinessCallback,
                    "end");
                return;
            }

            // Notify calcul in progress
            Messenger.Default.Send(new NotificationMessage<bool>(true, "Calculating transit trips..."), MessengerToken.MainMapProgressIndicator);

            // TODO: fix initial context state not set. Hacked up in view startup.
            ProxyQuery.GetTransitDirections(
                this._selectedStartLocation.GeoCoordinate,
                this._selectedEndLocation.GeoCoordinate,
                this.DateTime,
                this.TimeType,
                this.GetTransitDirectionsCallback,
                null);
        }

        internal void StartOver()
        {
            this.StartLocationText = Globals.MyCurrentLocationText;
            this.EndLocationText = string.Empty;
            this._selectedStartLocation = null;
            this._selectedEndLocation = null;
            this.SelectedTransitTrip = null;
            this.TimeType = TimeCondition.Now;
            this.TransitDescriptionCollection.Clear();
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
                            this._selectedStartLocation = location;
                        }
                        else
                        {
                            this.EndLocationText = location.DisplayName;
                            this._isEndLocationStale = false;
                            this._selectedEndLocation = location;
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

            DispatcherHelper.UIDispatcher.BeginInvoke(
                () =>
                {
                    this.TransitDescriptionCollection.Clear();
                    foreach (var transit in result.TransitDescriptions)
                    {
                        this.TransitDescriptionCollection.Add(transit);
                    }
                });

            Messenger.Default.Send(new NotificationMessage(string.Empty), MessengerToken.TransitTripsReady);
        }

        // Event handler for the GeoCoordinateWatcher.PositionChanged event.
        private void Watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            this.UserGeoCoordinate = e.Position.Location;
            ////this.mainMap.SetView(e.Position.Location, 15.0);
        }
    }
}