using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TransitWP7.ViewModels
{
    public class TransitTripsListViewModel
    {
        public TransitRequestContext Context { get { return TransitRequestContext.Current; } }
        public ObservableCollection<SummaryTransitData> FormattedTransitTrips { get; private set; }

        public TransitTripsListViewModel()
        {
            this.FormattedTransitTrips = new ObservableCollection<SummaryTransitData>();
        }

        public void InitializeView()
        {
            if (TransitRequestContext.Current.TransitDescriptionCollection.Count > 0)
            {
                this.DisplayTransitTripSummaries();
            }
            else
            {
                ProxyQuery.GetTransitDirections(
                    TransitRequestContext.Current.StartGeoCoordinate,
                    TransitRequestContext.Current.EndGeoCoordinate,
                    TransitRequestContext.Current.DateTime,
                    TransitRequestContext.Current.TimeType,
                    TransitRouteCalculated,
                    null);

                //TODO: Show we are calculating
               // this.TempMessage.Text = "Calculating...";
            }
        }

        private void TransitRouteCalculated(ProxyQueryResult result)
        {
            if (result.Error == null)
            {
                foreach (var item in result.TransitDescriptions)
                {
                    TransitRequestContext.Current.TransitDescriptionCollection.Add(item);
                }

                DisplayTransitTripSummaries();
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    //TODO: show we are calculating.
                    //this.TempMessage.Text = "";
                    //TODO: better error message
                    MessageBox.Show(result.Error.Message);
                });
            }
        }

        private void DisplayTransitTripSummaries()
        {
            foreach (var transitOption in TransitRequestContext.Current.TransitDescriptionCollection)
            {
                var atd = new SummaryTransitData();
                bool isWalk = false;
                for (int x = 0; x < transitOption.ItinerarySteps.Count; x++)
                {
                    bool lastStep = (x == (transitOption.ItinerarySteps.Count - 1) ? true : false);
                    ItineraryStep item = transitOption.ItinerarySteps[x];
                    if (item.IconType != "")
                    {
                        if (item.IconType.StartsWith("W"))
                        {
                            if (!isWalk)
                            {
                                Deployment.Current.Dispatcher.BeginInvoke(() =>
                                {
                                    Image img = new Image();
                                    img.Source = new BitmapImage(new Uri("images/walk_lo.png", UriKind.Relative));
                                    atd.Steps.Add(new TransitStep(lastStep ? "" : "->", img));
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

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    FormattedTransitTrips.Add(atd);
                });
            }

            //Deployment.Current.Dispatcher.BeginInvoke(() =>
            //{
                //TODO: Show calculating status.
               // this.TempMessage.Text = "";
            //});
        }

        public void ApplyTripSelection(int selectedIndex)
        {
            TransitRequestContext.Current.SelectedTransitTrip = TransitRequestContext.Current.TransitDescriptionCollection[selectedIndex];
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
