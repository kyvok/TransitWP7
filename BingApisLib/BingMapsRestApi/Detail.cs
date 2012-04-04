namespace BingApisLib.BingMapsRestApi
{
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents a BingMaps Detail field as defined in http://msdn.microsoft.com/en-us/library/ff701718.aspx
    /// </summary>
    [DebuggerStepThrough]
    [DataContract(Name = "Detail", Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class Detail
    {
        [DataMember(Name = "compassDegrees", IsRequired = false)]
        public string CompassDegrees { get; set; }

        [DataMember(Name = "maneuverType", IsRequired = false)]
        public string ManeuverType { get; set; }

        [DataMember(Name = "names", IsRequired = false)]
        public string Name { get; set; }

        [DataMember(Name = "StartPathIndices", IsRequired = false)]
        public string StartPathIndex { get; set; }

        [DataMember(Name = "EndPathIndices", IsRequired = false)]
        public string EndPathIndex { get; set; }

        [DataMember(Name = "roadType", IsRequired = false)]
        public string RoadType { get; set; }

        [DataMember(Name = "locationCode", IsRequired = false)]
        public string LocationCode { get; set; }

        [DataMember(Name = "mode", IsRequired = false)]
        public string Mode { get; set; }

        [DataMember(Name = "previousEntityId", IsRequired = false)]
        public string PreviousEntityId { get; set; }

        [DataMember(Name = "nextEntityId", IsRequired = false)]
        public string NextEntityId { get; set; }
    }
}