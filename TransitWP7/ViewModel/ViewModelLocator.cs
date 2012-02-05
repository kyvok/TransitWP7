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
            
            CreateMainMapViewModel();
            CreateLocationSelectionViewModel();
            CreateDirectionsViewModel();
            CreateSettingsViewModel();
        }

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