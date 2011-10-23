using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.IO;

namespace TransitWP7
{
    public static class PersistedInfo
    {
        const string filename = "persisted.xml";

        public static void Save()
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream stream = storage.CreateFile(filename);
            XmlSerializer xml = new XmlSerializer(typeof(TransitRequestContext));
            xml.Serialize(stream, TransitRequestContext.Current);
            stream.Close();
            stream.Dispose();
        }

        public static void Load()
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();

            if (storage.FileExists(filename))
            {
                IsolatedStorageFileStream stream = storage.OpenFile(filename, FileMode.Open);
                XmlSerializer xml = new XmlSerializer(typeof(TransitRequestContext));
                TransitRequestContext.Current = xml.Deserialize(stream) as TransitRequestContext;
                stream.Close();
                stream.Dispose();
            }
            else
            {
                // default values
                TransitRequestContext.Current.StartName = "My Current Location";
                TransitRequestContext.Current.StartAddress = "";

                TransitRequestContext.Current.EndName = "";
                TransitRequestContext.Current.EndAddress = "";
            }
        }
    }
}
