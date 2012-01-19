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
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Shell;


namespace TransitWP7
{
    public partial class MainMapView : PhoneApplicationPage
    {
        private readonly ViewModels.MainMapViewModel _viewModel = new ViewModels.MainMapViewModel();

        public MainMapView()
        {
            InitializeComponent();
            this.mainMap.CredentialsProvider = new ApplicationIdCredentialsProvider(ApiKeys.BingMapsKey);
            this.mainMap.SetView(new GeoCoordinate(39.450, -98.908), 3.3);
        }

        private void TextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Focus();
            }
        }

        private void InputBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var inputBox = sender as TextBox;
            inputBox.Background = new SolidColorBrush(Colors.Transparent);
            inputBox.SelectAll();
        }

        private void DatePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            _viewModel.EnsureDateTimeSyncInContext(e.NewDateTime.Value, _viewModel.Context.DateTime);
        }

        private void TimePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            _viewModel.EnsureDateTimeSyncInContext(_viewModel.Context.DateTime, e.NewDateTime.Value);
        }

        private void ListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var timeTypePicker = sender as ListPicker;
            if (timeTypePicker != null)
            {
                switch (timeTypePicker.SelectedIndex)
                {
                    case 0:
                        _viewModel.Context.DateTime = DateTime.Now;
                        _viewModel.Context.TimeType = TimeCondition.Now;
                        datePicker.Visibility = Visibility.Collapsed;
                        timePicker.Visibility = Visibility.Collapsed;
                        break;
                    case 1:
                        _viewModel.EnsureDateTimeSyncInContext(datePicker.Value, timePicker.Value);
                        _viewModel.Context.TimeType = TimeCondition.DepartingAt;
                        datePicker.Visibility = Visibility.Visible;
                        timePicker.Visibility = Visibility.Visible;
                        break;
                    case 2:
                        _viewModel.EnsureDateTimeSyncInContext(datePicker.Value, timePicker.Value);
                        _viewModel.Context.TimeType = TimeCondition.ArrivingAt;
                        datePicker.Visibility = Visibility.Visible;
                        timePicker.Visibility = Visibility.Visible;
                        break;
                    case 3:
                        _viewModel.EnsureDateTimeSyncInContext(datePicker.Value, timePicker.Value);
                        _viewModel.Context.TimeType = TimeCondition.LastArrivalTime;
                        datePicker.Visibility = Visibility.Visible;
                        timePicker.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var inputBox = sender as TextBlock;
            dateTimeStackPanel.Visibility = dateTimeStackPanel.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            inputBox.Text = dateTimeStackPanel.Visibility == Visibility.Collapsed ? "More options" : "Less options";
        }

        private bool startSolved = false;
        private bool endSolved = false;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!startSolved)
            {
                ProxyQuery.GetLocationsAndBusiness(this.startingInput.Text, TransitRequestContext.Current.UserGeoCoordinate, StartingCallbackForBingApiQuery, null);
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
                });
            }
            else
            {
                if (result.LocationDescriptions.Count == 1)
                {

                }
                else
                {
                    System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        this.queryNameTextBlock.Text = this.startingInput.Text;
                        this.resultsList.ItemsSource = result.LocationDescriptions;
                        this.bottomGrid.Height = Application.Current.Host.Content.ActualHeight - this.topGrid.ActualHeight;
                        this.bottomGrid.Visibility = Visibility.Visible;
                    });
                }
            }
        }

        private void resultsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TransitRequestContext.Current.SelectedStartingLocation = (LocationDescription)this.resultsList.Items[this.resultsList.SelectedIndex];
            this.bottomGrid.Visibility = Visibility.Collapsed;
        }

        private void TextBlock_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var temp = this.startingInput.Text;
            this.startingInput.Text = this.endingInput.Text;
            this.endingInput.Text = temp;
        }
    }
}