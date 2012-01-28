using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BingApisLib.BingMapsRestApi
{
    /// <summary>
    /// Represents a BingMaps TransitLine field as defined in http://msdn.microsoft.com/en-us/library/ff701718.aspx
    /// </summary>
    [DebuggerStepThrough]
    [DataContract(Name = "TransitLine", Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class TransitLine
    {
        [DataMember(Name = "verboseName", IsRequired = false)]
        public string VerboseName { get; set; }

        [DataMember(Name = "abbreviatedName", IsRequired = false)]
        public string AbbreviatedName { get; set; }

        [DataMember(Name = "agencyId", IsRequired = false)]
        public string AgencyId { get; set; }

        [DataMember(Name = "agencyName", IsRequired = false)]
        public string AgencyName { get; set; }

        [DataMember(Name = "lineColor", IsRequired = false)]
        public string LineColor { get; set; }

        [DataMember(Name = "lineTextColor", IsRequired = false)]
        public string LineTextColor { get; set; }

        [DataMember(Name = "uri", IsRequired = false)]
        public string Uri { get; set; }

        [DataMember(Name = "phoneNumber", IsRequired = false)]
        public string PhoneNumber { get; set; }

        [DataMember(Name = "providerInfo", IsRequired = false)]
        public string ProviderInfo { get; set; }
    }
}