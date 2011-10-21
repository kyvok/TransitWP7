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

namespace TransitWP7
{
    public partial class DateTimeSelectionPage : PhoneApplicationPage
    {
        public DateTimeSelectionPage()
        {
            InitializeComponent();
        }



        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void AcceptButton_Click(object sender, EventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void DatePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
        }

        private void TimePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
        }
    }
}