namespace TransitWP7
{
    using System;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Xml.Serialization;
    using TransitWP7.ViewModel;

    public static class PersistedInfo
    {
        private const string MainMapViewModelSavedInfo = "MainMapVM.xml";
        private const string DirectionsViewModelSavedInfo = "DirectionsVM.xml";
        private const string LocationSelectionViewModelSavedInfo = "LocationSelectionVM.xml";
        ////private const string SettingsViewModelSavedInfo = "SettingsVM.xml";

        public static void Save()
        {
            using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                SaveViewModel(storage, MainMapViewModelSavedInfo, ViewModelLocator.MainMapViewModelStatic);
                SaveViewModel(storage, DirectionsViewModelSavedInfo, ViewModelLocator.DirectionsViewModelStatic);
                SaveViewModel(storage, LocationSelectionViewModelSavedInfo, ViewModelLocator.LocationSelectionViewModelStatic);
                ////SaveViewModel(storage, SettingsViewModelSavedInfo, ViewModelLocator.SettingsViewModelStatic);
            }

            AutoCompleteDataManager.SaveData();
        }

        public static void Load()
        {
            using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.FileExists(MainMapViewModelSavedInfo)
                    && storage.FileExists(DirectionsViewModelSavedInfo)
                    && storage.FileExists(LocationSelectionViewModelSavedInfo)
                    /*&& storage.FileExists(SettingsViewModelSavedInfo)*/)
                {
                    try
                    {
                        ViewModelLocator.MainMapViewModelStatic = LoadViewModel<MainMapViewModel>(storage, MainMapViewModelSavedInfo);
                        ViewModelLocator.DirectionsViewModelStatic = LoadViewModel<DirectionsViewModel>(storage, DirectionsViewModelSavedInfo);
                        ViewModelLocator.LocationSelectionViewModelStatic = LoadViewModel<LocationSelectionViewModel>(storage, LocationSelectionViewModelSavedInfo);
                        ////ViewModelLocator.SettingsViewModelStatic = LoadViewModel<SettingsViewModel>(storage, SettingsViewModelSavedInfo);
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        IsolatedStorageHelper.SafeDeleteFile(storage, MainMapViewModelSavedInfo);
                        IsolatedStorageHelper.SafeDeleteFile(storage, DirectionsViewModelSavedInfo);
                        IsolatedStorageHelper.SafeDeleteFile(storage, LocationSelectionViewModelSavedInfo);
                        ////SafeDeleteFile(storage, SettingsViewModelStatic);
                    }
                }
            }

            AutoCompleteDataManager.RestoreData();
        }

        private static T LoadViewModel<T>(IsolatedStorageFile storage, string file)
        {
            using (var stream = storage.OpenFile(file, FileMode.Open))
            {
                var xml = new XmlSerializer(typeof(T));
                return (T)xml.Deserialize(stream);
            }
        }

        private static void SaveViewModel<T>(IsolatedStorageFile storage, string file, T viewModel)
        {
            using (var stream = storage.CreateFile(file))
            {
                var xml = new XmlSerializer(typeof(T));
                xml.Serialize(stream, viewModel);
            }
        }
    }
}
