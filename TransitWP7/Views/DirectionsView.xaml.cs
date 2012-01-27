using System;
using Microsoft.Phone.Controls;
using TransitWP7.ViewModels;

namespace TransitWP7.Views
{
    public partial class DirectionsView : PhoneApplicationPage
    {
        private readonly DirectionsViewModel _viewModel;

        public DirectionsView()
        {
            this.InitializeComponent();
            this._viewModel = new DirectionsViewModel();
            this.directionsList.DataContext = this._viewModel.Context;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs arg)
        {
            base.OnNavigatedTo(arg);

            int selectedIndex;
            if (this.NavigationContext.QueryString.ContainsKey("selectedIndex")
                && Int32.TryParse(this.NavigationContext.QueryString["selectedIndex"], out selectedIndex))
            {
                this.directionsList.SelectedIndex = selectedIndex;
            }
        }
    }
}