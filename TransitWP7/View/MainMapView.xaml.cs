﻿using System;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
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
            this.RegisterForNotification(
                "SelectedItem",
                this.directionsStepView,
                (d, e) =>
                {
                    if (this._viewModel.SelectedTransitTrip != null)
                    {
                        this.mainMap.SetView(this._viewModel.SelectedTransitTrip.ItinerarySteps[this.directionsStepView.SelectedItem].GeoCoordinate, 16);
                    }
                });

            this._viewModel.DoServiceChecks();

            this.SetUIVisibility(UIViewState.OnlyStartEndInputsView);

            LittleWatson.CheckForPreviousException();
        }

        private enum UIViewState
        {
            ItineraryView,
            TransitOptionsView,
            OnlyStartEndInputsView,
            MapViewOnly
        }

        private enum AppBarMenuItemOrder
        {
            Directions = 0,
            TransitOptions,
            ClearMap,
            Settings,
            About
        }

        private UIViewState CurrentViewState { get; set; }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            // reset the progressbar and UI
            Messenger.Default.Send(new NotificationMessage<bool>(false, string.Empty), MessengerToken.MainMapProgressIndicator);
            Messenger.Default.Send(new NotificationMessage<bool>(true, "Unlocking UI"), MessengerToken.LockUiIndicator);
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

            Messenger.Default.Register<NotificationMessage<bool>>(
                this,
                MessengerToken.MainMapProgressIndicator,
                notificationMessage => DispatcherHelper.UIDispatcher.BeginInvoke(
                    () => this.SetProgressBarState(notificationMessage.Notification, notificationMessage.Content)));

            Messenger.Default.Register<NotificationMessage<bool>>(
                this,
                MessengerToken.LockUiIndicator,
                notificationMessage => DispatcherHelper.UIDispatcher.BeginInvoke(
                    () =>
                    {
                        this.IsEnabled = notificationMessage.Content;
                        this.ApplicationBar.IsVisible = notificationMessage.Content;
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
            var inputBox = sender as AutoCompleteBox;
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
            // TODO: if not location enabled, ask permission
            this.mainMap.SetView(this._viewModel.UserGeoCoordinate, 15);
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
            this.mainMap.SetView(this._viewModel.UserGeoCoordinate, 15);

            // the following is a workaround for the appbar preventing the update of binding for textbox
            this.startingInput.Text += " ";
            this.endingInput.Text += " ";

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
            this._viewModel.TryResolveEndpoints();
        }

        private void ApplicationBarAbout_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
        }

        private void SetUIVisibility(UIViewState uiState)
        {
            switch (uiState)
            {
                case UIViewState.ItineraryView:
                    this.ItineraryViewAnimation.Begin();
                    this.ApplicationBar.IsVisible = true;
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
                this.SetUIVisibility(UIViewState.ItineraryView);
                this.mainMap.SetView(this._viewModel.SelectedTransitTrip.MapView);

                // set zoom a little lower so endpoints don't underlap overlays
                this.mainMap.ZoomLevel = this.mainMap.TargetZoomLevel - 0.4;
            }
        }

        private void ApplicationBarTransitSearch_Click(object sender, EventArgs e)
        {
            this.SetUIVisibility(UIViewState.OnlyStartEndInputsView);
            this.endingInput.Focus();
        }
    }
}