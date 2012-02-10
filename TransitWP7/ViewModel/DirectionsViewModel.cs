﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

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
