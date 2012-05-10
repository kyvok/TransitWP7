namespace TransitWP7.View
{
    using System;
    using System.ComponentModel;
    using System.Device.Location;
    using System.Globalization;
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
    using Microsoft.Phone.Tasks;
    using TransitWP7.Model;
    using TransitWP7.Resources;
    using TransitWP7.ViewModel;

    public partial class MainMapView : PhoneApplicationPage
    {
        private readonly MainMapViewModel _viewModel;

        public MainMapView()
        {
            this.InitializeComponent();

            this._viewModel = ViewModelLocator.MainMapViewModelStatic;
            this.mainMap.CredentialsProvider = new ApplicationIdCredentialsProvider(ApiKeys.BingMapsKey);
            this.mainMap.SetView(new GeoCoordinate(39.450, -98.908), 3.3);

            // The following is simply to have the timeConditionPicker text aligned.
            var temp = this._viewModel.TimeType;
            this._viewModel.TimeType = TimeCondition.LastArrivalTime;
            this._viewModel.TimeType = temp;

            this.RegisterNotifications();
            this.RegisterForNotification(
                "SelectedItem",
                this.directionsStepView,
                (d, e) =>
                {
                    if (this._viewModel.SelectedTransitTrip != null)
                    {
                        this._viewModel.CenterMapGeoSet = true;
                        this.mainMap.SetView(this._viewModel.SelectedTransitTrip.ItinerarySteps[this.directionsStepView.SelectedItem == -1 ? 0 : directionsStepView.SelectedItem].GeoCoordinate, Globals.LocateMeZoomLevel);
                        this.SetUIVisibility(UIViewState.ItineraryView);
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
            ClearMap,
            Settings,
            About
        }

        private enum AppBarIconOrder
        {
            NewSearch = 0,
            LocateMe,
            Results,
            Steps
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
            ThreadPool.QueueUserWorkItem(_ => DispatcherHelper.CheckBeginInvokeOnUI(this.FirstRunCheck));
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
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
                this._viewModel.StartOver();
                AutoCompleteDataManager.ResetData();
                var result = MessageBox.Show(SR.FirstRunAuthorizeLocationDesc, SR.FirstRunAuthorizeLocationTitle, MessageBoxButton.OKCancel);
                ViewModelLocator.SettingsViewModelStatic.UseLocationSetting = result == MessageBoxResult.OK;
                ViewModelLocator.SettingsViewModelStatic.FirstLaunchSetting = "set";
            }

            this._viewModel.InitializeGeoCoordinateWatcher();
            this._viewModel.DoServiceChecks();
        }

        private void RegisterForNotification(string propertyName, FrameworkElement element, PropertyChangedCallback callback)
        {
            // Bind to a dependency property
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
                dialogMessage => DispatcherHelper.CheckBeginInvokeOnUI(
                    () =>
                    {
                        var result = MessageBox.Show(dialogMessage.Content, dialogMessage.Caption, dialogMessage.Button);
                        dialogMessage.ProcessCallback(result);
                    }));

            Messenger.Default.Register<NotificationMessage<int>>(
               this,
               MessengerToken.TripStepSelection,
               notificationMessage => DispatcherHelper.CheckBeginInvokeOnUI(
                   () =>
                   {
                       this.directionsStepView.SelectedItem = notificationMessage.Content;
                   }));

            Messenger.Default.Register<NotificationMessage<bool>>(
                this,
                MessengerToken.MainMapProgressIndicator,
                notificationMessage => DispatcherHelper.CheckBeginInvokeOnUI(
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
                notificationMessage => DispatcherHelper.CheckBeginInvokeOnUI(
                    () =>
                    {
                        this.IsEnabled = notificationMessage.Content;
                        this.ApplicationBar.IsVisible = notificationMessage.Content;
                    }));

            Messenger.Default.Register<NotificationMessage<bool>>(
                this,
                MessengerToken.EnableLocationButtonIndicator,
                notificationMessage => DispatcherHelper.CheckBeginInvokeOnUI(
                    () =>
                    {
                        if (!NavigationService.CurrentSource.OriginalString.Contains("MainMapView"))
                        {
                            return;
                        }

                        this.meIndicator.Visibility = notificationMessage.Content ? Visibility.Visible : Visibility.Collapsed;
                        var locateMeButton = (ApplicationBarIconButton)this.ApplicationBar.Buttons[(int)AppBarIconOrder.LocateMe];
                        locateMeButton.IsEnabled = notificationMessage.Content;
                    }));

            Messenger.Default.Register<NotificationMessage<string>>(
                this,
                MessengerToken.EndpointResolutionPopup,
                notificationMessage => DispatcherHelper.CheckBeginInvokeOnUI(
                    () => NavigationService.Navigate(
                        new Uri(
                            string.Format(CultureInfo.InvariantCulture, "{0}?endpoint={1}&query={2}", PhonePageUri.LocationSelectionView, notificationMessage.Notification, Uri.EscapeDataString(notificationMessage.Content)),
                            UriKind.Relative))));

            Messenger.Default.Register<NotificationMessage>(
                this,
                MessengerToken.TransitTripsReady,
                notificationMessage => DispatcherHelper.CheckBeginInvokeOnUI(this.ShowTransitTripsList));
        }

        private void ShowTransitTripsList()
        {
            this.SetUIVisibility(UIViewState.TransitOptionsView);
        }

        private void TextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = (AutoCompleteBox)sender;
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

        private void InputBoxGotFocus(object sender, RoutedEventArgs e)
        {
            var inputBox = (AutoCompleteBox)sender;
            inputBox.MinimumPrefixLength = 1;
            inputBox.Background = new SolidColorBrush(Colors.Transparent);

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

        private void InputBoxLostFocus(object sender, RoutedEventArgs e)
        {
            var inputBox = (AutoCompleteBox)sender;
            inputBox.MinimumPrefixLength = -1;
        }

        private void InputBoxDropDownClosing(object sender, RoutedPropertyChangingEventArgs<bool> e)
        {
            var inputBox = (AutoCompleteBox)sender;
            var locationDescription = (LocationDescription)inputBox.SelectedItem;
            if (locationDescription != null)
            {
                if (inputBox.Name.Contains("start"))
                {
                    this._viewModel.StartLocationText = locationDescription.DisplayName;
                    this._viewModel.SelectedStartLocation = locationDescription;
                }
                else
                {
                    this._viewModel.EndLocationText = locationDescription.DisplayName;
                    this._viewModel.SelectedEndLocation = locationDescription;
                }
            }
        }

        private void SwapEndpoints(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this._viewModel.SwapEndPoints();
        }

        private void SetProgressBarState(string message, bool state)
        {
            // TODO: remove this Workaround for agressive check location status progress bar message.
            // Real fix needs to preserve history of progress messages.
            if (message == SR.ProgressBarAcquiringLocation && message != SystemTray.ProgressIndicator.Text)
            {
                return;
            }

            SystemTray.ProgressIndicator.Text = message;
            SystemTray.ProgressIndicator.IsIndeterminate = state;
            SystemTray.ProgressIndicator.IsVisible = state;
        }

        private void PushpinTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var pushpin = (Pushpin)sender;
            this.directionsStepView.SelectedItem = ((int)pushpin.Content) - 1;
        }

        private void ListPickerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var listPicker = (ListPicker)sender;
            listPicker.Background = new SolidColorBrush(Colors.Transparent);
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
                    this.ApplicationBar.IsVisible = false;
                    this.bottomGrid.Height = 800 - this.topGrid.ActualHeight - 32;
                    this.TransitOptionsViewAnimation.Begin();
                    break;
                case UIViewState.OnlyStartEndInputsView:
                    this.ApplicationBar.IsVisible = false;
                    this.OnlyStartEndInputViewAnimation.Completed += (s, a) => this.endingInput.Focus();
                    this.OnlyStartEndInputViewAnimation.Begin();
                    break;
                case UIViewState.MapViewOnly:
                    this.MapViewOnlyAnimation.Begin();
                    this.ApplicationBar.IsVisible = true;
                    break;
            }

            var appbarmenuDirections = (ApplicationBarMenuItem)this.ApplicationBar.MenuItems[(int)AppBarMenuItemOrder.Directions];
            appbarmenuDirections.IsEnabled = this._viewModel.SelectedTransitTrip != null;

            var appbariconResults = (ApplicationBarIconButton)this.ApplicationBar.Buttons[(int)AppBarIconOrder.Results];
            appbariconResults.IsEnabled = this._viewModel.TransitDescriptionCollection != null
                                       && this._viewModel.TransitDescriptionCollection.Count != 0;

            var appbariconSteps = (ApplicationBarIconButton)this.ApplicationBar.Buttons[(int)AppBarIconOrder.Steps];
            appbariconSteps.IsEnabled = this._viewModel.SelectedTransitTrip != null;

            this.CurrentViewState = uiState;
        }

        private void TransitTripsListTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this._viewModel.SetSelectedTransitTrip(this.TransitTripsList.SelectedIndex);

            if (this._viewModel.SelectedTransitTrip != null)
            {
                this.directionsStepView.SelectedItem = -1;
                this.SetUIVisibility(UIViewState.ItineraryView);
            }
        }

        private void ApplicationBarTransitSearchClick(object sender, EventArgs e)
        {
            this.SetUIVisibility(UIViewState.OnlyStartEndInputsView);
        }

        private void ApplicationBarSettingsClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri(PhonePageUri.SettingsView, UriKind.Relative));
        }

        private void ContentControlTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this._viewModel.BeginCalculateTransit();
        }

        private void ApplicationBarLocateMeClick(object sender, EventArgs e)
        {
            this.mainMap.SetView(this._viewModel.UserGeoCoordinate, Globals.LocateMeZoomLevel);
            this._viewModel.CenterMapGeoSet = false;
        }

        private void ApplicationBarDirectionsListClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri(PhonePageUri.DirectionsView, UriKind.Relative));
        }

        private void ApplicationBarShowTransitOptionsClick(object sender, EventArgs e)
        {
            this.ShowTransitTripsList();
        }

        private void ApplicationBarClearMapClick(object sender, EventArgs e)
        {
            this._viewModel.StartOver();
            this.SetUIVisibility(UIViewState.OnlyStartEndInputsView);
            this.mainMap.SetView(this._viewModel.UserGeoCoordinate ?? this._viewModel.CenterMapGeoCoordinate, Globals.LocateMeZoomLevel);
            this.directionsStepView.SelectedItem = -1;

            // the following is a workaround for the appbar preventing the update of binding for textbox
            this.startingInput.Text += " ";
            this.endingInput.Text += " ";
            this.startingInput.Text = this.startingInput.Text.TrimEnd();
            this.endingInput.Text = this.endingInput.Text.TrimEnd();

            this.endingInput.Focus();
        }

        private void ApplicationBarAboutClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarShowStepsClick(object sender, EventArgs e)
        {
            if (this.CurrentViewState == UIViewState.ItineraryView)
            {
                this.SetUIVisibility(UIViewState.MapViewOnly);
            }
            else
            {
                this.SetUIVisibility(UIViewState.ItineraryView);
            }
        }

        private void MainMapTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (this.CurrentViewState != UIViewState.ItineraryView)
            {
                this.SetUIVisibility(UIViewState.MapViewOnly);
            }

            this.selectedPoint.Visibility = Visibility.Collapsed;
        }

        private void MainMapMapPan(object sender, MapDragEventArgs e)
        {
            this._viewModel.CenterMapGeoSet = true;
        }

        private void MainMapHold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var map = (Map)sender;
            var point = e.GetPosition(map);
            this.selectedPoint.Visibility = Visibility.Visible;

            this.selectedPoint.Location = map.ViewportPointToLocation(point);
        }

        private void SelectedOnMapActionTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var pushpin = (Pushpin)sender;
            var point = e.GetPosition(pushpin);
            var startZone = point.X > 8 && point.X < 72 && point.Y < -16 && point.Y > -42;
            var endZone = point.X < -8 && point.X > -72 && point.Y > 16 && point.Y < 42;
            if (startZone || endZone)
            {
                this.SetUIVisibility(UIViewState.OnlyStartEndInputsView);
                pushpin.Visibility = Visibility.Collapsed;

                this.IsEnabled = false;

                ProxyQuery.GetLocationAddress(
                    pushpin.Location,
                    result => DispatcherHelper.CheckBeginInvokeOnUI(
                        () =>
                            {
                                var displayName = "Unknown location";
                                if (result.LocationDescriptions != null && result.LocationDescriptions.Count > 0)
                                {
                                    displayName = result.LocationDescriptions[0].DisplayName;
                                }

                                var loc = new LocationDescription(pushpin.Location)
                                    { DisplayName = displayName };
                                if (startZone)
                                {
                                    this._viewModel.StartLocationText = displayName;
                                    this._viewModel.SelectedStartLocation = loc;
                                }
                                else
                                {
                                    this._viewModel.EndLocationText = displayName;
                                    this._viewModel.SelectedEndLocation = loc;
                                }

                                this.IsEnabled = true;
                            }),
                    null);
            }

            e.Handled = true;
        }

        private void DateTimePickersTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (this._viewModel.TimeType == TimeCondition.Now)
            {
                this._viewModel.TimeType = TimeCondition.DepartingAt;
            }
        }

        private void FastTimeChangeClick(object sender, RoutedEventArgs e)
        {
            var button = (HyperlinkButton)sender;
            var condition = (string)button.Content;

            switch (condition)
            {
                case "<<15min":
                    this._viewModel.DateTime = this._viewModel.DateTime.Subtract(TimeSpan.FromMinutes(15));
                    break;
                case "<5min":
                    this._viewModel.DateTime = this._viewModel.DateTime.Subtract(TimeSpan.FromMinutes(5));
                    break;
                case "now":
                    this._viewModel.DateTime = DateTime.Now;
                    break;
                case "5min>":
                    this._viewModel.DateTime = this._viewModel.DateTime.Add(TimeSpan.FromMinutes(5));
                    break;
                case "15min>>":
                    this._viewModel.DateTime = this._viewModel.DateTime.Add(TimeSpan.FromMinutes(15));
                    break;
            }

            if (this._viewModel.TimeType == TimeCondition.Now)
            {
                this._viewModel.TimeType = TimeCondition.DepartingAt;
            }

            this._viewModel.BeginCalculateTransit();
        }

        private void GetContactAddressButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var addressChooserTask = new AddressChooserTask();
            addressChooserTask.Completed += (s, r) =>
                {
                    if (r.TaskResult == TaskResult.OK)
                    {
                        if (((Button)sender).Tag.Equals("start"))
                        {
                            this.startingInput.Text = r.Address;
                        }
                        else
                        {
                            this.endingInput.Text = r.Address;
                        }
                    }
                };
            addressChooserTask.Show();
        }
    }
}