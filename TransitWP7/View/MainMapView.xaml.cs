using System;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Shell;
using TransitWP7.Resources;
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

            this.startingInput.ItemsSource = new ObservableCollection<string> { SR.MyCurrentLocationText };
            this.endingInput.ItemsSource = new ObservableCollection<string> { SR.MyCurrentLocationText };

            this.RegisterNotifications();
            this.RegisterForNotification(
                "SelectedItem",
                this.directionsStepView,
                (d, e) =>
                {
                    if (this._viewModel.SelectedTransitTrip != null)
                    {
                        this.mainMap.SetView(this._viewModel.SelectedTransitTrip.ItinerarySteps[this.directionsStepView.SelectedItem == -1 ? 0 : directionsStepView.SelectedItem].GeoCoordinate, Globals.LocateMeZoomLevel);
                    }
                });

            // Must be able to quit application with BackKey press when opening the app.
            // So show the map view always when starting up.
            this.SetUIVisibility(UIViewState.MapViewOnly);

            // Additional logic for application start up view.
            if (this._viewModel.SelectedTransitTrip != null)
            {
                this.mainMap.SetView(this._viewModel.SelectedTransitTrip.MapView);

                // set zoom a little lower so endpoints don't underlap overlays
                this.mainMap.ZoomLevel = this.mainMap.TargetZoomLevel - 0.4;
            }
            else if (!this._viewModel.CenterMapGeoSet && ViewModelLocator.SettingsViewModelStatic.UseLocationSetting && this._viewModel.UserGeoCoordinate != null)
            {
                this.mainMap.SetView(this._viewModel.UserGeoCoordinate, Globals.LocateMeZoomLevel);
            }

            LittleWatson.CheckForPreviousException();
        }

        private enum UIViewState
        {
            OnlyStartEndInputsView,
            MapViewOnly,
            TransitOptionsView,
            ItineraryView,
        }

        private enum AppBarMenuItemOrder
        {
            Directions = 0,
            TransitOptions,
            ClearMap,
            Settings,
            About
        }

        private enum AppBarIconOrder
        {
            NewSearch = 0,
            LocateMe
        }

        private UIViewState CurrentViewState { get; set; }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            // reset the progressbar and UI
            Messenger.Default.Send(new NotificationMessage<bool>(false, string.Empty), MessengerToken.MainMapProgressIndicator);
            Messenger.Default.Send(new NotificationMessage<bool>(true, "Unlocking UI"), MessengerToken.LockUiIndicator);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Ensure UI is correct when navigating back from other pages.
            this.SetUIVisibility(this.CurrentViewState);

            // Use background thread for this.
            ThreadPool.QueueUserWorkItem(_ => DispatcherHelper.UIDispatcher.BeginInvoke(this.FirstRunCheck));
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            switch (this.CurrentViewState)
            {
                case UIViewState.MapViewOnly:
                    base.OnBackKeyPress(e);
                    return;
                case UIViewState.OnlyStartEndInputsView:
                    this.SetUIVisibility(UIViewState.MapViewOnly);
                    break;
                case UIViewState.TransitOptionsView:
                    this.SetUIVisibility(UIViewState.OnlyStartEndInputsView);
                    break;
                case UIViewState.ItineraryView:
                    this.SetUIVisibility(UIViewState.TransitOptionsView);
                    break;
            }

            e.Cancel = true;
        }

        private void FirstRunCheck()
        {
            if (string.IsNullOrEmpty(ViewModelLocator.SettingsViewModelStatic.FirstLaunchSetting))
            {
                var result = MessageBox.Show(SR.FirstRunAuthorizeLocationDesc, SR.FirstRunAuthorizeLocationTitle, MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    ViewModelLocator.SettingsViewModelStatic.UseLocationSetting = true;
                }
                else
                {
                    ViewModelLocator.SettingsViewModelStatic.UseLocationSetting = false;
                }

                ViewModelLocator.SettingsViewModelStatic.FirstLaunchSetting = "set";
            }

            this._viewModel.InitializeGeoCoordinateWatcher();
            this._viewModel.DoServiceChecks();
        }

        private void RegisterForNotification(string propertyName, FrameworkElement element, PropertyChangedCallback callback)
        {
            // Bind to a depedency property
            var b = new Binding(propertyName) { Source = element };
            var prop = DependencyProperty.RegisterAttached(
                "ListenAttached" + propertyName,
                typeof(object),
                typeof(UserControl),
                new PropertyMetadata(callback));

            element.SetBinding(prop, b);
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

            Messenger.Default.Register<NotificationMessage<int>>(
               this,
               MessengerToken.TripStepSelection,
               notificationMessage => DispatcherHelper.UIDispatcher.BeginInvoke(
                   () =>
                   {
                       this.directionsStepView.SelectedItem = notificationMessage.Content;
                   }));

            Messenger.Default.Register<NotificationMessage<bool>>(
                this,
                MessengerToken.MainMapProgressIndicator,
                notificationMessage => DispatcherHelper.UIDispatcher.BeginInvoke(
                    () =>
                    {
                        if (!NavigationService.CurrentSource.OriginalString.Contains("MainMapView"))
                        {
                            return;
                        }

                        this.SetProgressBarState(notificationMessage.Notification, notificationMessage.Content);
                    }));

            Messenger.Default.Register<NotificationMessage<bool>>(
                this,
                MessengerToken.LockUiIndicator,
                notificationMessage => DispatcherHelper.UIDispatcher.BeginInvoke(
                    () =>
                    {
                        this.IsEnabled = notificationMessage.Content;
                        this.ApplicationBar.IsVisible = notificationMessage.Content;
                    }));

            Messenger.Default.Register<NotificationMessage<bool>>(
                this,
                MessengerToken.EnableLocationButtonIndicator,
                notificationMessage => DispatcherHelper.UIDispatcher.BeginInvoke(
                    () =>
                    {
                        if (!NavigationService.CurrentSource.OriginalString.Contains("MainMapView"))
                        {
                            return;
                        }

                        // TODO: possibly pass a small string to indicate a third state for the meIndicator.
                        if (notificationMessage.Content)
                        {
                            this.meIndicator.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            this.meIndicator.Visibility = Visibility.Collapsed;
                        }

                        var locateMeButton = (ApplicationBarIconButton)this.ApplicationBar.Buttons[(int)AppBarIconOrder.LocateMe];
                        locateMeButton.IsEnabled = notificationMessage.Content;
                    }));

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
            this.SetUIVisibility(UIViewState.TransitOptionsView);
            this.bottomGrid.Height = 800 - this.topGrid.ActualHeight - 32;
        }

        private void TextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = sender as AutoCompleteBox;
                Debug.Assert(textBox != null, "textBox should be AutoCompleteBox");
                if (textBox.Name == "startingInput")
                {
                    this.endingInput.Focus();
                }
                else
                {
                    this.Focus();
                    this._viewModel.BeginCalculateTransit();
                }
            }
        }

        private void InputBox_GotFocus(object sender, RoutedEventArgs e)
        {
            this.ApplicationBar.IsVisible = false;
            var inputBox = sender as AutoCompleteBox;
            Debug.Assert(inputBox != null, "inputBox should be AutoCompleteBox");
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
            var inputBox = sender as AutoCompleteBox;
            Debug.Assert(inputBox != null, "inputBox should be AutoCompleteBox");
            inputBox.MinimumPrefixLength = -1;
            if (this.bottomGridTranslate.Y != 0)
            {
                this.ApplicationBar.IsVisible = true;
            }
        }

        private void SwapEndpoints(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var temp = this.startingInput.Text;
            this.startingInput.Text = this.endingInput.Text;
            this.endingInput.Text = temp;
        }

        private void ApplicationBarLocateMe_Click(object sender, EventArgs e)
        {
            this.mainMap.SetView(this._viewModel.UserGeoCoordinate, Globals.LocateMeZoomLevel);
            this._viewModel.CenterMapGeoSet = false;
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

        private void Pushpin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var pushpin = sender as Pushpin;
            this.directionsStepView.SelectedItem = ((int)pushpin.Content) - 1;
        }

        private void ApplicationBarShowTransitOptions_Click(object sender, EventArgs e)
        {
            this.ShowTransitTripsList();
        }

        private void ApplicationBarClearMap_Click(object sender, EventArgs e)
        {
            this._viewModel.StartOver();
            this.SetUIVisibility(UIViewState.OnlyStartEndInputsView);
            this.mainMap.SetView(this._viewModel.UserGeoCoordinate, Globals.LocateMeZoomLevel);
            this.directionsStepView.SelectedItem = -1;

            // the following is a workaround for the appbar preventing the update of binding for textbox
            this.startingInput.Text += " ";
            this.endingInput.Text += " ";
            this.startingInput.Text = this.startingInput.Text.Remove(this.startingInput.Text.Length - 1, 1);
            this.endingInput.Text = this.endingInput.Text.Remove(this.endingInput.Text.Length - 1, 1);

            this.endingInput.Focus();
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
            this._viewModel.BeginCalculateTransit();
        }

        private void ApplicationBarAbout_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
        }

        private void SetUIVisibility(UIViewState uiState)
        {
            // Stop all UI animation first to prevent concurrency issues.
            this.ItineraryViewAnimation.Pause();
            this.TransitOptionsViewAnimation.Pause();
            this.OnlyStartEndInputViewAnimation.Pause();
            this.MapViewOnlyAnimation.Pause();

            switch (uiState)
            {
                case UIViewState.ItineraryView:
                    this.ItineraryViewAnimation.Begin();
                    this.ApplicationBar.IsVisible = true;
                    if (this._viewModel.SelectedTransitTrip != null)
                    {
                        if (this.directionsStepView.SelectedItem == -1)
                        {
                            this.mainMap.SetView(this._viewModel.SelectedTransitTrip.MapView);

                            // set zoom a little lower so endpoints don't underlap overlays
                            this.mainMap.ZoomLevel = this.mainMap.TargetZoomLevel - 0.4;
                        }
                    }

                    break;
                case UIViewState.TransitOptionsView:
                    this.TransitOptionsViewAnimation.Begin();
                    this.ApplicationBar.IsVisible = false;
                    break;
                case UIViewState.OnlyStartEndInputsView:
                    this.OnlyStartEndInputViewAnimation.Begin();
                    this.ApplicationBar.IsVisible = true;
                    break;
                case UIViewState.MapViewOnly:
                    this.MapViewOnlyAnimation.Begin();
                    this.ApplicationBar.IsVisible = true;
                    break;
            }

            var appbarmenuDirections = (ApplicationBarMenuItem)this.ApplicationBar.MenuItems[(int)AppBarMenuItemOrder.Directions];
            appbarmenuDirections.IsEnabled = this._viewModel.SelectedTransitTrip != null;

            var appbarmenuTrips = (ApplicationBarMenuItem)this.ApplicationBar.MenuItems[(int)AppBarMenuItemOrder.TransitOptions];
            appbarmenuTrips.IsEnabled = this._viewModel.TransitDescriptionCollection != null
                                     && this._viewModel.TransitDescriptionCollection.Count != 0;

            this.CurrentViewState = uiState;
        }

        private void TransitTripsList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this._viewModel.SetSelectedTransitTrip(this.TransitTripsList.SelectedIndex);

            if (this._viewModel.SelectedTransitTrip != null)
            {
                this.directionsStepView.SelectedItem = -1;
                this.SetUIVisibility(UIViewState.ItineraryView);
            }
        }

        private void ApplicationBarTransitSearch_Click(object sender, EventArgs e)
        {
            this.SetUIVisibility(UIViewState.OnlyStartEndInputsView);
            this.startingInput.Focus();
        }

        private void MainMap_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (this.CurrentViewState != UIViewState.ItineraryView)
            {
                this.SetUIVisibility(UIViewState.MapViewOnly);
            }
        }

        private void MainMap_MapPan(object sender, MapDragEventArgs e)
        {
            this._viewModel.CenterMapGeoSet = true;
        }
    }
}