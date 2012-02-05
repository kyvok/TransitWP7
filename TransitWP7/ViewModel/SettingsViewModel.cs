﻿using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using GalaSoft.MvvmLight;

namespace TransitWP7.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IsolatedStorageSettings _settings;
        private const string UseLocationSettingKey = "UseLocationSetting";

        public SettingsViewModel()
        {
            this._settings = IsolatedStorageSettings.ApplicationSettings;
        }

        public bool UseLocationSetting
        {
            get
            {
                return this.GetValueOrDefault(UseLocationSettingKey, true);
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

        private T GetValueOrDefault<T>(string key, T defaultValue)
        {
            if (this._settings.Contains(key))
            {
                return (T)this._settings[key];
            }

            return defaultValue;
        }

        private void SaveSettings()
        {
            this._settings.Save();
        }
    }
}