using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Phone.Controls;
using TransitWP7.ViewModel;

namespace TransitWP7.View
{
    public partial class DirectionStepView : UserControl, INotifyPropertyChanged
    {
        private readonly DirectionsViewModel _viewModel;
        private bool alreadyHookedScrollEvents = false;
        private bool _isScrollBarScrolling;
        private int _selectedItem;

        public DirectionStepView()
        {
            this.InitializeComponent();
            this._selectedItem = 0;
            this._viewModel = ViewModelLocator.DirectionsViewModelStatic;
            this.Loaded += new System.Windows.RoutedEventHandler(this.DirectionsView_Loaded);
            var ran = new ScrollViewerUtilities();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int SelectedItem
        {
            get
            {
                return this._selectedItem;
            }

            set
            {
                if (this._selectedItem != value)
                {
                    this._selectedItem = value;
                    this.RaisePropertyChanged("SelectedItem");
                }

                this.AnimateSnapScrollViewer();
            }
        }

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

                    // snap the scrollviewer into place. the screen is 480px.
                    if (this._isScrollBarScrolling == false)
                    {
                        var offset = this.directionsListScrollViewer.HorizontalOffset;
                        this.SelectedItem = (int)((offset + 240) / 480);
                    }
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
            var element = VisualTreeHelper.GetChild(this.directionsListScrollViewer, 0) as FrameworkElement;
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

        private void AnimateSnapScrollViewer()
        {
            ((DoubleAnimation)this.snapScrollViewer.Children[0]).From = this.directionsListScrollViewer.HorizontalOffset;
            ((DoubleAnimation)this.snapScrollViewer.Children[0]).To = 480 * this.SelectedItem;
            this.snapScrollViewer.Begin();
        }
    }

    // technique from http://marlongrech.wordpress.com/2009/09/14/how-to-set-wpf-scrollviewer-verticaloffset-and-horizontal-offset/
    // does not work though.
    public class ScrollViewerUtilities
    {
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.RegisterAttached(
            "HorizontalOffset",
            typeof(double),
            typeof(ScrollViewerUtilities),
            new PropertyMetadata(OnHorizontalOffsetChanged));

        public static double GetHorizontalOffset(DependencyObject d)
        {
            return (double)d.GetValue(HorizontalOffsetProperty);
        }

        public static void SetHorizontalOffset(DependencyObject d, double value)
        {
            d.SetValue(HorizontalOffsetProperty, value);
        }

        private static void OnHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewer = (ScrollViewer)d;
            viewer.ScrollToHorizontalOffset((double)e.NewValue);
        }
    }

    // technique from: http://blogs.msdn.com/b/delay/archive/2009/08/04/scrolling-so-smooth-like-the-butter-on-a-muffin-how-to-animate-the-horizontal-verticaloffset-properties-of-a-scrollviewer.aspx
    public class ScrollViewerOffsetMediator : FrameworkElement
    {
        public static readonly DependencyProperty ScrollViewerProperty =
            DependencyProperty.Register(
                "ScrollViewer",
                typeof(ScrollViewer),
                typeof(ScrollViewerOffsetMediator),
                new PropertyMetadata(OnScrollViewerChanged));

        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register(
                "HorizontalOffset",
                typeof(double),
                typeof(ScrollViewerOffsetMediator),
                new PropertyMetadata(OnHorizontalOffsetChanged));

        public ScrollViewer ScrollViewer
        {
            get { return (ScrollViewer)GetValue(ScrollViewerProperty); }
            set { SetValue(ScrollViewerProperty, value); }
        }

        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        public static void OnHorizontalOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var mediator = (ScrollViewerOffsetMediator)o;
            if (null != mediator.ScrollViewer)
            {
                mediator.ScrollViewer.ScrollToHorizontalOffset((double)e.NewValue);
            }
        }

        private static void OnScrollViewerChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var mediator = (ScrollViewerOffsetMediator)o;
            var scrollViewer = (ScrollViewer)e.NewValue;
            if (null != scrollViewer)
            {
                scrollViewer.ScrollToHorizontalOffset(mediator.HorizontalOffset);
            }
        }
    }
}