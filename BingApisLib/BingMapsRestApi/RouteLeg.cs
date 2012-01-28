using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BingApisLib.BingMapsRestApi
{
    /// <summary>
    /// Represents a BingMaps RouteLeg field as defined in http://msdn.microsoft.com/en-us/library/ff701718.aspx
    /// </summary>
    [DebuggerStepThrough]
    [DataContract(Name = "RouteLeg", Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class RouteLeg
    {
        [DataMember(Name = "travelDistance", IsRequired = false)]
        public double TravelDistance { get; set; }

        [DataMember(Name = "travelDuration", IsRequired = false)]
        public double TravelDuration { get; set; }

        [DataMember(Name = "actualStart", IsRequired = false)]
        public Point ActualStart { get; set; }

        [DataMember(Name = "actualEnd", IsRequired = false)]
        public Point ActualEnd { get; set; }

        [DataMember(Name = "startLocation", IsRequired = false)]
        public Location StartLocation { get; set; }

        [DataMember(Name = "endLocation", IsRequired = false)]
        public Location EndLocation { get; set; }

        [DataMember(Name = "itineraryItems", IsRequired = false)]
        [XmlElement("ItineraryItem")]
        public ItineraryItem[] ItineraryItems { get; set; }

        [DataMember(Name = "startTime", IsRequired = false)]
        public System.DateTime StartTime { get; set; }

        [DataMember(Name = "endTime", IsRequired = false)]
        public System.DateTime EndTime { get; set; }
    }
}