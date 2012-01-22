
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace TransitWP7.ViewModels
{
    public class LocationSelectionViewModel : ViewModelBase
    {
        public string endpointName;

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
            var nm = new NotificationMessage<int>(selectedIndex, endpointName);
            Messenger.Default.Send(nm);
        }
    }
}
