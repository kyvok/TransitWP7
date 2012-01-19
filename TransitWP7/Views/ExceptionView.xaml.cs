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
using Microsoft.Phone.Shell;

namespace TransitWP7
{
    public partial class ExceptionView : PhoneApplicationPage
    {
        public ExceptionView()
        {
            InitializeComponent();
        }

        protected override void  OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.exceptionText.Text = (string)PhoneApplicationService.Current.State["exception"];
            base.OnNavigatedTo(e);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(this.exceptionText.Text);
        }
    }
}