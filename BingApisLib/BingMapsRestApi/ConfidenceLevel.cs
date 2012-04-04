namespace BingApisLib.BingMapsRestApi
{
    using System.Runtime.Serialization;

    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/ff701725.aspx
    /// </summary>
    [DataContract(Name = "ConfidenceLevel")]
    public enum ConfidenceLevel
    {
        [EnumMember]
        High,
        [EnumMember]
        Medium,
        [EnumMember]
        Low,
        [EnumMember]
        Unknown
    }
}