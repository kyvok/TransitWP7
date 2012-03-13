using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using TransitWP7.ViewModel;

namespace TransitWP7
{
    using System.Collections.Generic;

    using GalaSoft.MvvmLight;

    public static class PersistedInfo
    {
        private const string MainMapViewModelSavedInfo = "MainMapVM.xml";
        private const string DirectionsViewModelSavedInfo = "DirectionsVM.xml";
        private const string LocationSelectionViewModelSavedInfo = "LocationSelectionVM.xml";
        private const string SettingsViewModelSavedInfo = "SettingsVM.xml";

        public static void Save()
        {
            var storage = IsolatedStorageFile.GetUserStoreForApplication();
            {
                var stream = storage.CreateFile(MainMapViewModelSavedInfo);
                var xml = new XmlSerializer(typeof(MainMapViewModel));
                xml.Serialize(stream, ViewModelLocator.MainMapViewModelStatic);
                stream.Close();
                stream.Dispose();
            }

            {
                var stream = storage.CreateFile(DirectionsViewModelSavedInfo);
                var xml = new XmlSerializer(typeof(DirectionsViewModel));
                xml.Serialize(stream, ViewModelLocator.DirectionsViewModelStatic);
                stream.Close();
                stream.Dispose();
            }

            {
                var stream = storage.CreateFile(LocationSelectionViewModelSavedInfo);
                var xml = new XmlSerializer(typeof(LocationSelectionViewModel));
                xml.Serialize(stream, ViewModelLocator.LocationSelectionViewModelStatic);
                stream.Close();
                stream.Dispose();
            }

            {
                var stream = storage.CreateFile(SettingsViewModelSavedInfo);
                var xml = new XmlSerializer(typeof(SettingsViewModel));
                xml.Serialize(stream, ViewModelLocator.SettingsViewModelStatic);
                stream.Close();
                stream.Dispose();
            }
        }

        public static void Load()
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();

            if (storage.FileExists(MainMapViewModelSavedInfo)
                && storage.FileExists(DirectionsViewModelSavedInfo)
                && storage.FileExists(LocationSelectionViewModelSavedInfo)
                && storage.FileExists(SettingsViewModelSavedInfo))
            {
                try
                {
                    {
                        var stream = storage.OpenFile(MainMapViewModelSavedInfo, FileMode.Open);
                        var xml = new XmlSerializer(typeof(MainMapViewModel));
                        ViewModelLocator.MainMapViewModelStatic = xml.Deserialize(stream) as MainMapViewModel;
                        stream.Close();
                        stream.Dispose();
                    }

                    {
                        var stream = storage.OpenFile(DirectionsViewModelSavedInfo, FileMode.Open);
                        var xml = new XmlSerializer(typeof(DirectionsViewModel));
                        ViewModelLocator.DirectionsViewModelStatic = xml.Deserialize(stream) as DirectionsViewModel;
                        stream.Close();
                        stream.Dispose();
                    }

                    {
                        var stream = storage.OpenFile(LocationSelectionViewModelSavedInfo, FileMode.Open);
                        var xml = new XmlSerializer(typeof(LocationSelectionViewModel));
                        ViewModelLocator.LocationSelectionViewModelStatic = xml.Deserialize(stream) as LocationSelectionViewModel;
                        stream.Close();
                        stream.Dispose();
                    }

                    {
                        var stream = storage.OpenFile(SettingsViewModelSavedInfo, FileMode.Open);
                        var xml = new XmlSerializer(typeof(SettingsViewModel));
                        ViewModelLocator.SettingsViewModelStatic = xml.Deserialize(stream) as SettingsViewModel;
                        stream.Close();
                        stream.Dispose();
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    storage.DeleteFile(MainMapViewModelSavedInfo);
                }
            }
        }
    }
}
