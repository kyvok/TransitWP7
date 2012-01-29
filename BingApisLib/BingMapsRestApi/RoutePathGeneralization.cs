using System.Diagnostics;
using System.Xml.Serialization;

namespace BingApisLib.BingMapsRestApi
{
    // TODO: Finish converting this to JSON

    /// <summary>
    /// Represents a BingMaps RoutePathGeneralization field as defined in http://msdn.microsoft.com/en-us/library/ff701718.aspx
    /// </summary>
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class RoutePathGeneralization
    {
        [XmlElement(typeof(int), ElementName = "Index")]
        public int[] PathIndices { get; set; }

        public double LatLongTolerance { get; set; }
    }
}