//TODO: copyright info

namespace TransitWP7
{
    using System;
    using System.Device.Location;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;

    public enum NeedToResolve
    {
        None = 0x0,
        Start = 0x1,
        End = 0x2,
        StartAndEnd = 0x3
    }

    public partial class MainPage : PhoneApplicationPage
    {
        private ViewModels.MainPageViewModel viewModel;

        private string startLocationOnFocus = null;
        private Brush startAddressColorOnFocus = null;
        private string endLocationOnFocus = null;
        private Brush endAddressColorOnFocus = null;

        private string currentAddress = "";
        private string currentConfidence = "";

        public NeedToResolve NeedToResolve
        {
            get
            {
                int ret = 0;
                if (this.startAddress.Text == "")
                    ret |= (int)NeedToResolve.Start;
                if (this.endAddress.Text == "")
                    ret |= (int)NeedToResolve.End;

                return (NeedToResolve)ret;
            }
        }

        public MainPage()
        {
            InitializeComponent();
            this.viewModel = new ViewModels.MainPageViewModel();

            this.DataContext = this.viewModel.Context;

            GeoLocation.Instance.GeoWatcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(this.watcher_PositionChanged);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.New)
            {
                // Go an extra step in usability, auto-select the end location input!
                this.endingInput.Focus();
            }

            // determine if I came from the result selection page
            if ((bool)PhoneApplicationService.Current.State.ContainsKey("isFromResultSelection"))
            {
                PhoneApplicationService.Current.State.Remove("isFromResultSelection");
                this.ReturnFromResultSelection((bool)PhoneApplicationService.Current.State["isStartResult"]);

                if ((bool)PhoneApplicationService.Current.State.ContainsKey("resolveEndingLater"))
                {
                    PhoneApplicationService.Current.State.Remove("resolveEndingLater");
                    ProxyQuery.GetLocationsAndBusiness(this.endingInput.Text, TransitRequestContext.Current.UserGeoCoordinate, EndingCallbackForBingApiQuery, null);
                }
            }
        }

        // Event handler for the GeoCoordinateWatcher.PositionChanged event.
        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            TransitRequestContext.Current.UserGeoCoordinate = e.Position.Location;

            // Poll bing maps about the location
            ProxyQuery.GetLocationAddress(TransitRequestContext.Current.UserGeoCoordinate, LocationCallback, null);
        }

        private void LocationCallback(ProxyQueryResult result)
        {
            if (result.Error != null)
            {
                MessageBox.Show(result.Error.Message, "LocationCallback obtained an error!", MessageBoxButton.OK);
            }
            else
            {
                LocationDescription locationDesc = result.LocationDescriptions[0];
                this.currentAddress = locationDesc.DisplayName;
                this.currentConfidence = locationDesc.Confidence;
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    ImageBrush image = new ImageBrush();
                    image.ImageSource = (ImageSource)new ImageSourceConverter().ConvertFromString(LocationImage.GetImagePath(locationDesc.StateOrProvince));
                    this.LayoutRoot.Background = image;

                    if (this.startingInput.Text == Globals.MyCurrentLocationText)
                    {
                        switch (this.currentConfidence)
                        {
                            case "High":
                                this.startAddress.Foreground = new SolidColorBrush(Colors.Green);
                                break;
                            case "Medium":
                                this.startAddress.Foreground = new SolidColorBrush(Colors.Yellow);
                                break;
                            case "Low":
                                this.startAddress.Foreground = new SolidColorBrush(Colors.Red);
                                break;
                        }
                        this.startAddress.Text = String.Format("Address: {0}",
                            this.currentAddress);
                    }
                });
            }
        }

        private void swapText_Click(object sender, RoutedEventArgs e)
        {
            this.viewModel.SwapEndStartLocations();

            //swap the address styles
            Brush brushTemp = null;
            brushTemp = this.startAddress.Foreground;
            this.startAddress.Foreground = this.endAddress.Foreground;
            this.endAddress.Foreground = brushTemp;

            //swap the GPS locations
            GeoCoordinate locationTemp = null;
            locationTemp = TransitRequestContext.Current.StartGeoCoordinate;
            TransitRequestContext.Current.StartGeoCoordinate = TransitRequestContext.Current.EndGeoCoordinate;
            TransitRequestContext.Current.EndGeoCoordinate = locationTemp;
        }

        private void navigateButton_Click(object sender, RoutedEventArgs e)
        {
            this.theProgressBar.Visibility = Visibility.Visible;
            this.navigateButton.IsEnabled = false;

            //remove old result, we are starting a new search!
            TransitRequestContext.Current.SelectedTransitTrip = null;
            TransitRequestContext.Current.TransitDescriptionCollection.Clear();
            TransitRequestContext.Current.StartingLocationDescriptionCollection.Clear();
            TransitRequestContext.Current.EndingLocationDescriptionCollection.Clear();

            // call the old verify address
            if (this.NeedToResolve == TransitWP7.NeedToResolve.None)
            {
                MoveToTransitSelection();
            }
            else
            {
                this.verifyAddress_Click(sender, e);
            }
        }

        private void startingInput_GotFocus(object sender, RoutedEventArgs e)
        {
            var inputBox = sender as TextBox;
            inputBox.SelectAll();

            // save the old text if we got focus
            this.startLocationOnFocus = this.startingInput.Text;

            // grey out the address
            this.startAddressColorOnFocus = this.startAddress.Foreground;
            this.startAddress.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void endingInput_GotFocus(object sender, RoutedEventArgs e)
        {
            var inputBox = sender as TextBox;
            inputBox.SelectAll();

            // save the old text if we got focus
            this.endLocationOnFocus = this.endingInput.Text;

            // grey out the address
            this.endAddressColorOnFocus = this.endAddress.Foreground;
            this.endAddress.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void startingInput_LostFocus(object sender, RoutedEventArgs e)
        {
            // check to see if we need to clear address bar
            if (this.startingInput.Text == this.startLocationOnFocus)
            {
                this.startAddress.Foreground = this.startAddressColorOnFocus;
            }
            else
            {
                // use empty as current location
                if (this.startingInput.Text == "")
                {
                    this.startingInput.Text = Globals.MyCurrentLocationText;
                    this.startAddress.Text = String.Format("Address: {0}", this.currentAddress);
                }
                else
                {
                    this.startAddress.Text = "";
                }
            }
        }

        private void endingInput_LostFocus(object sender, RoutedEventArgs e)
        {
            // check to see if we need to clear address bar
            if (this.endingInput.Text == this.endLocationOnFocus)
            {
                this.endAddress.Foreground = this.endAddressColorOnFocus;
            }
            else
            {
                // use empty as current location
                if (this.endingInput.Text == "")
                {
                    this.endingInput.Text = Globals.MyCurrentLocationText;
                    this.endAddress.Text = String.Format("Address: {0}", this.currentAddress);
                }
                else
                {
                    this.endAddress.Text = "";
                }
            }
        }

        private void verifyAddress_Click(object sender, RoutedEventArgs e)
        {
            // Let's resolve the addresses
            bool resolveEndLocationLater = false;

            // resolve the starting address if necessary
            if (this.startAddress.Text == "")
            {
                ProxyQuery.GetLocationsAndBusiness(this.startingInput.Text, TransitRequestContext.Current.UserGeoCoordinate, StartingCallbackForBingApiQuery, null);

                resolveEndLocationLater = true;
            }

            // resolve the ending address if necessary
            if (this.endAddress.Text == "")
            {
                if (resolveEndLocationLater == false)
                {
                    ProxyQuery.GetLocationsAndBusiness(this.endingInput.Text, TransitRequestContext.Current.UserGeoCoordinate, EndingCallbackForBingApiQuery, null);
                }
                else
                {
                    PhoneApplicationService.Current.State["resolveEndingLater"] = true;
                }
            }
        }

        private void StartingCallbackForBingApiQuery(ProxyQueryResult result)
        {
            if (result.Error != null)
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.startingInput.Focus();
                    MessageBox.Show(result.Error.Message);
                    this.theProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                    this.navigateButton.IsEnabled = true;
                });
            }
            else
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    UIStartingCallbackForBingApiQuery(result);
                });
            }
        }

        private void EndingCallbackForBingApiQuery(ProxyQueryResult result)
        {
            if (result.Error != null)
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.theProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                    this.navigateButton.IsEnabled = true;
                    this.endingInput.Focus();
                    MessageBox.Show(result.Error.Message);
                });
            }
            else
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    UIEndingCallbackForBingApiQuery(result);
                });
            }
        }

        private void UIStartingCallbackForBingApiQuery(ProxyQueryResult result)
        {
            // this is a starting result
            PhoneApplicationService.Current.State["isStartResult"] = true;

            // save the query name for later
            PhoneApplicationService.Current.State["theQuery"] = this.startingInput.Text;

            if (result.Error == null)
            {
                // save the result set
                PhoneApplicationService.Current.State["theResultSet"] = result.LocationDescriptions;
                NavigationService.Navigate(new Uri("/ResultSelectionPage.xaml", UriKind.Relative));
            }
            else
            {
                this.theProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                this.navigateButton.IsEnabled = true;
                this.startingInput.Focus();
                MessageBox.Show(result.Error.Message);
            }
        }

        private void UIEndingCallbackForBingApiQuery(ProxyQueryResult result)
        {
            // this is an ending result
            PhoneApplicationService.Current.State["isStartResult"] = false;

            // save the query name for later
            PhoneApplicationService.Current.State["theQuery"] = this.endingInput.Text;

            if (result.Error == null)
            {
                // save the result set
                PhoneApplicationService.Current.State["theResultSet"] = result.LocationDescriptions;
                NavigationService.Navigate(new Uri("/ResultSelectionPage.xaml", UriKind.Relative));
            }
            else
            {
                this.theProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                this.navigateButton.IsEnabled = true;
                this.endingInput.Focus();
                MessageBox.Show(result.Error.Message);
            }
        }

        private void ReturnFromResultSelection(bool isStartResult)
        {
            LocationDescription result = (LocationDescription)PhoneApplicationService.Current.State["selectedResult"];
            PhoneApplicationService.Current.State.Remove("isStartResult");

            // set some values here
            if (isStartResult)
            {
                if (this.NeedToResolve == TransitWP7.NeedToResolve.Start)
                {
                    TransitRequestContext.Current.StartGeoCoordinate = result.GeoCoordinate;
                    this.startingInput.Text = result.DisplayName;
                    this.startAddress.Text = result.Address;
                    this.startAddress.Foreground = new SolidColorBrush(Colors.Green);
                    MoveToTransitSelection();
                }
            }
            else
            {
                TransitRequestContext.Current.EndGeoCoordinate = result.GeoCoordinate;
                this.endingInput.Text = result.DisplayName;
                this.endAddress.Text = result.Address;
                this.endAddress.Foreground = new SolidColorBrush(Colors.Green);
                MoveToTransitSelection();
            }
        }

        private void MoveToTransitSelection()
        {
            // stop the progress bar
            this.theProgressBar.Visibility = System.Windows.Visibility.Collapsed;

            //remove the old callback
            GeoLocation.Instance.GeoWatcher.PositionChanged -= new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(this.watcher_PositionChanged);

            //HACK: replace this with an actual container object later
            TransitRequestContext.Current.StartGeoCoordinate = TransitRequestContext.Current.StartGeoCoordinate == null ? TransitRequestContext.Current.UserGeoCoordinate : TransitRequestContext.Current.StartGeoCoordinate;
            //NavigationService.Navigate(new Uri("/SelectTransitResultPage.xaml", UriKind.Relative));
            NavigationService.Navigate(new Uri("/TransitResultsPivotPage.xaml", UriKind.Relative));
        }

        private void TextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Focus();
            }
        }

        private void DatePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            DateTime baseTime = TransitRequestContext.Current.DateTime;
            DateTime composingTime = e.NewDateTime.Value;

            TransitRequestContext.Current.DateTime = new DateTime(
                composingTime.Year,
                composingTime.Month,
                composingTime.Day,
                baseTime.Hour,
                baseTime.Minute,
                baseTime.Second);
        }

        private void TimePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            DateTime baseTime = TransitRequestContext.Current.DateTime;
            DateTime composingTime = e.NewDateTime.Value;

            TransitRequestContext.Current.DateTime = new DateTime(
                baseTime.Year,
                baseTime.Month,
                baseTime.Day,
                composingTime.Hour,
                composingTime.Minute,
                composingTime.Second);
        }

        private void ListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var timeTypePicker = sender as ListPicker;
            if (timeTypePicker != null)
            {
                switch (timeTypePicker.SelectedIndex)
                {
                    case 0:
                        TransitRequestContext.Current.DateTime = DateTime.Now;
                        TransitRequestContext.Current.TimeType = TimeCondition.Now;
                        dateTimeStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case 1:
                        this.EnsureDateTimeSyncInContext();
                        TransitRequestContext.Current.TimeType = TimeCondition.DepartingAt;
                        dateTimeStackPanel.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case 2:
                        this.EnsureDateTimeSyncInContext();
                        TransitRequestContext.Current.TimeType = TimeCondition.ArrivingAt;
                        dateTimeStackPanel.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case 3:
                        TransitRequestContext.Current.DateTime = DateTime.Now;
                        TransitRequestContext.Current.TimeType = TimeCondition.LastArrivalTime;
                        dateTimeStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    default:
                        break;
                }
            }
        }

        private void EnsureDateTimeSyncInContext()
        {
            TransitRequestContext.Current.DateTime = new DateTime(
                datePicker.Value.Value.Year,
                datePicker.Value.Value.Month,
                datePicker.Value.Value.Day,
                timePicker.Value.Value.Hour,
                timePicker.Value.Value.Minute,
                timePicker.Value.Value.Second
                );
        }
    }
}