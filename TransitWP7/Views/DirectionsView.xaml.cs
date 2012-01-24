using Microsoft.Phone.Controls;
using TransitWP7.ViewModels;

namespace TransitWP7.Views
{
    public partial class DirectionsView : PhoneApplicationPage
    {
        private readonly DirectionsViewModel _viewModel;

        public DirectionsView()
        {
            InitializeComponent();
            this._viewModel = new DirectionsViewModel();
            this.directionsList.DataContext = _viewModel.Context;
        }
    }
}