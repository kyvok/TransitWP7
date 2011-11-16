//TODO: copyright info

namespace TransitWP7
{
    using System.Device.Location;

    // singleton class for obtaining the current user location
    public sealed class GeoLocation
    {
        private static readonly GeoLocation instance = new GeoLocation();
        private GeoCoordinateWatcher highGeowatcher = null;

        private GeoLocation()
        {
            this.highGeowatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            this.highGeowatcher.MovementThreshold = 20;
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
    }
}
