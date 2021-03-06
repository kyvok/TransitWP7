namespace BingApisLib.BingMapsRestApi
{
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents a BingMaps Instruction field as defined in http://msdn.microsoft.com/en-us/library/ff701718.aspx
    /// </summary>
    [DebuggerStepThrough]
    [DataContract(Name = "instruction", Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class Instruction
    {
        [DataMember(Name = "maneuverType", IsRequired = false)]
        [XmlAttribute(AttributeName = "maneuverType")]
        public string ManeuverType { get; set; }

        [DataMember(Name = "text", IsRequired = false)]
        [XmlText]
        public string Value { get; set; }
    }
}