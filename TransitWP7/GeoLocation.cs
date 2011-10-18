//TODO: copyright info

namespace TransitWP7
{
    using System.Device.Location;

    /*
    public enum Accuracy
    {
        Low = 0,
        Medium = 1,
        High = 2
    }
    */

    // singleton class for obtaining the current user location
    public sealed class GeoLocation
    {
        private static readonly GeoLocation instance = new GeoLocation();
        private GeoCoordinateWatcher highGeowatcher = null;

        private GeoLocation()
        {
            // initialize gps data
            this.highGeowatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            this.highGeowatcher.MovementThreshold = 20;
            this.highGeowatcher.Start();
        }

        public static GeoLocation Instance
        {
            get
            {
                return instance;
            }
        }

        public GeoCoordinateWatcher GeoWatcher
        {
            get
            {
                return this.highGeowatcher;
            }
        }


        /*
        private GeoCoordinateWatcher lowGeowatcher = null;
        private GeoCoordinateWatcher mediumGeowatcher = null;
        private GeoCoordinateWatcher highGeowatcher = null;
        private GeoCoordinateWatcher current = null;

        private Accuracy accuracy = Accuracy.Low;
        private double threshhold = 0;

        public event EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>> PositionChanged = null;
        
        private GeoLocation()
        {
            // initialize a low accuracy geowatcher
            this.lowGeowatcher = new GeoCoordinateWatcher();
            this.lowGeowatcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(this.watcher_PositionChanged);

            // initialize a medium accuracy geowatcher
            this.mediumGeowatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
            mediumGeowatcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(this.watcher_StatusChanged);

            // initialize a high accuray geowatcher
            this.highGeowatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
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
            this.highGeowatcher.MovementThreshold = this.threshhold;
            this.mediumGeowatcher.MovementThreshold = this.threshhold;
            this.lowGeowatcher.MovementThreshold = this.threshhold;

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

        // Event handler for the GeoCoordinateWatcher.PositionChanged event.
        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            // TODO: only process events from the 'active' geocoordinatewatcher

            this.PositionChanged(this, e);
        }
    }
    */
    }
}
