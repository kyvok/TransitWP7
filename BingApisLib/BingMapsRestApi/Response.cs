using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BingApisLib.BingMapsRestApi
{
    /// <summary>
    /// Represents a BingMaps Response as defined in http://msdn.microsoft.com/en-us/library/ff701707.aspx
    /// </summary>
    [DebuggerStepThrough]
    [DataContract(Name = "Response", Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    [XmlRoot(Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1", IsNullable = false)]
    public class Response
    {
        [DataMember(Name = "statusCode", IsRequired = false)]
        public int StatusCode { get; set; }

        [DataMember(Name = "statusDescription", IsRequired = false)]
        public string StatusDescription { get; set; }

        [DataMember(Name = "authenticationResultCode", IsRequired = false)]
        public AuthenticationResultCode AuthenticationResultCode { get; set; }

        [DataMember(Name = "traceId", IsRequired = false)]
        public string TraceId { get; set; }

        [DataMember(Name = "copyright", IsRequired = false)]
        public string Copyright { get; set; }

        [DataMember(Name = "brandLogoUri", IsRequired = false)]
        public string BrandLogoUri { get; set; }

        [DataMember(Name = "resourceSets", IsRequired = false)]
        [XmlArrayItem("ResourceSet")]
        public ResourceSet[] ResourceSets { get; set; }

        [DataMember(Name = "errorDetails", IsRequired = false)]
        public string[] ErrorDetails { get; set; }
    }
}