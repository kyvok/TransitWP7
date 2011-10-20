//TODO: copyright info

namespace TransitWP7
{
    using System;
    using System.Device.Location;
    using System.Windows;
    using System.Windows.Media;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;

    public partial class MainPage : PhoneApplicationPage
    {
        private string startLocationOnFocus = null;
        private Brush startAddressColorOnFocus = null;
        private string endLocationOnFocus = null;
        private Brush endAddressColorOnFocus = null;

        public MainPage()
        {
            // TODO: refactor the location stuff
            InitializeComponent();
            GeoLocation.Instance.GeoWatcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(this.watcher_PositionChanged);
        }

        // Event handler for the GeoCoordinateWatcher.PositionChanged event.
        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            TransitRequestContext.UserLocation = e.Position.Location;

            // Poll bing maps about the location
            ProxyQuery.GetLocationAddress(TransitRequestContext.UserLocation, LocationCallback, null);
        }

        private void LocationCallback(ProxyQueryResult result)
        {
            if (result.Error != null)
            {
                Console.WriteLine("obtained an error!");
                Console.WriteLine(result.Error.Message);
            }
            else
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    LocationDescription locationDesc = result.LocationDescriptions[0];
                    switch (locationDesc.Confidence)
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
                        locationDesc.DisplayName);
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
            locationTemp = TransitRequestContext.StartLocation;
            TransitRequestContext.StartLocation = TransitRequestContext.EndLocation;
            TransitRequestContext.EndLocation = locationTemp;
        }

        private void navigateButton_Click(object sender, RoutedEventArgs e)
        {
            //remove the old callback
            GeoLocation.Instance.GeoWatcher.PositionChanged -= new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(this.watcher_PositionChanged);

            //HACK: replace this with an actual container object later
            TransitRequestContext.StartLocation = TransitRequestContext.StartLocation == null ? TransitRequestContext.UserLocation : TransitRequestContext.StartLocation;

            NavigationService.Navigate(new Uri("/NavigateMapPage.xaml", UriKind.Relative));
        }

        private void findStart_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SelectMapLocation.xaml", UriKind.Relative));
        }

        private void findEnd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SelectMapLocation.xaml", UriKind.Relative));
        }

        private void hyperlinkButton1_Click(object sender, RoutedEventArgs e)
        {
            string value = (String)this.hyperlinkButton1.Content;
            if (value.Equals("departing at", StringComparison.InvariantCultureIgnoreCase))
            {
                this.hyperlinkButton1.Content = "arriving at";
                TransitRequestContext.TimeType = BingMapsRestApi.TimeType.Arrival;
            }
            else
            {
                this.hyperlinkButton1.Content = "departing at";
                TransitRequestContext.TimeType = BingMapsRestApi.TimeType.Departure;
            }
        }

        private void startingInput_GotFocus(object sender, RoutedEventArgs e)
        {
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
                this.startAddress.Text = "";
            }
        }

        // TODO: explore databinding this and get all this into its own class
        public bool IsTransitButtonEnabled
        {
            get
            {
                return this.startAddress.Text != "" && this.endAddress.Text != "";
            }
        }

        // TODO: explore databinding this and get all this into its own class
        public bool IsResolveButtonEnabled
        {
            get
            {
                return !this.IsTransitButtonEnabled;
            }
        }

        private void endingInput_GotFocus(object sender, RoutedEventArgs e)
        {
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
            }
        }

        private void nowButton_Click(object sender, RoutedEventArgs e)
        {
            this.datePicker1.Value = DateTime.Now;
            this.timePicker1.Value = DateTime.Now;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            // disable this button immediately to prevent multiple clicks
            // this.button1.IsEnabled = false;

            // Let's resolve the addresses
            bool resolveEndLocationLater = false;

            // resolve the starting address if necessary
            if (this.startAddress.Text == "")
            {
                ProxyQuery.GetLocationsAndBusiness(this.startingInput.Text, TransitRequestContext.UserLocation, StartingCallbackForBingApiQuery, null);

                resolveEndLocationLater = true;
            }

            // resolve the ending address if necessary
            if (this.endAddress.Text == "")
            {
                if (resolveEndLocationLater == false)
                {
                    ProxyQuery.GetLocationsAndBusiness(this.endingInput.Text, TransitRequestContext.UserLocation, EndingCallbackForBingApiQuery, null);
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
                Console.WriteLine("obtained an error!");
                Console.WriteLine(result.Error.Message);
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
                this.startingInput.Text += " -> no result";
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
                this.endingInput.Text += " -> no result";
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // determine if I came from the result selection page
            if ((bool)PhoneApplicationService.Current.State.ContainsKey("isFromResultSelection"))
            {
                PhoneApplicationService.Current.State.Remove("isFromResultSelection");
                this.ReturnFromResultSelection((bool)PhoneApplicationService.Current.State["isStartResult"]);

                if ((bool)PhoneApplicationService.Current.State.ContainsKey("resolveEndingLater"))
                {
                    PhoneApplicationService.Current.State.Remove("resolveEndingLater");
                    ProxyQuery.GetLocationsAndBusiness(this.endingInput.Text, TransitRequestContext.UserLocation, EndingCallbackForBingApiQuery, null);
                }
            }
        }

        private void ReturnFromResultSelection(bool isStartResult)
        {
            LocationDescription result = (LocationDescription)PhoneApplicationService.Current.State["selectedResult"];

            // set some values here
            if ((bool)PhoneApplicationService.Current.State["isStartResult"] == true)
            {
                TransitRequestContext.StartLocation = result.GeoCoordinate;
                this.startingInput.Text = result.DisplayName;
                this.startAddress.Text = result.Address;
                this.startAddress.Foreground = new SolidColorBrush(Colors.Green);
            }
            else
            {
                TransitRequestContext.EndLocation = result.GeoCoordinate;
                this.endingInput.Text = result.DisplayName;
                this.endAddress.Text = result.Address;
                this.endAddress.Foreground = new SolidColorBrush(Colors.Green);
            }
        }

        private void EndingCallbackForBingApiQuery(ProxyQueryResult result)
        {
            if (result.Error != null)
            {
                Console.WriteLine("obtained an error!");
                Console.WriteLine(result.Error.Message);
            }
            else
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    UIEndingCallbackForBingApiQuery(result);
                });
            }
        }
    }
}