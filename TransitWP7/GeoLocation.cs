//TODO: copyright info
using System.Device.Location;

namespace TransitWP7
{
    // singleton class for obtaining the current user location
    public sealed class GeoLocation
    {
        private static readonly GeoLocation instance = new GeoLocation();
        private GeoCoordinateWatcher highGeowatcher = null;

        private GeoLocation()
        {
            //TODO: geoposition granted access or not
            //TODO: monitor geoposition status
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
