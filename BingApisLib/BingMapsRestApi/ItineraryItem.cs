using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BingApisLib.BingMapsRestApi
{
    /// <summary>
    /// Represents a BingMaps ItineraryItem field as defined in http://msdn.microsoft.com/en-us/library/ff701718.aspx
    /// </summary>
    [DebuggerStepThrough]
    [DataContract(Name = "ItineraryItem", Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class ItineraryItem
    {
        [DataMember(Name = "childItineraryItems", IsRequired = false)]
        [XmlArrayItem("ItineraryItem", IsNullable = false)]
        public ItineraryItem[] ChildItineraryItems { get; set; }

        [DataMember(Name = "compassDirection", IsRequired = false)]
        public string CompassDirection { get; set; }

        [DataMember(Name = "details", IsRequired = false)]
        public Detail Detail { get; set; }

        [DataMember(Name = "exit", IsRequired = false)]
        public string Exit { get; set; }

        [DataMember(Name = "hints", IsRequired = false)]
        [XmlElement("Hint")]
        public string[] Hint { get; set; }

        [DataMember(Name = "iconType", IsRequired = false)]
        public IconType IconType { get; set; }

        [DataMember(Name = "instruction", IsRequired = false)]
        public Instruction Instruction { get; set; }

        [DataMember(Name = "maneuverPoint", IsRequired = false)]
        public Point ManeuverPoint { get; set; }

        [DataMember(Name = "sideOfStreet", IsRequired = false)]
        public SideOfStreet SideOfStreet { get; set; }

        // TODO: collection?
        [DataMember(Name = "signs", IsRequired = false)]
        public string Sign { get; set; }

        [DataMember(Name = "time", IsRequired = false)]
        public System.DateTime Time { get; set; }

        [DataMember(Name = "tollZone", IsRequired = false)]
        public string TollZone { get; set; }

        [DataMember(Name = "towardsRoadName", IsRequired = false)]
        public string TowardsRoadName { get; set; }

        [DataMember(Name = "transitLine", IsRequired = false)]
        public TransitLine TransitLine { get; set; }

        [DataMember(Name = "transitStopId", IsRequired = false)]
        public string TransitStopId { get; set; }

        [DataMember(Name = "transitTerminus", IsRequired = false)]
        public string TransitTerminus { get; set; }

        [DataMember(Name = "travelDistance", IsRequired = false)]
        public double TravelDistance { get; set; }

        [DataMember(Name = "travelDuration", IsRequired = false)]
        public double TravelDuration { get; set; }

        [DataMember(Name = "travelMode", IsRequired = false)]
        public string TravelMode { get; set; }

        [DataMember(Name = "warnings", IsRequired = false)]
        public WarningType WarningType { get; set; }
    }
}