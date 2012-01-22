using System.Diagnostics;
using System.Xml.Serialization;

namespace BingApisLib.BingSearchRestApi
{
    /// <summary>
    /// All types defined from http://msdn.microsoft.com/en-us/library/dd250882.aspx
    /// </summary>

    [DebuggerStepThrough()]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/LiveSearch/2008/04/XML/element")]
    [XmlRoot(Namespace = "http://schemas.microsoft.com/LiveSearch/2008/04/XML/element", IsNullable = false)]
    public class SearchResponse
    {
        public Query Query { get; set; }

        [XmlElement(Namespace = "http://schemas.microsoft.com/LiveSearch/2008/04/XML/phonebook")]
        public Phonebook Phonebook { get; set; }

        [XmlElement(Namespace = "http://schemas.microsoft.com/LiveSearch/2008/04/XML/spell")]
        public Spell Spell { get; set; }

        [XmlAttribute()]
        public string Version { get; set; }

        [XmlArrayItemAttribute("Error", IsNullable = false)]
        public Error[] Errors { get; set; }
    }

    [DebuggerStepThrough()]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/LiveSearch/2008/04/XML/element")]
    public class Query
    {
        public string SearchTerms { get; set; }
        public string AlterationQueryOverride { get; set; }
        public string AlteredQuery { get; set; }
    }

    [DebuggerStepThrough()]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/LiveSearch/2008/04/XML/phonebook")]
    [XmlRoot(Namespace = "http://schemas.microsoft.com/LiveSearch/2008/04/XML/phonebook", IsNullable = false)]
    public class Phonebook
    {
        public uint Total { get; set; }
        public uint Offset { get; set; }
        public string LocalSerpUrl { get; set; }
        public string Title { get; set; }

        [XmlArrayItem("PhonebookResult", IsNullable = false)]
        public PhonebookResult[] Results { get; set; }
    }

    [DebuggerStepThrough()]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/LiveSearch/2008/04/XML/phonebook")]
    public class PhonebookResult
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Business { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string StateOrProvince { get; set; }
        public string CountryOrRegion { get; set; }
        public string PostalCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string UniqueId { get; set; }
        public double UserRating { get; set; }
        public uint ReviewCount { get; set; }
        public string DisplayUrl { get; set; }

        [XmlIgnore()]
        public bool UserRatingSpecified { get; set; }

        [XmlIgnore()]
        public bool ReviewCountSpecified { get; set; }
    }

    [DebuggerStepThrough()]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/LiveSearch/2008/04/XML/spell")]
    public class Spell
    {
        public uint Total { get; set; }

        [XmlArrayItem("SpellResult", IsNullable = false)]
        public SpellResult[] Results { get; set; }
    }

    [DebuggerStepThrough()]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/LiveSearch/2008/04/XML/spell")]
    public class SpellResult
    {
        public string Value { get; set; }
    }

    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/LiveSearch/2008/04/XML/element")]
    public class Error
    {
        public uint Code { get; set; }
        public string Message { get; set; }
        public string HelpUrl { get; set; }
        public string Parameter { get; set; }
        public string SourceType { get; set; }
        public uint SourceTypeErrorCode { get; set; }
    }
}
