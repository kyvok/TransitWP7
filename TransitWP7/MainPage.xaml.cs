using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace TransitWP7
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            BingMapsRestApi.BingMapsQuery.GetLocationInfo(new BingMapsRestApi.Point(47.64054, -122.12934), SampleCallbackForBingApiQuery);
            BingMapsRestApi.BingMapsQuery.GetLocationsFromQuery("Starbucks", SampleCallbackForBingApiQuery);
            BingMapsRestApi.BingMapsQuery.GetLocationsFromQueryWithUserContext("Starbucks", new BingMapsRestApi.UserContextParameters(new BingMapsRestApi.Point(47.64054, -122.12934)), SampleCallbackForBingApiQuery);
            BingMapsRestApi.BingMapsQuery.GetTransitRouteFromPoints(new BingMapsRestApi.Point(47.623192, -122.326698), new BingMapsRestApi.Point(47.60223, -122.331039), SampleCallbackForBingApiQuery);
        }

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
                    result.Result.ResourceSets[0].Resources.Length,
                    result.Result.ResourceSets[0].Resources[0].GetType().Name);
            }
        }

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