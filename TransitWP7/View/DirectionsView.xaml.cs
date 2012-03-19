using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

using Microsoft.Phone.Controls;
using TransitWP7.ViewModel;

namespace TransitWP7.View
{
    public partial class DirectionsView : PhoneApplicationPage
    {
        private readonly DirectionsViewModel _viewModel;

        public DirectionsView()
        {
            this.InitializeComponent();
            this._viewModel = ViewModelLocator.DirectionsViewModelStatic;
        }

        private void ItemsPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            return;
        }
    }
}