namespace TransitWP7.View
{
    using GalaSoft.MvvmLight.Messaging;
    using Microsoft.Phone.Controls;

    public partial class DirectionsView : PhoneApplicationPage
    {
        public DirectionsView()
        {
            this.InitializeComponent();
        }

        private void DirectionsListSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.directionsList.SelectedIndex != -1)
            {
                Messenger.Default.Send(new NotificationMessage<int>(this.directionsList.SelectedIndex, string.Empty), MessengerToken.TripStepSelection);
                this.NavigationService.GoBack();
            }
        }
    }
}