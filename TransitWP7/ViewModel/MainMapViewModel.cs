﻿namespace TransitWP7.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Device.Location;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Messaging;
    using GalaSoft.MvvmLight.Threading;
    using Microsoft.Phone.Net.NetworkInformation;
    using TransitWP7.Model;
    using TransitWP7.Resources;

    public class MainMapViewModel : ViewModelBase
    {
        private GeoCoordinateWatcher _geoCoordinateWatcher;
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
        private bool _centerMapGeoSet;
        private double _zoomLevel;
        private int _pixelWidth;

        public MainMapViewModel()
        {
            Messenger.Default.Register<NotificationMessage<LocationDescription>>(
                this,
                MessengerToken.SelectedEndpoint,
                notificationMessage => this.UpdateLocation(
                    notificationMessage.Notification.Equals("start") ? "start" : "end", notificationMessage.Content));

            this.DateTime = DateTime.Now;
            this.TimeType = TimeCondition.Now;

            DeviceNetworkInformation.NetworkAvailabilityChanged += this.DeviceNetworkInformationNetworkAvailabilityChanged;

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

        public GeoCoordinateWatcher GeoCoordinateWatcher
        {
            get
            {
                if (this._geoCoordinateWatcher == null)
                {
                    this.InitializeGeoCoordinateWatcher();
                }

                return this._geoCoordinateWatcher;
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
                    if (value != null && value.GeoCoordinate != null)
                    {
                        this._isStartLocationStale = false;
                    }

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
                    if (value != null && value.GeoCoordinate != null)
                    {
                        this._isEndLocationStale = false;
                    }

                    this.RaisePropertyChanged("SelectedEndLocation");
                }
            }
        }

        public GeoCoordinate UserGeoCoordinate
        {
            get
            {
                if (ViewModelLocator.SettingsViewModelStatic.UseLocationSetting)
                {
                    return this._userGeoCoordinate;
                }

                // We don't know where the user is at all
                return null;
            }

            set
            {
                if (value != this._userGeoCoordinate)
                {
                    this._userGeoCoordinate = value;

                    if (value != null)
                    {
                        this.UpdatePixelWidth();
                    }

                    this.RaisePropertyChanged("UserGeoCoordinate");
                }
            }
        }

        public double MapZoomLevel
        {
            get
            {
                return this._zoomLevel;
            }

            set
            {
                if (!this._zoomLevel.Equals(value))
                {
                    this._zoomLevel = value;
                    this.UpdatePixelWidth();
                    this.RaisePropertyChanged("MapZoomLevel");
                }
            }
        }

        public int PixelWidth
        {
            get
            {
                return this._pixelWidth;
            }

            set
            {
                if (value != this._pixelWidth)
                {
                    this._pixelWidth = value;
                    this.RaisePropertyChanged("PixelWidth");
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

        public bool CenterMapGeoSet
        {
            get
            {
                return this._centerMapGeoSet;
            }

            set
            {
                if (value != this._centerMapGeoSet)
                {
                    this._centerMapGeoSet = value;
                    this.RaisePropertyChanged("CenterMapGeoSet");
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

        public ObservableCollection<LocationDescription> AutoCompleteData
        {
            get
            {
                return AutoCompleteDataManager.AutoCompleteData;
            }
        }

        public void DoServiceChecks()
        {
            CheckNetwork();
            if (ViewModelLocator.SettingsViewModelStatic.UseLocationSetting)
            {
                CheckLocationService(this.GeoCoordinateWatcher.Status);
            }
            else
            {
                // disable the locate me button
                Messenger.Default.Send(new NotificationMessage<bool>(false, string.Empty), MessengerToken.EnableLocationButtonIndicator);
            }
        }

        public void InitializeGeoCoordinateWatcher()
        {
            if (ViewModelLocator.SettingsViewModelStatic.UseLocationSetting)
            {
                if (this._geoCoordinateWatcher == null)
                {
                    this._geoCoordinateWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High) { MovementThreshold = 0 };
                    this._geoCoordinateWatcher.PositionChanged += this.GeoCoordinateWatcherPositionChanged;
                    this._geoCoordinateWatcher.StatusChanged += this.GeoCoordinateWatcherStatusChanged;
                    this._geoCoordinateWatcher.Start();
                }
            }
            else
            {
                this.DisposeGeoCoordinateWatcher();
            }
        }

        public void DisposeGeoCoordinateWatcher()
        {
            if (this._geoCoordinateWatcher != null)
            {
                this._geoCoordinateWatcher.Stop();
                this._geoCoordinateWatcher.Dispose();
                this._geoCoordinateWatcher = null;
            }
        }

        public override void Cleanup()
        {
            this.DisposeGeoCoordinateWatcher();
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

        public void BeginCalculateTransit()
        {
            // This is a new search, clear old transit info.
            this.TransitDescriptionCollection.Clear();
            this.SelectedTransitTrip = null;

            if (string.IsNullOrWhiteSpace(this.StartLocationText))
            {
                ProcessErrorMessage(SR.ErrorMsgTitleStartPointNotSet, SR.ErrorMsgDescStartPointNotSet);
                return;
            }

            if (string.IsNullOrWhiteSpace(this.EndLocationText))
            {
                ProcessErrorMessage(SR.ErrorMsgTitleEndPointNotSet, SR.ErrorMsgDescEndPointNotSet);
                return;
            }

            // Trim whitespace from the start/end locations
            this.StartLocationText = this.StartLocationText.Trim();
            this.EndLocationText = this.EndLocationText.Trim();

            if (this.StartLocationText == SR.MyCurrentLocationText)
            {
                this._isStartLocationStale = true;
                this.SelectedStartLocation = null;
            }

            if (this.EndLocationText == SR.MyCurrentLocationText)
            {
                this._isEndLocationStale = true;
                this.SelectedEndLocation = null;
            }

            AutoCompleteDataManager.AddSearchStringEntry(this.StartLocationText);
            AutoCompleteDataManager.AddSearchStringEntry(this.EndLocationText);

            this.CoreCalculateTransit();
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

        public void SwapEndPoints()
        {
            bool newStartLocationStale = this._isEndLocationStale;
            bool newEndLocationStale = this._isStartLocationStale;

            string temp = this.EndLocationText;
            this.EndLocationText = this.StartLocationText;
            this.StartLocationText = temp;

            LocationDescription tempLocation = this.SelectedEndLocation;
            this.SelectedEndLocation = this.SelectedStartLocation;
            this.SelectedStartLocation = tempLocation;

            this._isStartLocationStale = newStartLocationStale;
            this._isEndLocationStale = newEndLocationStale;
        }

        internal void StartOver()
        {
            this.StartLocationText = SR.MyCurrentLocationText;
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

        private static void ProcessErrorMessage(string title, string errorMsg)
        {
            var dialogMessage = new DialogMessage(errorMsg, null)
            {
                Caption = title,
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
                ProcessErrorMessage(SR.ErrorMsgTitleNoNetwork, SR.ErrorMsgDescNoNetwork);
            }
        }

        private static void CheckLocationService(GeoPositionStatus status)
        {
            switch (status)
            {
                case GeoPositionStatus.Disabled:
                    ProcessErrorMessage(SR.ErrorMsgTitleLocationServicesOff, SR.ErrorMsgDescLocationServicesOff);
                    Messenger.Default.Send(new NotificationMessage<bool>(false, string.Empty), MessengerToken.EnableLocationButtonIndicator);
                    break;
                case GeoPositionStatus.NoData:
                case GeoPositionStatus.Initializing:
                    Messenger.Default.Send(new NotificationMessage<bool>(true, SR.ProgressBarAcquiringLocation), MessengerToken.MainMapProgressIndicator);
                    Messenger.Default.Send(new NotificationMessage<bool>(false, string.Empty), MessengerToken.EnableLocationButtonIndicator);
                    break;
                case GeoPositionStatus.Ready:
                    Messenger.Default.Send(new NotificationMessage<bool>(false, SR.ProgressBarAcquiringLocation), MessengerToken.MainMapProgressIndicator);
                    Messenger.Default.Send(new NotificationMessage<bool>(true, string.Empty), MessengerToken.EnableLocationButtonIndicator);
                    break;
            }
        }

        private void UpdatePixelWidth()
        {
            if (this.UserGeoCoordinate == null)
            {
                return;
            }

            // Mathematics for this are obtained from http://msdn.microsoft.com/en-us/library/aa940990.aspx
            var onScreenFactor = 156543.04 /*meters/pixel*/ * Math.Abs(Math.Cos(this.UserGeoCoordinate.Latitude)) / Math.Pow(2, this.MapZoomLevel);
            var radiusInMeters = this.UserGeoCoordinate.HorizontalAccuracy;
            var onScreenPixels = (int)Math.Ceiling(radiusInMeters / onScreenFactor);

            // Do not show the uncertainty circle under a certain threshold
            const int OnScreenPxThreshold = 52;
            const int AccuracyThreshold = 20;
            this.PixelWidth = (onScreenPixels < OnScreenPxThreshold || radiusInMeters < AccuracyThreshold) ? 0 : onScreenPixels;
        }

        private void CoreCalculateTransit()
        {
            // Notify calcul in progress
            Messenger.Default.Send(new NotificationMessage<bool>(true, SR.ProgressBarSearchingEndpoint), MessengerToken.MainMapProgressIndicator);
            Messenger.Default.Send(new NotificationMessage<bool>(false, "Locking UI"), MessengerToken.LockUiIndicator);

            if (ViewModelLocator.SettingsViewModelStatic.UseLocationSetting)
            {
                // Wait for location to be available
                if (this.GeoCoordinateWatcher.Status != GeoPositionStatus.Ready)
                {
                    CheckLocationService(this.GeoCoordinateWatcher.Status);
                    System.Threading.ThreadPool.QueueUserWorkItem(_ =>
                        {
                            System.Threading.Thread.Sleep(1000);
                            DispatcherHelper.CheckBeginInvokeOnUI(this.CoreCalculateTransit);
                        });
                    return;
                }

                if (this._isStartLocationStale
                    && SR.MyCurrentLocationText.Equals(this.StartLocationText, StringComparison.OrdinalIgnoreCase))
                {
                    this.UpdateLocation(
                        "start",
                        new LocationDescription(this.UserGeoCoordinate) { DisplayName = SR.MyCurrentLocationText });
                    return;
                }

                if (this._isEndLocationStale
                    && SR.MyCurrentLocationText.Equals(this.EndLocationText, StringComparison.OrdinalIgnoreCase))
                {
                    this.UpdateLocation(
                        "end",
                        new LocationDescription(this.UserGeoCoordinate) { DisplayName = SR.MyCurrentLocationText });
                    return;
                }
            }

            if (this._isStartLocationStale)
            {
                ProxyQuery.GetLocationsAndBusiness(this.StartLocationText, this.UserGeoCoordinate ?? this.CenterMapGeoCoordinate).ContinueWith(
                    continuation => this.GetLocationsAndBusinessCallback(continuation.Result, "start", this.StartLocationText));
                return;
            }

            if (this._isEndLocationStale)
            {
                ProxyQuery.GetLocationsAndBusiness(this.EndLocationText, this.UserGeoCoordinate ?? this.CenterMapGeoCoordinate).ContinueWith(
                    continuation => this.GetLocationsAndBusinessCallback(continuation.Result, "end", this.EndLocationText));
                return;
            }

            // Notify calcul in progress
            Messenger.Default.Send(new NotificationMessage<bool>(true, SR.ProgressBarSearchingTrips), MessengerToken.MainMapProgressIndicator);

            // Ensure time is up-to-date if using Now
            if (this.TimeType == TimeCondition.Now)
            {
                this.DateTime = DateTime.Now;
            }

            // TODO: fix initial context state not set. Hacked up in view startup.
            ProxyQuery.GetTransitDirections(
                this._selectedStartLocation.GeoCoordinate,
                this._selectedEndLocation.GeoCoordinate,
                this.DateTime,
                this.TimeType).ContinueWith(
                continuation =>
                {
                    var transitDirections = continuation.Result.ToList();

                    // Notify progress bar calcul is done
                    Messenger.Default.Send(new NotificationMessage<bool>(false, string.Empty), MessengerToken.MainMapProgressIndicator);
                    Messenger.Default.Send(new NotificationMessage<bool>(true, "Unlocking UI"), MessengerToken.LockUiIndicator);

                    if (transitDirections.Count == 0)
                    {
                        ProcessErrorMessage(SR.ErrorMsgTitleNoTransitFound, "No transit or reasonable walking directions could be found.");
                        return;
                    }

                    DispatcherHelper.CheckBeginInvokeOnUI(
                        () =>
                        {
                            this.TransitDescriptionCollection.Clear();
                            foreach (var transit in transitDirections)
                            {
                                this.TransitDescriptionCollection.Add(transit);
                            }
                        });

                    Messenger.Default.Send(new NotificationMessage(string.Empty), MessengerToken.TransitTripsReady);
                });
        }

        private void GetLocationsAndBusinessCallback(IEnumerable<LocationDescription> locations, string endpoint, string query)
        {
            var locationDescriptions = locations.ToList();

            if (locationDescriptions.Count == 0)
            {
                ProcessErrorMessage(SR.ErrorMsgTitleLocationNotFound, string.Format(CultureInfo.InvariantCulture, "Could not locate a result for {0}.", query));
                return;
            }

            if (locationDescriptions.Count == 1)
            {
                this.UpdateLocation(endpoint, locationDescriptions[0]);
            }
            else
            {
                Messenger.Default.Send(new NotificationMessage<List<LocationDescription>>(locationDescriptions, endpoint), MessengerToken.EndpointResolutionPopup);
                Messenger.Default.Send(new NotificationMessage<string>(query, endpoint), MessengerToken.EndpointResolutionPopup);
            }
        }

        private void UpdateLocation(string endpoint, LocationDescription location)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(
                    () =>
                    {
                        AutoCompleteDataManager.AddLocationEntry(location);

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

                        this.CoreCalculateTransit();
                    });
        }

        private void DeviceNetworkInformationNetworkAvailabilityChanged(object sender, NetworkNotificationEventArgs e)
        {
            CheckNetwork();
        }

        private void GeoCoordinateWatcherPositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            this.UserGeoCoordinate = e.Position.Location;
            if (!this.CenterMapGeoSet)
            {
                this.CenterMapGeoCoordinate = this.UserGeoCoordinate;
            }

            if (this.UserGeoCoordinate.HorizontalAccuracy < 4)
            {
                this.GeoCoordinateWatcher.MovementThreshold = Globals.MovementThreshold;
            }
        }

        private void GeoCoordinateWatcherStatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            this.GeoCoordinateWatcher.MovementThreshold = 0;
            CheckLocationService(e.Status);
        }
    }
}