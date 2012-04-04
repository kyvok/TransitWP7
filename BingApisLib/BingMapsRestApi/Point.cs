namespace BingApisLib.BingMapsRestApi
{
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    // TODO: Finish converting this to JSON

    /// <summary>
    /// Represents a BingMaps Point type as defined in http://msdn.microsoft.com/en-us/library/ff701726.aspx
    /// </summary>
    [DebuggerStepThrough]
    [DataContract(Name = "Point", Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class Point
    {
        public Point()
        {
        }

        public Point(double latitute, double longitude)
        {
            this.Latitude = latitute;
            this.Longitude = longitude;
        }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public override string ToString()
        {
            return string.Format(
                "{0},{1}",
                this.Latitude.ToString("G9"),
                this.Longitude.ToString("G9"));
        }
    }
}