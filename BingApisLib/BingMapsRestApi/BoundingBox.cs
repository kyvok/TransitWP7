namespace BingApisLib.BingMapsRestApi
{
    using System.Diagnostics;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents a BingMaps BoundingBox type as defined in http://msdn.microsoft.com/en-us/library/ff701726.aspx
    /// </summary>
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class BoundingBox
    {
        public BoundingBox()
        {
        }

        public BoundingBox(double southLatitude, double westLongitude, double northLatitude, double eastLongitude)
        {
            this.SouthLatitude = southLatitude;
            this.WestLongitude = westLongitude;
            this.NorthLatitude = northLatitude;
            this.EastLongitude = eastLongitude;
        }

        public double SouthLatitude { get; set; }

        public double WestLongitude { get; set; }
        
        public double NorthLatitude { get; set; }
        
        public double EastLongitude { get; set; }

        public override string ToString()
        {
            return string.Format(
                "{0},{1},{2},{3}",
                this.SouthLatitude.ToString("G9"),
                this.WestLongitude.ToString("G9"),
                this.NorthLatitude.ToString("G9"),
                this.EastLongitude.ToString("G9"));
        }
    }
}