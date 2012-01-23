using System;
using System.Device.Location;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;

namespace TransitWP7
{
    // TODO: Localize this app properly. Will need a resource file.
    public partial class MainMapView : PhoneApplicationPage
    {
        private readonly ViewModels.MainMapViewModel _viewModel = new ViewModels.MainMapViewModel();

        public MainMapView()
        {
            InitializeComponent();
            this.DataContext = this._viewModel;
            this.mainMap.CredentialsProvider = new ApplicationIdCredentialsProvider(ApiKeys.BingMapsKey);
            this.mainMap.SetView(new GeoCoordinate(39.450, -98.908), 3.3);

            RegisterNotifications();
        }

        private void RegisterNotifications()
        {
            Messenger.Default.Register<DialogMessage>(
                this,
                msg => DispatcherHelper.UIDispatcher.BeginInvoke(
                    () =>
                    {
                        var result = MessageBox.Show(msg.Content, msg.Caption, msg.Button);
                        msg.ProcessCallback(result);
                    }));

            Messenger.Default.Register<NotificationMessage>(
                this,
                msg =>
                {
                    if (msg.Notification != "transit")
                    {
                        DispatcherHelper.UIDispatcher.BeginInvoke(
                            () => NavigationService.Navigate(
                                new Uri(
                                    string.Format("/Views/LocationSelectionView.xaml?endpoint={0}", msg.Notification),
                                    UriKind.Relative)));
                    }
                    else
                    {
                        DispatcherHelper.UIDispatcher.BeginInvoke(
                            () =>
                            {
                                this.bottomGrid.Visibility = Visibility.Visible;
                                this.transitTripsList.ItemsSource = _viewModel.FormattedTransitTrips;
                                this.bottomGrid.Height = 800 - this.topGrid.ActualHeight - 32;
                            });
                    }
                });
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
                        datePicker.IsEnabled = false;
                        timePicker.IsEnabled = false;
                        break;
                    case 1:
                        this._viewModel.EnsureDateTimeSyncInContext(datePicker.Value, timePicker.Value, TimeCondition.DepartingAt);
                        datePicker.IsEnabled = true;
                        timePicker.IsEnabled = true;
                        break;
                    case 2:
                        this._viewModel.EnsureDateTimeSyncInContext(datePicker.Value, timePicker.Value, TimeCondition.ArrivingAt);
                        datePicker.IsEnabled = true;
                        timePicker.IsEnabled = true;
                        break;
                    case 3:
                        this._viewModel.EnsureDateTimeSyncInContext(datePicker.Value, timePicker.Value, TimeCondition.LastArrivalTime);
                        datePicker.IsEnabled = true;
                        timePicker.IsEnabled = false;
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
            this.mainMap.SetView(this._viewModel.Context.UserGeoCoordinate, 14);
        }

        private void transitTripsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.bottomGrid.Visibility = Visibility.Collapsed;
            this.topGrid.Visibility = Visibility.Collapsed;

            this._viewModel.Context.SelectedTransitTrip = this._viewModel.Context.TransitDescriptionCollection[this.transitTripsList.SelectedIndex];

            TransitDescription description = TransitRequestContext.Current.SelectedTransitTrip;

            if (description != null)
            {
                foreach (var step in description.ItinerarySteps)
                {
                    string instructContent = string.Empty;
                    if (step.IconType != string.Empty)
                    {
                        instructContent = step.IconType.Substring(0, 1);
                        if (step.IconType.StartsWith("B"))
                        {
                            instructContent += step.BusNumber;
                        }
                    }

                    this.pushpinStepsLayer.Children.Add(
                        new Pushpin()
                        {
                            Location = step.GeoCoordinate,
                            Content = instructContent,
                            Style = (Style)(Application.Current.Resources["TransitStepPushpinStyle"]),
                            PositionOrigin = PositionOrigin.Center
                        });
                }

                routePath.Locations.Clear();
                foreach (var pathPoint in description.PathPoints)
                {
                    this.routePath.Locations.Add(pathPoint);
                }

                this.mainMap.SetView(description.MapView);
            }
        }

        ////private void Button_Click_1(object sender, RoutedEventArgs e)
        ////{
        ////    dateTimeStackPanel.Visibility = dateTimeStackPanel.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        ////}
    }
}