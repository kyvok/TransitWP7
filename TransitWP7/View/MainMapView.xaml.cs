﻿using System;
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
            InitializeComponent();
            this._viewModel = ViewModelLocator.MainMapViewModelStatic;
            this.DataContext = this._viewModel;
            this.mainMap.CredentialsProvider = new ApplicationIdCredentialsProvider(ApiKeys.BingMapsKey);
            this.mainMap.SetView(new GeoCoordinate(39.450, -98.908), 3.3);

            this.RegisterNotifications();
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
                notificationMessage => DispatcherHelper.UIDispatcher.BeginInvoke(
                    () =>
                    {
                        this.bottomGrid.Visibility = Visibility.Visible;
                        this.transitTripsList.ItemsSource = this._viewModel.TransitDescriptionCollection;
                        this.bottomGrid.Height = 800 - this.topGrid.ActualHeight - 32;
                    }));
        }

        private void TextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Focus();
            }
        }

        private void InputBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var inputBox = sender as TextBox;
            inputBox.Background = new SolidColorBrush(Colors.Transparent);
            inputBox.SelectAll();
        }

        private void DatePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            this._viewModel.EnsureDateTimeSyncInContext(e.NewDateTime.Value, this._viewModel.Context.DateTime);
        }

        private void TimePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            this._viewModel.EnsureDateTimeSyncInContext(this._viewModel.Context.DateTime, e.NewDateTime.Value);
        }

        private void ListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var timeTypePicker = sender as ListPicker;
            if (timeTypePicker != null)
            {
                switch (timeTypePicker.SelectedIndex)
                {
                    case 0:
                        this._viewModel.EnsureDateTimeSyncInContext(DateTime.Now, DateTime.Now, TimeCondition.Now);
                        this.datePicker.IsEnabled = false;
                        this.timePicker.IsEnabled = false;
                        break;
                    case 1:
                        this._viewModel.EnsureDateTimeSyncInContext(datePicker.Value, timePicker.Value, TimeCondition.DepartingAt);
                        this.datePicker.IsEnabled = true;
                        this.timePicker.IsEnabled = true;
                        break;
                    case 2:
                        this._viewModel.EnsureDateTimeSyncInContext(datePicker.Value, timePicker.Value, TimeCondition.ArrivingAt);
                        this.datePicker.IsEnabled = true;
                        this.timePicker.IsEnabled = true;
                        break;
                    case 3:
                        this._viewModel.EnsureDateTimeSyncInContext(datePicker.Value, timePicker.Value, TimeCondition.LastArrivalTime);
                        this.datePicker.IsEnabled = true;
                        this.timePicker.IsEnabled = false;
                        break;
                }
            }
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var inputBox = sender as TextBlock;
            dateTimeStackPanel.Visibility = dateTimeStackPanel.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            inputBox.Text = dateTimeStackPanel.Visibility == Visibility.Collapsed ? "Show options" : "Hide options";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this._viewModel.TryResolveEndpoints();
        }

        private void TextBlock_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var temp = this.startingInput.Text;
            this.startingInput.Text = this.endingInput.Text;
            this.endingInput.Text = temp;
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            // TODO: if not location enabled, ask permission
            this.mainMap.SetView(this._viewModel.Context.UserGeoCoordinate, 16);
        }

        private void SetProgressBarState(string message, bool state)
        {
            SystemTray.ProgressIndicator.Text = message;
            SystemTray.ProgressIndicator.IsIndeterminate = state;
            SystemTray.ProgressIndicator.IsVisible = state;
        }

        // TODO: This is not MVVM friendly. Needs refactoring.
        private void transitTripsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.bottomGrid.Visibility = Visibility.Collapsed;
            this.topGrid.Visibility = Visibility.Collapsed;

            this._viewModel.Context.SelectedTransitTrip = this._viewModel.TransitDescriptionCollection[this.transitTripsList.SelectedIndex];

            TransitDescription description = TransitRequestContext.Current.SelectedTransitTrip;

            if (description != null)
            {
                int stepNumber = 0;
                foreach (var step in description.ItinerarySteps)
                {
                    stepNumber++;
                    var pushpin = new Pushpin()
                        {
                            Location = step.GeoCoordinate,
                            Content = stepNumber,
                            Style = (Style)(Application.Current.Resources["TransitStepPushpinStyle"]),
                            PositionOrigin = PositionOrigin.Center,
                            Tag = stepNumber - 1,
                        };

                    pushpin.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.pushpin_Tap);
                    this.pushpinStepsLayer.Children.Add(pushpin);
                }


                var startPushpin = new Pushpin()
                    {
                        Location = description.PathPoints[0],
                        Content = "A",
                        Style = (Style)(Application.Current.Resources["TransitEndpointPushpinStyle"]),
                        PositionOrigin = PositionOrigin.Center,
                        Tag = 0
                    };
                startPushpin.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.pushpin_Tap);

                var endPushpin = new Pushpin()
                    {
                        Location = description.PathPoints[description.PathPoints.Count - 1],
                        Content = "B",
                        Style = (Style)(Application.Current.Resources["TransitEndpointPushpinStyle"]),
                        PositionOrigin = PositionOrigin.Center,
                        Tag = stepNumber - 1
                    };
                endPushpin.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.pushpin_Tap);

                this.pushpinStepsLayer.Children.Add(startPushpin);
                this.pushpinStepsLayer.Children.Add(endPushpin);

                routePath.Locations.Clear();
                foreach (var pathPoint in description.PathPoints)
                {
                    this.routePath.Locations.Add(pathPoint);
                }

                this.mainMap.SetView(description.MapView);
            }
        }

        void pushpin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var pushpin = sender as Pushpin;
            NavigationService.Navigate(new Uri(string.Format("{0}?selectedIndex={1}", PhonePageUri.DirectionsView ,pushpin.Tag), UriKind.Relative));
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri(PhonePageUri.DirectionsView, UriKind.Relative));
        }

        ////private void Button_Click_1(object sender, RoutedEventArgs e)
        ////{
        ////    dateTimeStackPanel.Visibility = dateTimeStackPanel.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        ////}
    }
}