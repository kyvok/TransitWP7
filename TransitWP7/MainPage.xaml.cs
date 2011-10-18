//TODO: copyright info

namespace TransitWP7
{
    using System;
    using System.Windows;
    using Microsoft.Phone.Controls;
    using System.Device.Location;
    using System.Windows.Media;
    using Microsoft.Phone.Shell;
    using TransitWP7.BingSearchRestApi;

    public partial class MainPage : PhoneApplicationPage
    {
        private GeoCoordinate currentLocation;
        private string startLocationOnFocus = null;
        private Brush startAddressColorOnFocus = null;
        private GeoCoordinate startCoordinate = null;
        private string endLocationOnFocus = null;
        private Brush endAddressColorOnFocus = null;
        private GeoCoordinate endCoordinate = null;

        // Constructor
        public MainPage()
        {
            //phonebook invlaid
            //"http://api.bing.net/xml.aspx?AppId=0E33FAC75BCECF26B08D540030F357E235539409&Query=starbucks&Sources=Phonebooks"
            //phonebook valid
            //"http://api.bing.net/xml.aspx?AppId=0E33FAC75BCECF26B08D540030F357E235539409&Query=starbucks&Sources=Phonebooks"

            // TODO: refactor the location stuff
            InitializeComponent();
            GeoLocation.Instance.GeoWatcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(this.watcher_PositionChanged);

            BingSearchRestApi.BingSearchQuery.GetLocationInfo("starbucks", new GeoCoordinate(47.64054, -122.12934), SampleCallbackForPhonebookQuery, null);

            /*
            BingMapsRestApi.BingMapsQuery.GetLocationInfo(
                new GeoCoordinate(47.64054, -122.12934),
                SampleCallbackForBingApiQuery);
            BingMapsRestApi.BingMapsQuery.GetLocationsFromQuery(
                "Starbucks", 
                SampleCallbackForBingApiQuery);
            BingMapsRestApi.BingMapsQuery.GetLocationsFromQuery(
                "Starbucks",
                new BingMapsRestApi.UserContextParameters(new GeoCoordinate(47.64054, -122.12934)),
                SampleCallbackForBingApiQuery);
            BingMapsRestApi.BingMapsQuery.GetTransitRoute(
                new GeoCoordinate(47.623192, -122.326698),
                new GeoCoordinate(47.60223, -122.331039),
                DateTime.Now.AddHours(-6),
                TransitWP7.BingMapsRestApi.TimeType.Departure,
                SampleCallbackForBingApiQuery);
             */
        }

        private void SampleCallbackForPhonebookQuery(BingSearchRestApi.BingSearchQueryResult result)
        {
            if (result.Error != null)
            {
                Console.WriteLine("obtained an error!");
                Console.WriteLine(result.Error.Message);
            }
            else
            {
                Console.WriteLine("obtained result!");
                Console.WriteLine("Got {0} results", result.Response.Phonebook.Total);
            }
        }

        /*
        private void SampleCallbackForBingApiQuery(BingMapsRestApi.BingMapsQueryResult result)
        {
            if (result.Error != null)
            {
                Console.WriteLine("obtained an error!");
                Console.WriteLine(result.Error.Message);
            }
            else
            {
                Console.WriteLine("obtained result!");
                Console.WriteLine("Got {0} {1} results",
                    result.Response.ResourceSets[0].Resources.Length,
                    result.Response.ResourceSets[0].Resources[0].GetType().Name);
            }
        }
        */

        // Event handler for the GeoCoordinateWatcher.PositionChanged event.
        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            this.currentLocation = e.Position.Location;

            // Poll bing maps about the location
            BingMapsRestApi.BingMapsQuery.GetLocationInfo(this.currentLocation, LocationCallback);
        }

        private void LocationCallback(BingMapsRestApi.BingMapsQueryResult result)
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
                    TransitWP7.BingMapsRestApi.Location response = (TransitWP7.BingMapsRestApi.Location)(result.Response.ResourceSets[0].Resources[0]);
                    switch (response.Confidence)
                    {
                        case BingMapsRestApi.ConfidenceLevel.High:
                            this.startAddress.Foreground = new SolidColorBrush(Colors.Green);
                            break;
                        case BingMapsRestApi.ConfidenceLevel.Medium:
                            this.startAddress.Foreground = new SolidColorBrush(Colors.Yellow);
                            break;
                        case BingMapsRestApi.ConfidenceLevel.Low:
                            this.startAddress.Foreground = new SolidColorBrush(Colors.Red);
                            break;
                    }
                    this.startAddress.Text = String.Format("Address: {0}",
                        response.Name);
                });
            }
        }


        private void swapText_Click(object sender, RoutedEventArgs e)
        {
            string temp = null;

            //TODO: Is there an atomic swap?
            temp = this.startingInput.Text;
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
        }

        private void navigateButton_Click(object sender, RoutedEventArgs e)
        {
            //remove the old callback
            GeoLocation.Instance.GeoWatcher.PositionChanged -= new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(this.watcher_PositionChanged);

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
            if (value.Equals("leaving at", StringComparison.InvariantCultureIgnoreCase))
            {
                this.hyperlinkButton1.Content = "arriving at";
            }
            else
            {
                this.hyperlinkButton1.Content = "leaving at";
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

            // resolve the starting address if necessary
            if (this.startAddress.Text == "")
            {
                BingSearchRestApi.BingSearchQuery.GetLocationInfo(this.startingInput.Text, this.currentLocation, StartingCallbackForBingApiQuery, null);
                /*
                BingMapsRestApi.BingMapsQuery.GetLocationsFromQuery(
                    this.startingInput.Text,
                    new BingMapsRestApi.UserContextParameters(this.currentLocation),
                    StartingCallbackForBingApiQuery);
                */
            }

            // resolve the ending address if necessary
            if (this.endAddress.Text == "")
            {
                BingSearchRestApi.BingSearchQuery.GetLocationInfo(this.endingInput.Text, this.currentLocation, EndingCallbackForBingApiQuery, null);
                /*
                BingMapsRestApi.BingMapsQuery.GetLocationsFromQuery(
                    this.endingInput.Text,
                    new BingMapsRestApi.UserContextParameters(this.currentLocation),
                    EndingCallbackForBingApiQuery);
                */
            }
        }

        private void StartingCallbackForBingApiQuery(BingSearchRestApi.BingSearchQueryResult result)
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

        private void UIStartingCallbackForBingApiQuery(BingSearchRestApi.BingSearchQueryResult result)
        {
            // this is an ending result
            PhoneApplicationService.Current.State["isStartResult"] = true;

            // save the query name for later
            PhoneApplicationService.Current.State["theQuery"] = this.startingInput.Text;

            // save the result set
            PhoneApplicationService.Current.State["theResultSet"] = result.Response.Phonebook.Results;
            NavigationService.Navigate(new Uri("/ResultSelectionPage.xaml", UriKind.Relative));

            //                // just use the first result
            //    Console.WriteLine("obtained result!");
            //    Console.WriteLine("Got {0} {1} results",
            //        result.Response.ResourceSets[0].Resources.Length,
            //        result.Response.ResourceSets[0].Resources[0].GetType().Name);
            //}

            //TransitWP7.BingMapsRestApi.Location response = (TransitWP7.BingMapsRestApi.Location)(result.Response.ResourceSets[0].Resources[0]);
            //this.startCoordinate = new GeoCoordinate(response.Point.Latitude, response.Point.Longitude);
            //System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            //{
            //    switch (response.Confidence)
            //    {
            //        case BingMapsRestApi.ConfidenceLevel.High:
            //            this.startAddress.Foreground = new SolidColorBrush(Colors.Green);
            //            break;
            //        case BingMapsRestApi.ConfidenceLevel.Medium:
            //            this.startAddress.Foreground = new SolidColorBrush(Colors.Yellow);
            //            break;
            //        case BingMapsRestApi.ConfidenceLevel.Low:
            //            this.startAddress.Foreground = new SolidColorBrush(Colors.Red);
            //            break;
            //    }
            //    this.startAddress.Text = response.Address.FormattedAddress;
            //}
            //);
        }

        private void UIEndingCallbackForBingApiQuery(BingSearchRestApi.BingSearchQueryResult result)
        {
            // this is an ending result
            PhoneApplicationService.Current.State["isStartResult"] = false;

            // save the query name for later
            PhoneApplicationService.Current.State["theQuery"] = this.endingInput.Text;

            // save the result set
            PhoneApplicationService.Current.State["theResultSet"] = result.Response.Phonebook.Results;
            NavigationService.Navigate(new Uri("/ResultSelectionPage.xaml", UriKind.Relative));

            // just use the first result
            //Console.WriteLine("obtained result!");
            //Console.WriteLine("Got {0} {1} results",
            //    result.Response.ResourceSets[0].Resources.Length,
            //    result.Response.ResourceSets[0].Resources[0].GetType().Name);

            //TransitWP7.BingMapsRestApi.Location response = (TransitWP7.BingMapsRestApi.Location)(result.Response.ResourceSets[0].Resources[0]);
            //this.endCoordinate = new GeoCoordinate(response.Point.Latitude, response.Point.Longitude);
            //System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            //    {
            //        switch (response.Confidence)
            //        {
            //            case BingMapsRestApi.ConfidenceLevel.High:
            //                this.endAddress.Foreground = new SolidColorBrush(Colors.Green);
            //                break;
            //            case BingMapsRestApi.ConfidenceLevel.Medium:
            //                this.endAddress.Foreground = new SolidColorBrush(Colors.Yellow);
            //                break;
            //            case BingMapsRestApi.ConfidenceLevel.Low:
            //                this.endAddress.Foreground = new SolidColorBrush(Colors.Red);
            //                break;
            //        }
            //        this.endAddress.Text = response.Address.FormattedAddress;
            //    }
            //);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // determine if I came from the result selection page
            if ((bool)PhoneApplicationService.Current.State.ContainsKey("isFromResultSelection"))
            {
                PhoneApplicationService.Current.State.Remove("isFromResultSelection");
                this.ReturnFromResultSelection((bool)PhoneApplicationService.Current.State["isStartResult"]);
            }
        }

        private void ReturnFromResultSelection(bool isStartResult)
        {
            PhonebookResult result = (PhonebookResult) PhoneApplicationService.Current.State["selectedResult"];
            
            // set some values here
            if ((bool)PhoneApplicationService.Current.State["isStartResult"] == true)
            {
                this.startCoordinate = new GeoCoordinate(result.Latitude, result.Longitude);
                this.startingInput.Text = result.Business;
                this.startAddress.Text = result.Address;
                this.startAddress.Foreground = new SolidColorBrush(Colors.Green);
            }
            else
            {
                this.endCoordinate = new GeoCoordinate(result.Latitude, result.Longitude);
                this.endingInput.Text = result.Business;
                this.endAddress.Text = result.Address;
                this.endAddress.Foreground = new SolidColorBrush(Colors.Green);
            }
        }

        private void EndingCallbackForBingApiQuery(BingSearchRestApi.BingSearchQueryResult result)
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