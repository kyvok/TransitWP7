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

namespace TransitWP7.ViewModels
{
    // TODO: progressindicator native to be enabled when calculation in progress. NATIVE, not the progressbar one.
    public class MainMapViewModel : ViewModelBase
    {
        private string _startLocationText;
        private bool _isStartLocationStale = true;
        private string _endLocationText;
        private bool _isEndLocationStale = true;

        public List<TransitDescription> TransitTrips;
        public ObservableCollection<SummaryTransitData> FormattedTransitTrips = new ObservableCollection<SummaryTransitData>();

        public MainMapViewModel()
        {
            GeoLocation.Instance.GeoWatcher.PositionChanged += this.watcher_PositionChanged;

            Messenger.Default.Register<NotificationMessage<int>>(
                this,
                notificationMessage =>
                {
                    if (notificationMessage.Notification.Equals("start"))
                    {
                        this.UpdateLocation("start", this.Context._possibleStartLocations[notificationMessage.Content]);
                    }
                    else
                    {
                        this.UpdateLocation("end", this.Context._possibleEndLocations[notificationMessage.Content]);
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
            Messenger.Default.Send(new NotificationMessage<bool>(true, "Resolving endpoints..."));

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
            Messenger.Default.Send(new NotificationMessage<bool>(true, "Calculating transit trips..."));

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
            Messenger.Default.Send(new NotificationMessage<bool>(false, string.Empty));

            if (result.Error != null)
            {
                ProcessErrorMessage(result.Error.Message);
                return;
            }

            this.Context.TransitDescriptionCollection = new ObservableCollection<TransitDescription>(result.TransitDescriptions);
            this.DisplayTransitTripSummaries();
        }

        // TODO: Add Start and Endpoint to the list of steps. Transit missing end pushpin always.
        private void DisplayTransitTripSummaries()
        {
            foreach (var transitOption in TransitRequestContext.Current.TransitDescriptionCollection)
            {
                var atd = new SummaryTransitData();
                var isWalk = false;
                foreach (var item in transitOption.ItinerarySteps)
                {
                    if (item.IconType == string.Empty)
                    {
                        continue;
                    }

                    if (item.IconType.StartsWith("W"))
                    {
                        if (!isWalk)
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(
                                () =>
                                {
                                    var img = new Image
                                                  {
                                                      Source = new BitmapImage(new Uri("/images/walk_lo.png", UriKind.Relative))
                                                  };
                                    atd.Steps.Add(new TransitStep(string.Empty, img));
                                });
                        }
                    }
                    else if (item.IconType.StartsWith("B"))
                    {
                        var item1 = item;
                        Deployment.Current.Dispatcher.BeginInvoke(
                            () =>
                            {
                                var img = new Image
                                              {
                                                  Source = new BitmapImage(new Uri("/images/bus_lo.png", UriKind.Relative))
                                              };
                                atd.Steps.Add(new TransitStep(item1.BusNumber, img));
                            });
                    }

                    isWalk = item.TravelMode.StartsWith("W") ? true : false;
                }

                atd.Duration = ((int)(transitOption.TravelDuration / 60)).ToString(CultureInfo.InvariantCulture) + " min";
                atd.ArrivesAt = transitOption.ArrivalTime;
                atd.DepartsAt = transitOption.DepartureTime;

                DispatcherHelper.UIDispatcher.BeginInvoke(() => this.FormattedTransitTrips.Add(atd));

                Messenger.Default.Send(new NotificationMessage("transit"));
            }
        }

        private static void ProcessErrorMessage(string errorMsg)
        {
            var dm = new DialogMessage(errorMsg, null)
                         {
                             Caption = "Oups!",
                             Button = MessageBoxButton.OK
                         };
            Messenger.Default.Send(dm);

            // collapse any pending progressIndicator
            Messenger.Default.Send(new NotificationMessage<bool>(false, string.Empty));
        }
    }

    // TODO: get rid of these 2 classes!
    public class SummaryTransitData
    {
        public SummaryTransitData()
        {
            this.Steps = new List<TransitStep>();
        }

        public List<TransitStep> Steps { get; set; }
        public string Duration { get; set; }
        public string DepartsAt { get; set; }
        public string ArrivesAt { get; set; }
    }

    public class TransitStep
    {
        public TransitStep(string str, Image image)
        {
            this.Str = str;
            this.Image = image;
        }

        public string Str { get; set; }
        public Image Image { get; set; }
    }
}