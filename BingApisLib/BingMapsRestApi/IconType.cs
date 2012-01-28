using System.Runtime.Serialization;

namespace BingApisLib.BingMapsRestApi
{
    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/ff701718.aspx
    /// </summary>
    [DataContract(Name = "IconType")]
    public enum IconType
    {
        [EnumMember]
        None,
        [EnumMember]
        Airline,
        [EnumMember]
        Auto,
        [EnumMember]
        Bus,
        [EnumMember]
        Ferry,
        [EnumMember]
        Train,
        [EnumMember]
        Walk,
        [EnumMember]
        Other
    }
}