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

    public partial class MainPage : PhoneApplicationPage
    {
        private string startLocationOnFocus = null;
        private Brush startAddressColorOnFocus = null;
        private string endLocationOnFocus = null;
        private Brush endAddressColorOnFocus = null;

        private string currentAddress = "";
        private GeoLocation currentLocation = null;
        private string currentConfidence = "";

        public MainPage()
        {
            // TODO: refactor the location stuff
            InitializeComponent();
            GeoLocation.Instance.GeoWatcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(this.watcher_PositionChanged);

            // restore the last used values
            this.startAddress.Text = TransitRequestContext.Current.StartAddress;
            this.startingInput.Text = TransitRequestContext.Current.StartName;

            this.endAddress.Text = TransitRequestContext.Current.EndAddress;
            this.endingInput.Text = TransitRequestContext.Current.EndName;

            // Go an extra step in usability, auto-select the end location input!
            this.endingInput.Focus();
        }

        // Event handler for the GeoCoordinateWatcher.PositionChanged event.
        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            TransitRequestContext.Current.UserLocation = e.Position.Location;

            // Poll bing maps about the location
            ProxyQuery.GetLocationAddress(TransitRequestContext.Current.UserLocation, LocationCallback, null);
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
            //TODO: Is there an atomic swap?
            string temp = this.startingInput.Text;
            this.startingInput.Text = this.endingInput.Text;
            this.endingInput.Text = temp;

            //swap the addresses text
            string addressTemp = null;
            addressTemp = this.startAddress.Text;
            this.startAddress.Text = this.endAddress.Text;
            this.endAddress.Text = addressTemp;

            //swap the address styles
            Brush brushTemp = null;
            brushTemp = this.startAddress.Foreground;
            this.startAddress.Foreground = this.endAddress.Foreground;
            this.endAddress.Foreground = brushTemp;

            //swap the GPS locations
            GeoCoordinate locationTemp = null;
            locationTemp = TransitRequestContext.Current.StartLocation;
            TransitRequestContext.Current.StartLocation = TransitRequestContext.Current.EndLocation;
            TransitRequestContext.Current.EndLocation = locationTemp;
        }

        private void navigateButton_Click(object sender, RoutedEventArgs e)
        {
            this.theProgressBar.Visibility = Visibility.Visible;
            // call the old verify address
            this.verifyAddress_Click(sender, e);
        }

        private void startingInput_GotFocus(object sender, RoutedEventArgs e)
        {
            //select all text
            this.startingInput.SelectionStart = 0;
            this.startingInput.SelectionLength = this.startingInput.Text.Length;

            // save the old text if we got focus
            this.startLocationOnFocus = this.startingInput.Text;

            // grey out the address
            this.startAddressColorOnFocus = this.startAddress.Foreground;
            this.startAddress.Foreground = new SolidColorBrush(Colors.Gray);
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

                TransitRequestContext.Current.StartName = this.startingInput.Text;
                TransitRequestContext.Current.StartAddress = this.startAddress.Text;
            }
        }

        private void endingInput_GotFocus(object sender, RoutedEventArgs e)
        {
            //select all text
            this.endingInput.SelectionStart = 0;
            this.endingInput.SelectionLength = this.endingInput.Text.Length;

            // save the old text if we got focus
            this.endLocationOnFocus = this.endingInput.Text;

            // grey out the address
            this.endAddressColorOnFocus = this.endAddress.Foreground;
            this.endAddress.Foreground = new SolidColorBrush(Colors.Gray);
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
                this.endAddress.Text = "";
                TransitRequestContext.Current.EndName = this.endingInput.Text;
                TransitRequestContext.Current.EndAddress = this.endAddress.Text;
            }
        }

        private void verifyAddress_Click(object sender, RoutedEventArgs e)
        {
            // Let's resolve the addresses
            bool resolveEndLocationLater = false;

            // resolve the starting address if necessary
            if (this.startAddress.Text == "")
            {
                ProxyQuery.GetLocationsAndBusiness(this.startingInput.Text, TransitRequestContext.Current.UserLocation, StartingCallbackForBingApiQuery, null);

                resolveEndLocationLater = true;
            }

            // resolve the ending address if necessary
            if (this.endAddress.Text == "")
            {
                if (resolveEndLocationLater == false)
                {
                    ProxyQuery.GetLocationsAndBusiness(this.endingInput.Text, TransitRequestContext.Current.UserLocation, EndingCallbackForBingApiQuery, null);
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
                    MessageBox.Show("Could not find start location");
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

        private void UIStartingCallbackForBingApiQuery(ProxyQueryResult result)
        {
            // this is an ending result
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
                this.startingInput.Focus();
                MessageBox.Show("Could not find start location");
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
                this.endingInput.Focus();
                MessageBox.Show("Could not find end location");
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // determine if I came from the result selection page
            if ((bool)PhoneApplicationService.Current.State.ContainsKey("isFromResultSelection"))
            {
                PhoneApplicationService.Current.State.Remove("isFromResultSelection");
                this.ReturnFromResultSelection((bool)PhoneApplicationService.Current.State["isStartResult"]);

                if ((bool)PhoneApplicationService.Current.State.ContainsKey("resolveEndingLater"))
                {
                    PhoneApplicationService.Current.State.Remove("resolveEndingLater");
                    ProxyQuery.GetLocationsAndBusiness(this.endingInput.Text, TransitRequestContext.Current.UserLocation, EndingCallbackForBingApiQuery, null);
                }
            }
        }

        private void ReturnFromResultSelection(bool isStartResult)
        {
            LocationDescription result = (LocationDescription)PhoneApplicationService.Current.State["selectedResult"];

            // set some values here
            if ((bool)PhoneApplicationService.Current.State["isStartResult"] == true)
            {
                TransitRequestContext.Current.StartLocation = result.GeoCoordinate;
                this.startingInput.Text = result.DisplayName;
                this.startAddress.Text = result.Address;
                this.startAddress.Foreground = new SolidColorBrush(Colors.Green);
            }
            else
            {
                TransitRequestContext.Current.EndLocation = result.GeoCoordinate;
                this.endingInput.Text = result.DisplayName;
                this.endAddress.Text = result.Address;
                this.endAddress.Foreground = new SolidColorBrush(Colors.Green);

                // stop the progress bar
                this.theProgressBar.Visibility = System.Windows.Visibility.Collapsed;

                //remove the old callback
                GeoLocation.Instance.GeoWatcher.PositionChanged -= new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(this.watcher_PositionChanged);

                //HACK: replace this with an actual container object later
                TransitRequestContext.Current.StartLocation = TransitRequestContext.Current.StartLocation == null ? TransitRequestContext.Current.UserLocation : TransitRequestContext.Current.StartLocation;
                NavigationService.Navigate(new Uri("/SelectTransitResultPage.xaml", UriKind.Relative));
            }
        }

        private void EndingCallbackForBingApiQuery(ProxyQueryResult result)
        {
            if (result.Error != null)
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.endingInput.Focus();
                    MessageBox.Show("Could not find end location");
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