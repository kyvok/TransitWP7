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
    public partial class TransitTripDirections : UserControl
    {
        TransitTripDirectionsViewModel viewModel;

        public TransitTripDirections()
        {
            InitializeComponent();
            this.viewModel = new TransitTripDirectionsViewModel();
            this.directionsList.DataContext = viewModel.Context;
        }
    }
}
