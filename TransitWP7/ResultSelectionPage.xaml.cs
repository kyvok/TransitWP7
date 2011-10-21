//TODO: copyright info

namespace TransitWP7
{
    using System;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using System.Collections.Generic;

    public partial class ResultSelectionPage : PhoneApplicationPage
    {
        List<LocationDescription> resultSet = null;
        bool isStartResult = true;

        public ResultSelectionPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs arg)
        {
            base.OnNavigatedTo(arg);

            // check to see if we have a title
            object titleText = null;
            if (PhoneApplicationService.Current.State.TryGetValue("theQuery", out titleText))
            {
                this.PageTitle.Text = (string)titleText;
            }


            // check to see if we have results
            if (PhoneApplicationService.Current.State.ContainsKey("theResultSet"))
            {
                resultSet = (List<LocationDescription>)PhoneApplicationService.Current.State["theResultSet"];
                this.resultsList.ItemsSource = this.resultSet;
            }

            // check to see if we're the starting result or end result
            if (PhoneApplicationService.Current.State.ContainsKey("isStartResult"))
            {
                this.isStartResult = (bool)PhoneApplicationService.Current.State["isStartResult"];
            }
            else
            {
                throw new Exception("should never be here");
            }
        }

        private void resultsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            PhoneApplicationService.Current.State["isFromResultSelection"] = true;
            PhoneApplicationService.Current.State["selectedResult"] = this.resultSet[this.resultsList.SelectedIndex];
            this.NavigationService.GoBack();
        }
    }
}