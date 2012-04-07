namespace TransitWP7.View
{
    using System.Collections;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    public partial class DirectionStepView : UserControl, INotifyPropertyChanged
    {
        private bool _alreadyHookedScrollEvents;
        private bool _isScrollBarScrolling;
        private int _selectedItem;

        public DirectionStepView()
        {
            this.InitializeComponent();
            this._selectedItem = 0;
            this.Loaded += this.DirectionsViewLoaded;
            this.Tap += (x, y) => this.RaisePropertyChanged("SelectedItem");
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
                this._selectedItem = value;
                this.RaisePropertyChanged("SelectedItem");
                this.RaisePropertyChanged("ShowRightArrow");
                this.RaisePropertyChanged("ShowLeftArrow");

                this.AnimateSnapScrollViewer();
            }
        }

        public bool ShowLeftArrow
        {
            get
            {
                return !this.IsScrollBarScrolling && this.SelectedItem > 0;
            }
        }

        public bool ShowRightArrow
        {
            get
            {
                return !this.IsScrollBarScrolling && this.SelectedItem != this.directionsList.Items.Count - 1;
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
                    this.RaisePropertyChanged("ShowRightArrow");
                    this.RaisePropertyChanged("ShowLeftArrow");

                    // snap the scrollviewer into place. the screen is 480px.
                    if (this._isScrollBarScrolling == false)
                    {
                        var offset = this.directionsListScrollViewer.HorizontalOffset;
                        this.SelectedItem = (int)((offset + 240) / 480);
                    }
                }
            }
        }

        public void DirectionsViewLoaded(object sender, RoutedEventArgs e)
        {
            if (this._alreadyHookedScrollEvents)
            {
                return;
            }

            this._alreadyHookedScrollEvents = true;
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

        private void LeftArrow_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.SelectedItem = this.SelectedItem - 1;
        }

        private void RightArrow_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.SelectedItem = this.SelectedItem + 1;
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