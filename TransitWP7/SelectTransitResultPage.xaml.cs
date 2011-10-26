//TODO: copyright info

//TODO: only one option? automatically select it! No options? show walking directions!
//TODO: in results for end and start location, similar: only one option? take it!
namespace TransitWP7
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Phone.Controls;
    using System.Windows.Media.Imaging;

    public partial class SelectTransitResultPage : PhoneApplicationPage
    {
        List<SummaryTransitData> transitResults = new List<SummaryTransitData>();

        public SelectTransitResultPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (TransitRequestContext.Current.TransitDescriptionCollection.Count > 0)
            {
                this.DisplayTransitTripSummaries();
            }
            else
            {
                ProxyQuery.GetTransitDirections(
                    TransitRequestContext.Current.StartLocation,
                    TransitRequestContext.Current.EndLocation,
                    TransitRequestContext.Current.DateTime,
                    TransitRequestContext.Current.TimeType,
                    TransitRouteCalculated,
                    null);

                this.TempMessage.Text = "Calculating...";
            }
        }

        private void TransitRouteCalculated(ProxyQueryResult result)
        {
            if (result.Error == null)
            {
                foreach(var item in result.TransitDescriptions)
                {
                    TransitRequestContext.Current.TransitDescriptionCollection.Add(item);
                }

                DisplayTransitTripSummaries();
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.TempMessage.Text = "";
                    //TODO: better error message
                    MessageBox.Show(result.Error.Message);
                });
            }
        }

        private void DisplayTransitTripSummaries()
        {
            transitResults.Clear();

            foreach (var transitOption in TransitRequestContext.Current.TransitDescriptionCollection)
            {
                SummaryTransitData atd = new SummaryTransitData();
                bool isWalk = false;
                for (int x = 0; x < transitOption.ItinerarySteps.Count; x++)
                {
                    bool lastStep = (x == (transitOption.ItinerarySteps.Count - 1) ? true : false);
                    ItineraryStep item = transitOption.ItinerarySteps[x];
                    if (item.IconType != "")
                    {
                        if(item.IconType.StartsWith("W"))
                        {
                            if (!isWalk)
                            {
                                Deployment.Current.Dispatcher.BeginInvoke(() =>
                                {
                                    Image img = new Image();
                                    img.Source = new BitmapImage(new Uri("images/walk_lo.png", UriKind.Relative));
                                    atd.Steps.Add(new TransitStep( lastStep ? "" : "->", img));
                                });
                            }
                        }
                        else if (item.IconType.StartsWith("B"))
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                Image img = new Image();
                                img.Source = new BitmapImage(new Uri("images/bus_lo.png", UriKind.Relative));
                                atd.Steps.Add(new TransitStep(item.BusNumber + (lastStep ? "" : "->"), img));
                            });
                        }
                        isWalk = item.TravelMode.StartsWith("W") ? true : false;
                    }
                }
                atd.Duration = ((int)(transitOption.TravelDuration / 60)).ToString() + " min";
                atd.ArrivesAt = transitOption.ArrivalTime;
                atd.DepartsAt = transitOption.DepartureTime;

                transitResults.Add(atd);
            }

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                this.TempMessage.Text = "";
                this.resultsList.ItemsSource = transitResults;
            });
        }

        private void resultsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TransitRequestContext.Current.SelectedTransitTrip = TransitRequestContext.Current.TransitDescriptionCollection[this.resultsList.SelectedIndex];
            NavigationService.Navigate(new Uri("/NavigateMapPage.xaml", UriKind.Relative));
        }
    }

    //TODO: get rid of this class!
    public class SummaryTransitData
    {
        public SummaryTransitData()
        {
            this.Steps = new List<TransitStep>();
        }

        public List<TransitStep> Steps { get; set; }
        public string Duration { get; set; }
        public string DepartsAt { get; set; }
        public string ArrivesAt { get; set; }
    }

    public class TransitStep
    {
        public TransitStep(string str, Image image)
        {
            this.Str = str;
            this.Image = image;
        }

        public string Str { get; set; }
        public Image Image { get; set; }
    }
}