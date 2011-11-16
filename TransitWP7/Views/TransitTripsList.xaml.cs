using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using TransitWP7.ViewModels;

namespace TransitWP7.Views
{
    public partial class TransitTripsList : UserControl
    {
        private TransitTripsListViewModel viewModel;

        public TransitTripsList()
        {
            InitializeComponent();
            this.viewModel = new TransitTripsListViewModel();
            this.viewModel.InitializeView();
            this.DataContext = this.viewModel;
        }

        private void resultsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.viewModel.ApplyTripSelection(this.resultsList.SelectedIndex);
            //NavigationService.Navigate(new Uri("/NavigateMapPage.xaml", UriKind.Relative));
        }
    }
}
