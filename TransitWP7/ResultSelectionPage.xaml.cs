﻿//TODO: copyright info

namespace TransitWP7
{
    using System;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using TransitWP7.BingSearchRestApi;

    public partial class ResultSelectionPage : PhoneApplicationPage
    {
        PhonebookResult[] resultSet = null;
        bool isStartResult = true;

        public ResultSelectionPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs arg)
        {
            // check to see if we have a title
            object titleText = null;
            if (PhoneApplicationService.Current.State.TryGetValue("theQuery", out titleText))
            {
                this.PageTitle.Text = (string)titleText;
            }


            // check to see if we have results
            if (PhoneApplicationService.Current.State.ContainsKey("theResultSet"))
            {
                resultSet = (PhonebookResult[])PhoneApplicationService.Current.State["theResultSet"];
                this.resultsList.ItemsSource = this.resultSet;

                // automatically select first result
                this.resultsList.SelectedIndex = 0;
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

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void AcceptButton_Click(object sender, EventArgs e)
        {
            PhoneApplicationService.Current.State["isFromResultSelection"] = true;
            PhoneApplicationService.Current.State["selectedResult"] = this.resultSet[this.resultsList.SelectedIndex];
            this.NavigationService.GoBack();
        }
    }
}