using System;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Shell;
using TransitWP7.ViewModel;

namespace TransitWP7.View
{
    // TODO: Localize this app properly. Will need a resource file.
    public partial class MainMapView : PhoneApplicationPage
    {
        private readonly MainMapViewModel _viewModel;

        public MainMapView()
        {
            this.InitializeComponent();
            this._viewModel = ViewModelLocator.MainMapViewModelStatic;
            this.mainMap.CredentialsProvider = new ApplicationIdCredentialsProvider(ApiKeys.BingMapsKey);
            this.mainMap.SetView(new GeoCoordinate(39.450, -98.908), 3.3);

            this.startingInput.ItemsSource = new ObservableCollection<string> { Globals.MyCurrentLocationText };
            this.endingInput.ItemsSource = new ObservableCollection<string> { Globals.MyCurrentLocationText };

            this.RegisterNotifications();

            this._viewModel.DoServiceChecks();

            LittleWatson.CheckForPreviousException();
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            // reset the progressbar
            Messenger.Default.Send(new NotificationMessage<bool>(false, string.Empty), MessengerToken.MainMapProgressIndicator);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            // the initial state of the application
            if (this.topGrid.Visibility == Visibility.Visible && this.bottomGrid.Visibility == Visibility.Collapsed)
            {
                base.OnBackKeyPress(e);
                return;
            }

            e.Cancel = true;

            // directions list has been displayed
            if (this.topGrid.Visibility == Visibility.Visible && this.bottomGrid.Visibility == Visibility.Visible)
            {
                this.topGrid.Visibility = Visibility.Visible;
                this.bottomGrid.Visibility = Visibility.Collapsed;

                return;
            }

            // route has been selected
            if (this.topGrid.Visibility == Visibility.Collapsed && this.bottomGrid.Visibility == Visibility.Collapsed)
            {
                this.topGrid.Visibility = Visibility.Visible;
                this.bottomGrid.Visibility = Visibility.Visible;
                this.directionsGrid.Height = 0;
                return;
            }
        }

        private void RegisterNotifications()
        {
            Messenger.Default.Register<DialogMessage>(
                this,
                MessengerToken.ErrorPopup,
                dialogMessage => DispatcherHelper.UIDispatcher.BeginInvoke(
                    () =>
                    {
                        var result = MessageBox.Show(dialogMessage.Content, dialogMessage.Caption, dialogMessage.Button);
                        dialogMessage.ProcessCallback(result);
                    }));

            Messenger.Default.Register<NotificationMessage<bool>>(
                this,
                MessengerToken.MainMapProgressIndicator,
                notificationMessage => DispatcherHelper.UIDispatcher.BeginInvoke(
                    () => this.SetProgressBarState(notificationMessage.Notification, notificationMessage.Content)));

            Messenger.Default.Register<NotificationMessage>(
                this,
                MessengerToken.EndpointResolutionPopup,
                notificationMessage => DispatcherHelper.UIDispatcher.BeginInvoke(
                    () => NavigationService.Navigate(
                        new Uri(
                            string.Format("{0}?endpoint={1}", PhonePageUri.LocationSelectionView, notificationMessage.Notification),
                            UriKind.Relative))));

            Messenger.Default.Register<NotificationMessage>(
                this,
                MessengerToken.TransitTripsReady,
                notificationMessage => DispatcherHelper.UIDispatcher.BeginInvoke(this.ShowTransitTripsList));
        }

        private void ShowTransitTripsList()
        {
            this.directionsGrid.Height = 0;
            this.topGrid.Visibility = Visibility.Visible;
            this.TransitTripsList.ItemsSource = this._viewModel.TransitDescriptionCollection;
            this.bottomGrid.Height = 800 - this.topGrid.ActualHeight - 32 - 72;
            this.bottomGrid.Visibility = Visibility.Visible;
        }

        private void TextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = sender as AutoCompleteBox;
                if (textBox.Name == "startingInput")
                {
                    this.endingInput.Focus();
                }
                else
                {
                    this.Focus();
                    this._viewModel.TryResolveEndpoints();
                }
            }
        }

        private void InputBox_GotFocus(object sender, RoutedEventArgs e)
        {
            this.ApplicationBar.IsVisible = false;
            var inputBox = sender as AutoCompleteBox;
            inputBox.MinimumPrefixLength = 1;
            inputBox.Background = new SolidColorBrush(Colors.Transparent);
            inputBox.BorderBrush = Application.Current.Resources["UnderliningBorderBrush"] as Brush;

            // Need to traverse the DependencyObject tree to fetch the TextBox.
            // First layer is Grid, which contains as a first child the TextBox.
            var parent = VisualTreeHelper.GetChild(inputBox, 0);
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i) as TextBox;
                if (child != null)
                {
                    child.SelectAll();
                    break;
                }
            }
        }

        private void InputBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.ApplicationBar.IsVisible = true;
            var inputBox = sender as AutoCompleteBox;
            inputBox.MinimumPrefixLength = -1;
        }

        private void TextBlock_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var temp = this.startingInput.Text;
            this.startingInput.Text = this.endingInput.Text;
            this.endingInput.Text = temp;
        }

        private void ApplicationBarLocateMe_Click(object sender, EventArgs e)
        {
            // TODO: if not location enabled, ask permission
            this.mainMap.SetView(this._viewModel.UserGeoCoordinate, 16);
        }
 
        private void ApplicationBarDirectionsList_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri(PhonePageUri.DirectionsView, UriKind.Relative));
        }

        private void SetProgressBarState(string message, bool state)
        {
            SystemTray.ProgressIndicator.Text = message;
            SystemTray.ProgressIndicator.IsIndeterminate = state;
            SystemTray.ProgressIndicator.IsVisible = state;
        }

        private void TransitTripsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.bottomGrid.Visibility = Visibility.Collapsed;
            this.topGrid.Visibility = Visibility.Collapsed;
            this.directionsGrid.Height = double.NaN;

            this._viewModel.SelectedTransitTrip = this.TransitTripsList.SelectedIndex >= 0
                ? this._viewModel.TransitDescriptionCollection[this.TransitTripsList.SelectedIndex]
                : null;

            var notificationMessage = new NotificationMessage<TransitDescription>(
                                                                                this,
                                                                                this._viewModel.SelectedTransitTrip,
                                                                                string.Empty);
            Messenger.Default.Send(notificationMessage, MessengerToken.SelectedTransitTrip);

            if (this._viewModel.SelectedTransitTrip != null)
            {
                this.mainMap.SetView(this._viewModel.SelectedTransitTrip.MapView);
            }
        }

        private void Pushpin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var pushpin = sender as Pushpin;
            NavigationService.Navigate(new Uri(string.Format("{0}?selectedIndex={1}", PhonePageUri.DirectionsView, (int)pushpin.Content + 1), UriKind.Relative));
        }

        private void ApplicationBarShowTransitOptions_Click(object sender, EventArgs e)
        {
            this.ShowTransitTripsList();
        }

        private void ApplicationBarClearMap_Click(object sender, EventArgs e)
        {
            this._viewModel.StartOver();
            this.bottomGrid.Visibility = Visibility.Collapsed;
            this.topGrid.Visibility = Visibility.Visible;
            this.mainMap.SetView(this._viewModel.UserGeoCoordinate, 10);
            this.endingInput.Focus();
        }

        private void EndpointInputTextChanged(object sender, RoutedEventArgs routedEventArgs)
        {
            ((AutoCompleteBox)sender).GetBindingExpression(AutoCompleteBox.TextProperty).UpdateSource();
        }

        private void ListPickerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var listPicker = sender as ListPicker;
            listPicker.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void ApplicationBarSettings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri(PhonePageUri.SettingsView, UriKind.Relative));
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            this._viewModel.TryResolveEndpoints();
        }

        private void ApplicationBarAbout_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
        }

        private void MainMap_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            this._viewModel.CenterMapGeoCoordinate = this.mainMap.Center;
        }
    }
}