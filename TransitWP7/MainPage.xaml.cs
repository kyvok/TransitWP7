//TODO: copyright info

namespace TransitWP7
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;

    //public enum NeedToResolve
    //{
    //    None = 0x0,
    //    Start = 0x1,
    //    End = 0x2,
    //    StartAndEnd = 0x3
    //}

    public partial class MainPage : PhoneApplicationPage
    {
        private ViewModels.MainPageViewModel viewModel;

        //public NeedToResolve NeedToResolve
        //{
        //    get
        //    {
        //        int ret = 0;
        //        if (this.startAddress.Text == "")
        //            ret |= (int)NeedToResolve.Start;
        //        if (this.endAddress.Text == "")
        //            ret |= (int)NeedToResolve.End;

        //        return (NeedToResolve)ret;
        //    }
        //}

        public MainPage()
        {
            InitializeComponent();
            this.viewModel = new ViewModels.MainPageViewModel();

            this.DataContext = this.viewModel;
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

        private void swapText_Click(object sender, RoutedEventArgs e)
        {
            this.viewModel.SwapEndStartLocations();
        }

        private void navigateButton_Click(object sender, RoutedEventArgs e)
        {
            this.theProgressBar.Visibility = Visibility.Visible;

            //remove old result, we are starting a new search!
            TransitRequestContext.Current.SelectedTransitTrip = null;
            TransitRequestContext.Current.TransitDescriptionCollection.Clear();
            TransitRequestContext.Current.StartingLocationDescriptionCollection.Clear();
            TransitRequestContext.Current.EndingLocationDescriptionCollection.Clear();

            // call the old verify address
            //if (this.NeedToResolve == TransitWP7.NeedToResolve.None)
            //{
            //    MoveToTransitSelection();
            //}
            //else
            //{
                this.verifyAddress_Click(sender, e);
            //}
        }

        private void InputBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var inputBox = sender as TextBox;
            inputBox.SelectAll();
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
                NavigationService.Navigate(new Uri("/LocationSelectionView.xaml", UriKind.Relative));
            }
            else
            {
                this.theProgressBar.Visibility = System.Windows.Visibility.Collapsed;
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
                NavigationService.Navigate(new Uri("/LocationSelectionView.xaml", UriKind.Relative));
            }
            else
            {
                this.theProgressBar.Visibility = System.Windows.Visibility.Collapsed;
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
                this.viewModel.Context.SelectedStartingLocation = result;
                this.viewModel.StartName = result.DisplayName;
                this.viewModel.StartAddress = result.FormattedAddress;

                //if (this.NeedToResolve == TransitWP7.NeedToResolve.Start)
                //{
                //    MoveToTransitSelection();
                //}
            }
            else
            {
                this.viewModel.Context.SelectedEndingLocation = result;
                this.viewModel.EndName = result.DisplayName;
                this.viewModel.EndAddress = result.FormattedAddress;

                MoveToTransitSelection();
            }
        }

        private void MoveToTransitSelection()
        {
            // stop the progress bar
            this.theProgressBar.Visibility = System.Windows.Visibility.Collapsed;

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

    public class ConfidenceToBrushConverter : IValueConverter
    {
        private static SolidColorBrush greenBrush = new SolidColorBrush(Colors.Green);
        private static SolidColorBrush yellowBrush = new SolidColorBrush(Colors.Yellow);
        private static SolidColorBrush redBrush = new SolidColorBrush(Colors.Red);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string confidenceLevel = (string)value;
            switch (confidenceLevel)
            {
                case "High":
                    return greenBrush;
                case "Medium":
                    return yellowBrush;
                case "Low":
                    return redBrush;
                default:
                    return redBrush;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}