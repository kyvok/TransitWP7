using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

namespace TransitWP7.ViewModel
{
    public class LocationSelectionViewModel : ViewModelBase
    {
        private string _endpointName = "end";
        private ObservableCollection<LocationDescription> _locationDescriptions; 

        // TODO: calculate distance of origin or current location to each point
        public LocationSelectionViewModel()
        {
            Messenger.Default.Register<NotificationMessage<List<LocationDescription>>>(
                this,
                MessengerToken.EndpointResolutionPopup,
                notificationMessage => DispatcherHelper.UIDispatcher.BeginInvoke(
                    () =>
                    {
                        this._locationDescriptions = new ObservableCollection<LocationDescription>(notificationMessage.Content);
                    }));
        }

        public string EndpointName
        {
            get 
            {
                return this._endpointName;
            }

            set
            {
                if (value != this._endpointName)
                {
                    this._endpointName = value;
                    this.RaisePropertyChanged("EndpointName");
                }
            }
        }

        public ObservableCollection<LocationDescription> LocationDescriptions
        {
            get
            {
                return this._locationDescriptions;
            }

            set
            {
                if (value != this._locationDescriptions)
                {
                    this._locationDescriptions = value;
                    this.RaisePropertyChanged("LocationDescriptions");
                }
            }
        }

        public void SelectionMade(int selectedIndex)
        {
            var notificationMessage = new NotificationMessage<LocationDescription>(this.LocationDescriptions[selectedIndex], this.EndpointName);
            Messenger.Default.Send(notificationMessage, MessengerToken.SelectedEndpoint);
        }
    }
}
