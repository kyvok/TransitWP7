using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace TransitWP7.ViewModels
{
    public class LocationSelectionViewModel : ViewModelBase
    {
        public string EndpointName;

        // TODO: calculate distance of origin or current location to each point
        public LocationSelectionViewModel()
        {
        }

        public TransitRequestContext Context
        {
            get
            {
                return TransitRequestContext.Current;
            }
        }

        public void SelectionMade(int selectedIndex)
        {
            var nm = new NotificationMessage<int>(selectedIndex, this.EndpointName);
            Messenger.Default.Send(nm);
        }
    }
}
