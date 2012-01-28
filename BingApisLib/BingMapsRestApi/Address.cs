using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BingApisLib.BingMapsRestApi
{
    /// <summary>
    /// Represents a BingMaps Address type as defined in http://msdn.microsoft.com/en-us/library/ff701726.aspx
    /// </summary>
    [DebuggerStepThrough]
    [DataContract(Name = "Address", Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class Address
    {
        [DataMember(Name = "addressLine", IsRequired = false)]
        public string AddressLine { get; set; }

        [DataMember(Name = "locality", IsRequired = false)]
        public string Locality { get; set; }

        [DataMember(Name = "adminDistrict", IsRequired = false)]
        public string AdminDistrict { get; set; }

        [DataMember(Name = "adminDistrict2", IsRequired = false)]
        public string AdminDistrict2 { get; set; }

        [DataMember(Name = "formattedAddress", IsRequired = false)]
        public string FormattedAddress { get; set; }

        [DataMember(Name = "postalCode", IsRequired = false)]
        public string PostalCode { get; set; }

        [DataMember(Name = "countryRegion", IsRequired = false)]
        public string CountryRegion { get; set; }
    }
}