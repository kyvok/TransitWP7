using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Microsoft.Phone.Controls;
using TransitWP7.ViewModel;

namespace TransitWP7.View
{
    public partial class DirectionsView : UserControl, INotifyPropertyChanged
    {
        private readonly DirectionsViewModel _viewModel;
        private bool alreadyHookedScrollEvents = false;
        private bool _isScrollBarScrolling;

        public DirectionsView()
        {
            this.InitializeComponent();
            this._viewModel = ViewModelLocator.DirectionsViewModelStatic;
            this.Loaded += new System.Windows.RoutedEventHandler(this.DirectionsView_Loaded);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsScrollBarScrolling
        {
            get
            {
                return this._isScrollBarScrolling;
            }

            set
            {
                if (this._isScrollBarScrolling != value)
                {
                    this._isScrollBarScrolling = value;
                    this.RaisePropertyChanged("IsScrollBarScrolling");
                }
            }
        }

        public void DirectionsView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.alreadyHookedScrollEvents)
            {
                return;
            }

            this.alreadyHookedScrollEvents = true;
            FrameworkElement element = VisualTreeHelper.GetChild(this.directionsListScrollViewer, 0) as FrameworkElement;
            VisualStateGroup scrollStateGroup = null;
            if (element != null)
            {
                IList groups = VisualStateManager.GetVisualStateGroups(element);
                foreach (VisualStateGroup group in groups)
                {
                    if (group.Name == "ScrollStates")
                    {
                        scrollStateGroup = group;
                        break;
                    }
                }

                if (scrollStateGroup != null)
                {
                    scrollStateGroup.CurrentStateChanging += (s, args) => { this.IsScrollBarScrolling = args.NewState.Name == "Scrolling"; };
                }
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}