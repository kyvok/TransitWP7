using System.Diagnostics;
using System.Xml.Serialization;

namespace BingApisLib.BingMapsRestApi
{
    // TODO: Finish converting this to JSON

    /// <summary>
    /// Represents a BingMaps RoutePath field as defined in http://msdn.microsoft.com/en-us/library/ff701718.aspx
    /// </summary>
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class RoutePath
    {
        [XmlArrayItem("Point")]
        public Point[] Line { get; set; }

        [XmlElement("RoutePathGeneralization")]
        public RoutePathGeneralization[] RoutePathGeneralization { get; set; }
    }
}