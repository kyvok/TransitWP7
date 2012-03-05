using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Net.NetworkInformation;

namespace TransitWP7.ViewModel
{
    public class MainMapViewModel : ViewModelBase
    {
        private readonly GeoCoordinateWatcher _geoCoordinateWatcher;
        private ObservableCollection<TransitDescription> _transitDescriptionCollection = new ObservableCollection<TransitDescription>();
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
        private GeoCoordinate _centerMapGeoCoordinate;

        public MainMapViewModel()
        {
            this._geoCoordinateWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default) { MovementThreshold = 20 };
            this._geoCoordinateWatcher.PositionChanged += this.GeoCoordinateWatcher_PositionChanged;
            this._geoCoordinateWatcher.StatusChanged += this.GeoCoordinateWatcher_StatusChanged;
            this._geoCoordinateWatcher.Start();

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

            DeviceNetworkInformation.NetworkAvailabilityChanged += this.DeviceNetworkInformation_NetworkAvailabilityChanged;

#if DEBUG
            if (IsInDesignModeStatic)
            {
                this.StartLocationText = "Example Start Location";
                this.EndLocationText = "Example End Location";
                this.TransitDescriptionCollection = ViewModelLocator.TransitDescriptionsTestValues;
                this.SelectedTransitTrip = ViewModelLocator.TransitDescriptionsTestValues[0];
                this.UserGeoCoordinate = new GeoCoordinate(47.603, -122.329);
            }
#endif
        }

        public enum UIViewState
        {
            OnlyStartEndInputsView,
            MapViewOnly,
            TransitOptionsView,
            ItineraryView,
        }

        public UIViewState CurrentViewState { get; set; }

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
                    this._startLocationText = value;
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
                    this._endLocationText = value;
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

                if (this._timeType == TimeCondition.Now)
                {
                    this.DateTime = DateTime.Now;
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

        public LocationDescription SelectedStartLocation
        {
            get
            {
                return this._selectedStartLocation;
            }

            set
            {
                if (value != this._selectedStartLocation)
                {
                    this._selectedStartLocation = value;
                    this.RaisePropertyChanged("SelectedStartLocation");
                }
            }
        }

        public LocationDescription SelectedEndLocation
        {
            get
            {
                return this._selectedEndLocation;
            }

            set
            {
                if (value != this._selectedEndLocation)
                {
                    this._selectedEndLocation = value;
                    this.RaisePropertyChanged("SelectedEndLocation");
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

        public GeoCoordinate CenterMapGeoCoordinate
        {
            get
            {
                return this._centerMapGeoCoordinate;
            }

            set
            {
                if (value != this._centerMapGeoCoordinate)
                {
                    this._centerMapGeoCoordinate = value;
                    this.RaisePropertyChanged("CenterMapGeoCoordinate");
                }
            }
        }

        public ObservableCollection<TransitDescription> TransitDescriptionCollection
        {
            get
            {
                return this._transitDescriptionCollection;
            }

            set
            {
                this._transitDescriptionCollection = value;
            }
        }

        public void DoServiceChecks()
        {
            CheckNetwork();
            CheckLocationService(this._geoCoordinateWatcher.Status);
        }

        public override void Cleanup()
        {
            if (this._geoCoordinateWatcher != null)
            {
                this._geoCoordinateWatcher.Dispose();
            }

            base.Cleanup();
        }

        public void EnsureDateTimeSyncInContext(TimeCondition timeType)
        {
            this.TimeType = timeType;
            if (this.TimeType == TimeCondition.Now)
            {
                this.DateTime = DateTime.Now;
            }
        }

        public void TryResolveEndpoints()
        {
            // This is a new search, clear old transit info.
            this.TransitDescriptionCollection.Clear();
            this.SelectedTransitTrip = null;

            // Notify calcul in progress
            Messenger.Default.Send(new NotificationMessage<bool>(true, "Resolving endpoints..."), MessengerToken.MainMapProgressIndicator);
            Messenger.Default.Send(new NotificationMessage<bool>(false, "Locking UI"), MessengerToken.LockUiIndicator);

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

            // Trim whitespace from the start/end locations
            this.StartLocationText = this.StartLocationText.Trim();
            this.EndLocationText = this.EndLocationText.Trim();

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
            Messenger.Default.Send(new NotificationMessage<bool>(true, "Searching transit trips..."), MessengerToken.MainMapProgressIndicator);

            // TODO: fix initial context state not set. Hacked up in view startup.
            ProxyQuery.GetTransitDirections(
                this._selectedStartLocation.GeoCoordinate,
                this._selectedEndLocation.GeoCoordinate,
                this.DateTime,
                this.TimeType,
                this.GetTransitDirectionsCallback,
                null);
        }

        public void SetSelectedTransitTrip(int index)
        {
            this.SelectedTransitTrip = index >= 0 ? this.TransitDescriptionCollection[index] : null;
            var notificationMessage = new NotificationMessage<TransitDescription>(
                this,
                this.SelectedTransitTrip,
                string.Empty);
            Messenger.Default.Send(notificationMessage, MessengerToken.SelectedTransitTrip);
        }

        internal void StartOver()
        {
            this.StartLocationText = Globals.MyCurrentLocationText;
            this._isStartLocationStale = true;
            this.EndLocationText = string.Empty;
            this._isEndLocationStale = true;
            this.SelectedStartLocation = null;
            this.SelectedEndLocation = null;
            this.SelectedTransitTrip = null;
            this.TimeType = TimeCondition.Now;
            this.DateTime = DateTime.Now;
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
            Messenger.Default.Send(new NotificationMessage<bool>(true, "Unlocking UI"), MessengerToken.LockUiIndicator);
        }

        private static void CheckNetwork()
        {
            if (!DeviceNetworkInformation.IsNetworkAvailable)
            {
                ProcessErrorMessage("No network is available. Internet connection is required for calculating new transits.");
            }
        }

        private static void CheckLocationService(GeoPositionStatus status)
        {
            switch (status)
            {
                case GeoPositionStatus.Disabled:
                    ProcessErrorMessage("Location service is disabled. Some features won't work well.");
                    break;
                case GeoPositionStatus.NoData:
                case GeoPositionStatus.Initializing:
                    Messenger.Default.Send(new NotificationMessage<bool>(true, "Acquiring your current location..."), MessengerToken.MainMapProgressIndicator);
                    break;
                case GeoPositionStatus.Ready:
                    Messenger.Default.Send(new NotificationMessage<bool>(false, string.Empty), MessengerToken.MainMapProgressIndicator);
                    break;
                default:
                    break;
            }
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
                            this.SelectedStartLocation = location;
                        }
                        else
                        {
                            this.EndLocationText = location.DisplayName;
                            this._isEndLocationStale = false;
                            this.SelectedEndLocation = location;
                        }

                        TryResolveEndpoints();
                    });
        }

        private void GetTransitDirectionsCallback(ProxyQueryResult result)
        {
            // Notify progress bar calcul is done
            Messenger.Default.Send(new NotificationMessage<bool>(false, string.Empty), MessengerToken.MainMapProgressIndicator);
            Messenger.Default.Send(new NotificationMessage<bool>(true, "Unlocking UI"), MessengerToken.LockUiIndicator);

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

        private void DeviceNetworkInformation_NetworkAvailabilityChanged(object sender, NetworkNotificationEventArgs e)
        {
            CheckNetwork();
        }

        private void GeoCoordinateWatcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            this.UserGeoCoordinate = e.Position.Location;
        }

        private void GeoCoordinateWatcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            CheckLocationService(e.Status);
        }
    }
}