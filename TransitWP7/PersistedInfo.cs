using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using TransitWP7.ViewModel;

namespace TransitWP7
{
    public static class PersistedInfo
    {
        private const string SavedFileName = "persisted.xml";

        public static void Save()
        {
            var storage = IsolatedStorageFile.GetUserStoreForApplication();
            var stream = storage.CreateFile(SavedFileName);
            var xml = new XmlSerializer(typeof(MainMapViewModel));
            xml.Serialize(stream, ViewModelLocator.MainMapViewModelStatic);
            stream.Close();
            stream.Dispose();
        }

        public static void Load()
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();

            if (storage.FileExists(SavedFileName))
            {
                try
                {
                    var stream = storage.OpenFile(SavedFileName, FileMode.Open);
                    var xml = new XmlSerializer(typeof(MainMapViewModel));
                    ViewModelLocator.MainMapViewModelStatic = xml.Deserialize(stream) as MainMapViewModel;
                    stream.Close();
                    stream.Dispose();
                }
                catch (Exception)
                {
                }
                finally
                {
                    storage.DeleteFile(SavedFileName);
                }
            }
        }
    }
}
