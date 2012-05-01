namespace TransitWP7.ViewModel
{
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Messaging;
    using GalaSoft.MvvmLight.Threading;
    using TransitWP7.Model;

    public class DirectionsViewModel : ViewModelBase
    {
        private TransitDescription _transitDescription;

        public DirectionsViewModel()
        {
            Messenger.Default.Register<NotificationMessage<TransitDescription>>(
                this,
                MessengerToken.SelectedTransitTrip,
                notificationMessage => DispatcherHelper.CheckBeginInvokeOnUI(
                    () =>
                    {
                        this.TransitDescription = notificationMessage.Content;
                    }));

#if DEBUG
            if (IsInDesignModeStatic)
            {
                this.TransitDescription = ViewModelLocator.TransitDescriptionsTestValues[0];
            }
#endif
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
