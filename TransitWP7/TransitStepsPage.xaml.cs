//TODO: copyright info

namespace TransitWP7
{
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
    using Microsoft.Phone.Controls;

    public partial class TransitStepsPage : PhoneApplicationPage
    {
        private TransitDescription transitDesc;

        public TransitStepsPage()
        {
            InitializeComponent();

            this.ShowTransitStepList();
        }

        // 
        private void ShowTransitStepList()
        {
            this.resultsList.ItemsSource = transitDesc.ItinerarySteps;
        }
    }
}