using System;
using System.Device.Location;

namespace TransitWP7
{
    // singleton class for obtaining the current user location
    public sealed class GeoLocation : IDisposable
    {
        private static readonly GeoLocation instance = new GeoLocation();
        private readonly GeoCoordinateWatcher highGeowatcher = null;

        private GeoLocation()
        {
            this.highGeowatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default) { MovementThreshold = 20 };
            this.highGeowatcher.Start();
        }

        public static GeoLocation Instance
        {
            get { return instance; }
        }

        public GeoCoordinateWatcher GeoWatcher
        {
            get { return this.highGeowatcher; }
        }

        public void Dispose()
        {
            if (this.highGeowatcher != null)
            {
                this.highGeowatcher.Dispose();
            }
        }
    }
}
