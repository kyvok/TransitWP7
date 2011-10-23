﻿//TODO: copyright info

//TODO: only one option? automatically select it! No options? show walking directions!
//TODO: in results for end and start location, similar: only one option? take it!
namespace TransitWP7
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;

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
                    this.TempMessage.Text = "Error occured in query. Retry.";
                });
            }
        }

        private void DisplayTransitTripSummaries()
        {
            transitResults.Clear();

            foreach (var transitOption in TransitRequestContext.Current.TransitDescriptionCollection)
            {
                bool isWalk = false;
                var atd = new SummaryTransitData();
                foreach (var item in transitOption.ItinerarySteps)
                {
                    if (item.IconType != "")
                    {
                        atd.Steps += isWalk && item.IconType.StartsWith("W") ? "" : (atd.Steps == string.Empty ? "" : "->") + item.IconType.Substring(0, 1);
                        if (item.IconType.StartsWith("B"))
                        {
                            atd.Steps += item.BusNumber;
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
            this.Steps = string.Empty;
        }

        public string Steps { get; set; }
        public string Duration { get; set; }
        public string DepartsAt { get; set; }
        public string ArrivesAt { get; set; }
    }
}