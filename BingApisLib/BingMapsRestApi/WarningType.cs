using System.Runtime.Serialization;

namespace BingApisLib.BingMapsRestApi
{
    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/hh441731.aspx
    /// </summary>
    [DataContract(Name = "WarningType")]
    public enum WarningType
    {
        [EnumMember]
        Accident,
        [EnumMember]
        AdminDivisionChange,
        [EnumMember]
        BlockedRoad,
        [EnumMember]
        CheckTimetable,
        [EnumMember]
        Congestion,
        [EnumMember]
        CountryChange,
        [EnumMember]
        DisabledVehicle,
        [EnumMember]
        GateAccess,
        [EnumMember]
        GetOffTransit,
        [EnumMember]
        GetOnTransit,
        [EnumMember]
        IllegalUTurn,
        [EnumMember]
        MassTransit,
        [EnumMember]
        Miscellaneous,
        [EnumMember]
        NoIncident,
        [EnumMember]
        None,
        [EnumMember]
        Other,
        [EnumMember]
        OtherNews,
        [EnumMember]
        OtherTrafficIncidents,
        [EnumMember]
        PlannedEvents,
        [EnumMember]
        PrivateRoad,
        [EnumMember]
        RestrictedTurn,
        [EnumMember]
        RoadClosures,
        [EnumMember]
        RoadHazard,
        [EnumMember]
        ScheduledConstruction,
        [EnumMember]
        SeasonalClosures,
        [EnumMember]
        Tollbooth,
        [EnumMember]
        TollRoad,
        [EnumMember]
        TollZoneEnter,
        [EnumMember]
        TollZoneExit,
        [EnumMember]
        TrafficFlow,
        [EnumMember]
        TransitLineChange,
        [EnumMember]
        UnpavedRoad,
        [EnumMember]
        Weather
    }
}