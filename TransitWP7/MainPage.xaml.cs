//TODO: copyright info

namespace TransitWP7
{
    using System;
    using System.Windows;
    using Microsoft.Phone.Controls;
    using System.Device.Location;

    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();


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

        private void swapText_Click(object sender, RoutedEventArgs e)
        {
            string temp = null;

            //TODO: Is there an atomic swap?
            temp = this.startingInput.Text;
            this.startingInput.Text = this.endingInput.Text;
            this.endingInput.Text = temp;
        }

        private void navigateButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/NavigateMapPage.xaml", UriKind.Relative));
        }
    }
}