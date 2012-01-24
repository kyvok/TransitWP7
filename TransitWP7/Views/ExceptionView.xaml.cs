namespace TransitWP7
{
    using System.Windows;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;

    public partial class ExceptionView : PhoneApplicationPage
    {
        public ExceptionView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.exceptionText.Text = (string)PhoneApplicationService.Current.State["exception"];
            base.OnNavigatedTo(e);
        }

        private void CopyToClipboardClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(this.exceptionText.Text);
        }
    }
}