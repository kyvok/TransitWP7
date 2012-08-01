namespace GoogleApisLib.GooglePlacesApi
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    public enum StatusCodes
    {
        OK,

        ZERO_RESULTS,

        OVER_QUERY_LIMIT,

        REQUEST_DENIED,

        INVALID_REQUEST
    }

    /// <summary>
    /// All types defined from https://developers.google.com/places/documentation/#TextSearchRequests
    /// </summary>
    [DebuggerStepThrough]
    public class SearchResponse
    {
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Reviewed. Suppression is OK here.")]
        public string[] html_attributions { get; set; }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Reviewed. Suppression is OK here.")]
        public string status { get; set; }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Reviewed. Suppression is OK here.")]
        public TextSearchResult[] results { get; set; }
    }

    [DebuggerStepThrough]
    public class TextSearchResult
    {
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Reviewed. Suppression is OK here.")]
        public string name { get; set; }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Reviewed. Suppression is OK here.")]
        public string formatted_address { get; set; }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Reviewed. Suppression is OK here.")]
        public Geometry geometry { get; set; }
    }

    [DebuggerStepThrough]
    public class Geometry
    {
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Reviewed. Suppression is OK here.")]
        public Location location { get; set; }
    }

    [DebuggerStepThrough]
    public class Location
    {
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Reviewed. Suppression is OK here.")]
        public double lat { get; set; }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Reviewed. Suppression is OK here.")]
        public double lng { get; set; }
    }
}