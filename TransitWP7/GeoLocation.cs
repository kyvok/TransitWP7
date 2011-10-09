using System;
using System.Device.Location;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TransitWP7
{
    public enum Accuracy
    {
        Low = 0,
        Medium = 1,
        High = 2
    }

    // singleton class for obtaining the current user location
    public sealed class GeoLocation
    {
        private static readonly GeoLocation instance = new GeoLocation();
        private GeoCoordinateWatcher lowGeowatcher = null;
        private GeoCoordinateWatcher mediumGeowatcher = null;
        private GeoCoordinateWatcher highGeowatcher = null;
        private GeoCoordinateWatcher current = null;

        private Accuracy accuracy = Accuracy.Low;

        private GeoLocation()
        {
            // initialize a low accuracy geowatcher
            this.mediumGeowatcher = new GeoCoordinateWatcher();
            this.mediumGeowatcher.MovementThreshold = 20; // movement threshold of 20 meters

            // initialize a default accuracy geowatcher
            this.mediumGeowatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
            this.mediumGeowatcher.MovementThreshold = 20; // movement threshold of 20 meters

            // initialize a high accuray geowatcher
            this.highGeowatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            highGeowatcher.MovementThreshold = 20; // movement threshold of 20 meters
            highGeowatcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(this.watcher_StatusChanged);

            this.current = this.lowGeowatcher;
        }

        public Accuracy Accuracy
        {
            get
            {
                return this.accuracy;
            }
            set
            {
                if (this.accuracy == value)
                {
                    return;
                }

                this.accuracy = value;
                
                switch (value)
                {
                    case Accuracy.Low:
                        this.current = this.lowGeowatcher;
                        break;
                    case Accuracy.Medium:
                        this.current = this.mediumGeowatcher;
                        break;
                    case Accuracy.High:
                        this.current = this.highGeowatcher;
                        break;
                }

                RefreshWatchers();
            }
        }

        public GeoCoordinateWatcher Current
        {
            get
            {
                return this.current;
            }
        }

        public static GeoLocation Instance
        {
            get
            {
                return instance;
            }
        }

        public void Initialize()
        {
            // asynchronous call to start the services
            switch(this.Accuracy)
            {
                case Accuracy.High:
                    this.highGeowatcher.Start();
                    goto case Accuracy.Medium;
                case Accuracy.Medium:
                    this.mediumGeowatcher.Start();
                    goto case Accuracy.Low;
                case Accuracy.Low:
                    this.lowGeowatcher.Start();
                    break;
            }
        }

        public void Shutdown()
        {
            this.highGeowatcher.Stop();
            this.mediumGeowatcher.Stop();
            this.lowGeowatcher.Stop();
        }

        private void RefreshWatchers()
        {
            this.Shutdown();
            this.Initialize();
        }

        // Event handler for the GeoCoordinateWatcher.StatusChanged event.
        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.NoData:
                    // The Location Service is working, but it cannot get location data.
                    this.RefreshWatchers();
                    break;

                case GeoPositionStatus.Ready:
                    // The Location Service is working and is receiving location data.
                    this.RefreshWatchers();
                    break;
            }
        }
    }
}
