namespace TransitWP7
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Xml.Serialization;
    using TransitWP7.Model;
    using TransitWP7.Resources;

    public static class AutoCompleteDataManager
    {
        private const string AutoCompleteInfoFileName = "autocompleteentry.xml";
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(ObservableCollection<LocationDescription>));
        private static ObservableCollection<LocationDescription> _entries = new ObservableCollection<LocationDescription>();

        public static ObservableCollection<LocationDescription> AutoCompleteData
        {
            get
            {
                return _entries;
            }
        }

        public static void AddSearchStringEntry(string searchString)
        {
            AddLocationEntry(new LocationDescription { DisplayName = searchString.ToLower(CultureInfo.CurrentCulture) });
        }

        public static void AddLocationEntry(LocationDescription location)
        {
            if (location.DisplayName.Equals(SR.MyCurrentLocationText, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var matches = _entries.Where(o => o.DisplayName == location.DisplayName);

            var add = true;
            foreach (var m in matches)
            {
                // don't add recurrent search terms
                if (m.GeoCoordinate == null
                    && location.GeoCoordinate == null)
                {
                    add = false;
                    break;
                }

                // Add entry with identical names but different location
                if (m.GeoCoordinate != null
                    && location.GeoCoordinate != null
                    && m.GeoCoordinate.GetDistanceTo(location.GeoCoordinate) < 10)
                {
                    add = false;
                    break;
                }
            }

            if (add)
            {
                // Add an entry with the location set
                _entries.Add(location);
                return;
            }

            // prune excess data
            while (_entries.Count > 200)
            {
                // remove oldest entry, but not My Location
                _entries.RemoveAt(1);
            }
        }

        public static void RestoreData()
        {
            using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.FileExists(AutoCompleteInfoFileName))
                {
                    try
                    {
                        using (var stream = storage.OpenFile(AutoCompleteInfoFileName, FileMode.Open))
                        {
                            var savedEntries = Serializer.Deserialize(stream) as ObservableCollection<LocationDescription>;
                            if (savedEntries != null)
                            {
                                _entries = savedEntries;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        IsolatedStorageHelper.SafeDeleteFile(storage, AutoCompleteInfoFileName);
                        ResetData();
                    }
                }
            }
        }

        public static void ResetData()
        {
            using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                IsolatedStorageHelper.SafeDeleteFile(storage, AutoCompleteInfoFileName);

                _entries.Clear();
                _entries.Add(new LocationDescription { DisplayName = SR.MyCurrentLocationText });

                using (var stream = storage.OpenFile(AutoCompleteInfoFileName, FileMode.OpenOrCreate))
                {
                    Serializer.Serialize(stream, _entries);
                }
            }
        }

        public static void SaveData()
        {
            using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    using (var stream = storage.OpenFile(AutoCompleteInfoFileName, FileMode.Create))
                    {
                        var xml = new XmlSerializer(typeof(ObservableCollection<LocationDescription>));
                        xml.Serialize(stream, AutoCompleteData);
                    }
                }
                catch (Exception)
                {
                    IsolatedStorageHelper.SafeDeleteFile(storage, AutoCompleteInfoFileName);
                }
            }
        }
    }
}
