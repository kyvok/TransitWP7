using System.IO.IsolatedStorage;
using GalaSoft.MvvmLight;

namespace TransitWP7.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private const string FirstLaunchDoneKey = "FirstLaunchDone";
        private const string UseLocationSettingKey = "UseLocationSetting";
        private readonly IsolatedStorageSettings _settings;

        public SettingsViewModel()
        {
#if DEBUG
            // Need to skip in design mode as this will throw IsolatedStorageException.
            if (!IsInDesignModeStatic)
#endif
            {
                this._settings = IsolatedStorageSettings.ApplicationSettings;
            }
        }

        public string FirstLaunchSetting
        {
            get
            {
                return this.GetValueOrDefault(FirstLaunchDoneKey, string.Empty);
            }

            set
            {
                if (this.AddOrUpdate(FirstLaunchDoneKey, FirstLaunchDoneKey))
                {
                    this.SaveSettings();
                    this.RaisePropertyChanged("FirstLaunchSetting");
                }
            }
        }

        public bool UseLocationSetting
        {
            get
            {
                return this.GetValueOrDefault(UseLocationSettingKey, false);
            }

            set
            {
                if (this.AddOrUpdate(UseLocationSettingKey, value))
                {
                    this.SaveSettings();
                    this.RaisePropertyChanged("UseLocationSetting");
                }
            }
        }

        private T GetValueOrDefault<T>(string key, T defaultValue)
        {
            if (this._settings.Contains(key))
            {
                return (T)this._settings[key];
            }

            return defaultValue;
        }

        private bool AddOrUpdate(string key, object value)
        {
            var valueChanged = false;
            if (this._settings.Contains(key))
            {
                if (this._settings[key] != value)
                {
                    this._settings[key] = value;
                    valueChanged = true;
                }
            }
            else
            {
                this._settings.Add(key, value);
                valueChanged = true;
            }

            return valueChanged;
        }

        private void SaveSettings()
        {
            this._settings.Save();
        }
    }
}