using System;
using System.Collections.ObjectModel;
using System.Device.Location;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Controls.Maps;

namespace TransitWP7.ViewModel
{
    public class DirectionsViewModel : ViewModelBase
    {
        private TransitDescription _transitDescription;

        public DirectionsViewModel()
        {
            Messenger.Default.Register<NotificationMessage<TransitDescription>>(
                this,
                MessengerToken.SelectedTransitTrip,
                notificationMessage => DispatcherHelper.UIDispatcher.BeginInvoke(
                    () =>
                    {
                        this._transitDescription = notificationMessage.Content;
                    }));

            if (IsInDesignModeStatic)
            {
                this.TransitDescription = new TransitDescription()
                                              {
                                                  ArrivalTime = System.DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture),
                                                  DepartureTime = System.DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture),
                                                  StartLocation = new GeoCoordinate(0, 0),
                                                  EndLocation = new GeoCoordinate(0, 0),
                                                  MapView = new LocationRect(0, 0, 0, 0),
                                                  PathPoints = new LocationCollection(),
                                                  TravelDuration = 1234,
                                                  ItinerarySteps = new ObservableCollection<ItineraryStep>()
                                                                       {
                                                                           new ItineraryStep()
                                                                               {
                                                                                   BusNumber = "48",
                                                                                   GeoCoordinate = new GeoCoordinate(0, 0),
                                                                                   IconType = "Bus",
                                                                                   StepNumber = 1,
                                                                                   Time = DateTime.Now,
                                                                                   Hints = new ObservableCollection<string> { "hint 1", "hint 2" },
                                                                                   Instruction = "Do something, like walk, or take bus, or maybe run faster then light",
                                                                                   TravelMode = "Driving",
                                                                                   ChildItinerarySteps = new ObservableCollection<ItineraryStep>
                                                                                                             {
                                                                                                                 new ItineraryStep()
                                                                                                                     {
                                                                                                                         Instruction = "Take Bus (child step)"
                                                                                                                     }
                                                                                                             }
                                                                               },
                                                                        new ItineraryStep()
                                                                               {
                                                                                   GeoCoordinate = new GeoCoordinate(0, 0),
                                                                                   IconType = "Walk",
                                                                                   StepNumber = 2,
                                                                                   Time = DateTime.Now,
                                                                                   Hints = new ObservableCollection<string> { "hint 1", "hint 2" },
                                                                                   Instruction = "Do something, like walk, or take bus, or maybe run faster then light",
                                                                                   TravelMode = "Walk",
                                                                               }
                                                                       }
                                              };
            }
        }

        public TransitDescription TransitDescription
        {
            get
            {
                return this._transitDescription;
            }

            set
            {
                if (value != this._transitDescription)
                {
                    this._transitDescription = value;
                    this.RaisePropertyChanged("TransitDescription");
                }
            }
        }
    }
}
