namespace BingApisLib.BingMapsRestApi
{
    using System.Runtime.Serialization;

    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/ff701718.aspx
    /// </summary>
    /// 
    [DataContract(Name = "SideOfStreet")]
    public enum SideOfStreet
    {
        [EnumMember]
        Left,
        [EnumMember]
        Right,
        [EnumMember]
        Unknown
    }
}
