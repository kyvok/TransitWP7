using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using TransitWP7.ViewModel;

namespace TransitWP7.View
{
    public partial class DirectionsView : PhoneApplicationPage
    {
        private readonly DirectionsViewModel _viewModel;

        public DirectionsView()
        {
            this.InitializeComponent();
            this._viewModel = ViewModelLocator.DirectionsViewModelStatic;
        }

        private void DirectionsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.directionsList.SelectedIndex != -1)
            {
                Messenger.Default.Send(new NotificationMessage<int>(this.directionsList.SelectedIndex, string.Empty), MessengerToken.TripStepSelection);
                this.NavigationService.GoBack();
            }
        }
    }
}