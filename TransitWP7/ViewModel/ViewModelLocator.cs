/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:TransitWP7"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;
using BingApisLib.BingMapsRestApi;
using BingApisLib.BingSearchRestApi;
using GalaSoft.MvvmLight;

namespace TransitWP7.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        private static DirectionsViewModel _directionsViewModel;
        private static MainMapViewModel _mainMapViewModel;
        private static LocationSelectionViewModel _locationSelectionViewModel;
        private static SettingsViewModel _settingsViewModel;

        public ViewModelLocator()
        {
            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time services and viewmodels
            ////}
            ////else
            ////{
            ////    // Create run time services and view models
            ////}

            LocDescs = new ObservableCollection<LocationDescription>();
            BusDescs = new ObservableCollection<TransitDescription>();

            var bingMapsResponseSerializer = new XmlSerializer(typeof(Response));
            var bingSearchResponseSerializer = new XmlSerializer(typeof(SearchResponse));
            var bmlocresx = new Uri("TestData/bingmapslocations.xml", UriKind.Relative);
            var bmbusresx = new Uri("TestData/bingmapstransit.xml", UriKind.Relative);
            var bsphoresx = new Uri("TestData/bingservicephonebook.xml", UriKind.Relative);
            var sr1 = new StreamReader(System.Windows.Application.GetResourceStream(bmlocresx).Stream);
            var locRsp = (Response)bingMapsResponseSerializer.Deserialize(sr1);
            foreach (Location location in locRsp.ResourceSets[0].Resources)
            {
                LocDescs.Add(new LocationDescription(location));
            }

            sr1.Dispose();

            var sr2 = new StreamReader(System.Windows.Application.GetResourceStream(bsphoresx).Stream);
            var phoRsp = (SearchResponse)bingSearchResponseSerializer.Deserialize(sr2);
            foreach (PhonebookResult phone in phoRsp.Phonebook.Results)
            {
                LocDescs.Add(new LocationDescription(phone));
            }

            sr2.Dispose();

            var sr3 = new StreamReader(System.Windows.Application.GetResourceStream(bmbusresx).Stream);
            var busRsp = (Response)bingMapsResponseSerializer.Deserialize(sr3);
            foreach (Route route in busRsp.ResourceSets[0].Resources)
            {
                BusDescs.Add(new TransitDescription(route));
            }

            sr3.Dispose();

            CreateMainMapViewModel();
            CreateLocationSelectionViewModel();
            CreateDirectionsViewModel();
            CreateSettingsViewModel();
        }

        public static ObservableCollection<LocationDescription> LocDescs { get; set; }

        public static ObservableCollection<TransitDescription> BusDescs { get; set; }

        public static MainMapViewModel MainMapViewModelStatic
        {
            get
            {
                if (_mainMapViewModel == null)
                {
                    CreateMainMapViewModel();
                }

                return _mainMapViewModel;
            }
        }

        public static LocationSelectionViewModel LocationSelectionViewModelStatic
        {
            get
            {
                if (_locationSelectionViewModel == null)
                {
                    CreateLocationSelectionViewModel();
                }

                return _locationSelectionViewModel;
            }
        }

        public static DirectionsViewModel DirectionsViewModelStatic
        {
            get
            {
                if (_directionsViewModel == null)
                {
                    CreateDirectionsViewModel();
                }

                return _directionsViewModel;
            }
        }

        public static SettingsViewModel SettingsViewModelStatic
        {
            get
            {
                if (_settingsViewModel == null)
                {
                    CreateSettingsViewModel();
                }

                return _settingsViewModel;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainMapViewModel MainMapViewModel
        {
            get
            {
                return MainMapViewModelStatic;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public LocationSelectionViewModel LocationSelectionViewModel
        {
            get
            {
                return LocationSelectionViewModelStatic;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public DirectionsViewModel DirectionsViewModel
        {
            get
            {
                return DirectionsViewModelStatic;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SettingsViewModel SettingsViewModel
        {
            get
            {
                return SettingsViewModelStatic;
            }
        }

        public static void CreateMainMapViewModel()
        {
            if (_mainMapViewModel == null)
            {
                _mainMapViewModel = new MainMapViewModel();
            }
        }

        public static void CreateLocationSelectionViewModel()
        {
            if (_locationSelectionViewModel == null)
            {
                _locationSelectionViewModel = new LocationSelectionViewModel();
            }
        }

        public static void CreateDirectionsViewModel()
        {
            if (_directionsViewModel == null)
            {
                _directionsViewModel = new DirectionsViewModel();
            }
        }

        public static void CreateSettingsViewModel()
        {
            if (_settingsViewModel == null)
            {
                _settingsViewModel = new SettingsViewModel();
            }
        }

        public static void ClearMainMapViewModel()
        {
            _mainMapViewModel.Cleanup();
            _mainMapViewModel = null;
        }

        public static void ClearLocationSelectionViewModel()
        {
            _locationSelectionViewModel.Cleanup();
            _locationSelectionViewModel = null;
        }

        public static void ClearDirectionsViewModel()
        {
            _directionsViewModel.Cleanup();
            _directionsViewModel = null;
        }

        public static void ClearSettingsViewModel()
        {
            _settingsViewModel.Cleanup();
            _settingsViewModel = null;
        }

        public static void Cleanup()
        {
            ClearDirectionsViewModel();
            ClearMainMapViewModel();
            ClearLocationSelectionViewModel();
            ClearSettingsViewModel();
        }
    }
}