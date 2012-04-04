namespace BingApisLib.BingMapsRestApi
{
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents a BingMaps ResourceSet as defined in http://msdn.microsoft.com/en-us/library/ff701707.aspx
    /// </summary>
    [DebuggerStepThrough]
    [KnownType(typeof(Location)), KnownType(typeof(Route))]
    [DataContract(Name = "ResourceSet", Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class ResourceSet
    {
        public long EstimatedTotal { get; set; }

        [DataMember(Name = "resources", IsRequired = false)]
        [XmlArrayItem(typeof(Route), ElementName = "Route", IsNullable = false)]
        [XmlArrayItem(typeof(Location), ElementName = "Location", IsNullable = false)]
        public object[] Resources { get; set; }
    }
}