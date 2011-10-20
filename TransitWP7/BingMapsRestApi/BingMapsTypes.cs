//TODO: copyright info

//TODO: consider prefixing all these types to prevent name collision with maps control?
namespace TransitWP7.BingMapsRestApi
{
    using System.Diagnostics;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents a BingMaps Response as defined in http://msdn.microsoft.com/en-us/library/ff701707.aspx
    /// </summary>
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    [XmlRoot(Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1", IsNullable = false)]
    public class Response
    {
        public int StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public AuthenticationResultCode AuthenticationResultCode { get; set; }
        public string TraceId { get; set; }
        public string Copyright { get; set; }
        public string BrandLogoUri { get; set; }

        [XmlArrayItem("ResourceSet")]
        public ResourceSet[] ResourceSets { get; set; }

        public string[] ErrorDetails { get; set; }
    }

    /// <summary>
    /// Represents a BingMaps ResourceSet as defined in http://msdn.microsoft.com/en-us/library/ff701707.aspx
    /// </summary>
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class ResourceSet
    {
        public long EstimatedTotal { get; set; }

        [XmlArrayItem(typeof(Route), ElementName = "Route", IsNullable = false)]
        [XmlArrayItem(typeof(Location), ElementName = "Location", IsNullable = false)]
        public object[] Resources { get; set; }
    }

    /// <summary>
    /// Represents a BingMaps Point type as defined in http://msdn.microsoft.com/en-us/library/ff701726.aspx
    /// </summary>
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class Point
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Point() { }

        public Point(double latitute, double longitude)
        {
            this.Latitude = latitute;
            this.Longitude = longitude;
        }

        public override string ToString()
        {
            return string.Format("{0},{1}",
                this.Latitude.ToString("G9"),
                this.Longitude.ToString("G9"));
        }
    }

    /// <summary>
    /// Represents a BingMaps BoundingBox type as defined in http://msdn.microsoft.com/en-us/library/ff701726.aspx
    /// </summary>
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class BoundingBox
    {
        public double SouthLatitude { get; set; }
        public double WestLongitude { get; set; }
        public double NorthLatitude { get; set; }
        public double EastLongitude { get; set; }

        public BoundingBox() { }

        public BoundingBox(double southLatitude, double westLongitude, double northLatitude, double eastLongitude)
        {
            this.SouthLatitude = southLatitude;
            this.WestLongitude = westLongitude;
            this.NorthLatitude = northLatitude;
            this.EastLongitude = eastLongitude;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}",
                this.SouthLatitude.ToString("G9"),
                this.WestLongitude.ToString("G9"),
                this.NorthLatitude.ToString("G9"),
                this.EastLongitude.ToString("G9"));
        }
    }

    /// <summary>
    /// Represents a BingMaps Address type as defined in http://msdn.microsoft.com/en-us/library/ff701726.aspx
    /// </summary>
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class Address
    {
        public string AddressLine { get; set; }
        public string Locality { get; set; }
        public string AdminDistrict { get; set; }
        public string AdminDistrict2 { get; set; }
        public string FormattedAddress { get; set; }
        public string PostalCode { get; set; }
        public string CountryRegion { get; set; }
    }

    /// <summary>
    /// Represents a BingMaps Location resource as defined in http://msdn.microsoft.com/en-us/library/ff701725.aspx
    /// </summary>
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class Location
    {
        public string Name { get; set; }
        public Point Point { get; set; }
        public BoundingBox BoundingBox { get; set; }
        public EntityType EntityType { get; set; }
        public Address Address { get; set; }
        public ConfidenceLevel Confidence { get; set; }
    }

    /// <summary>
    /// Represents a BingMaps Route resource as defined in http://msdn.microsoft.com/en-us/library/ff701718.aspx
    /// </summary>
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class Route
    {
        public string Id { get; set; }
        public BoundingBox BoundingBox { get; set; }
        public string DistanceUnit { get; set; }
        public string DurationUnit { get; set; }
        public double TravelDistance { get; set; }
        public double TravelDuration { get; set; }

        [XmlElement("RouteLeg")]
        public RouteLeg[] RouteLegs { get; set; }

        [XmlElement("RoutePath")]
        public RoutePath[] RoutePaths { get; set; }
    }

    /// <summary>
    /// Represents a BingMaps RouteLeg field as defined in http://msdn.microsoft.com/en-us/library/ff701718.aspx
    /// </summary>
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class RouteLeg
    {
        public double TravelDistance { get; set; }
        public double TravelDuration { get; set; }
        public Point ActualStart { get; set; }
        public Point ActualEnd { get; set; }
        public Location StartLocation { get; set; }
        public Location EndLocation { get; set; }

        [XmlElement("ItineraryItem")]
        public ItineraryItem[] ItineraryItems { get; set; }

        public System.DateTime StartTime { get; set; }
        public System.DateTime EndTime { get; set; }
    }

    /// <summary>
    /// Represents a BingMaps ItineraryItem field as defined in http://msdn.microsoft.com/en-us/library/ff701718.aspx
    /// </summary>
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class ItineraryItem
    {
        [XmlArrayItem("ItineraryItem", IsNullable = false)]
        public ItineraryItem[] ChildItineraryItems { get; set; }

        public string CompassDirection { get; set; }
        public Detail Detail { get; set; }
        public string Exit { get; set; }

        [XmlElement("Hint")]
        public string[] Hint { get; set; }

        public IconType IconType { get; set; }
        public Instruction Instruction { get; set; }
        public Point ManeuverPoint { get; set; }
        public SideOfStreet SideOfStreet { get; set; }
        public string Sign { get; set; } //TODO: collection?
        public System.DateTime Time { get; set; }
        public string TollZone { get; set; }
        public string TowardsRoadName { get; set; }
        public TransitLine TransitLine { get; set; }
        public string TransitStopId { get; set; }
        public string TransitTerminus { get; set; }
        public double TravelDistance { get; set; }
        public double TravelDuration { get; set; }
        public string TravelMode { get; set; }
        public WarningType WarningType { get; set; }
    }

    /// <summary>
    /// Represents a BingMaps Instruction field as defined in http://msdn.microsoft.com/en-us/library/ff701718.aspx
    /// </summary>
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class Instruction
    {
        [XmlAttribute()]
        public string maneuverType { get; set; }

        [XmlText()]
        public string Value { get; set; }
    }

    /// <summary>
    /// Represents a BingMaps Detail field as defined in http://msdn.microsoft.com/en-us/library/ff701718.aspx
    /// </summary>
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class Detail
    {
        public string CompassDegrees { get; set; }
        public string ManeuverType { get; set; }
        public string Name { get; set; }
        public string StartPathIndex { get; set; }
        public string EndPathIndex { get; set; }
        public string RoadType { get; set; }
        public string LocationCode { get; set; }
        public string Mode { get; set; }
        public string PreviousEntityId { get; set; }
        public string NextEntityId { get; set; }
    }

    /// <summary>
    /// Represents a BingMaps TransitLine field as defined in http://msdn.microsoft.com/en-us/library/ff701718.aspx
    /// </summary>
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class TransitLine
    {
        public string VerboseName { get; set; }
        public string AbbreviatedName { get; set; }
        public string AgencyId { get; set; }
        public string AgencyName { get; set; }
        public string LineColor { get; set; }
        public string LineTextColor { get; set; }
        public string Uri { get; set; }
        public string PhoneNumber { get; set; }
        public string ProviderInfo { get; set; }
    }

    /// <summary>
    /// Represents a BingMaps RoutePath field as defined in http://msdn.microsoft.com/en-us/library/ff701718.aspx
    /// </summary>
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class RoutePath
    {
        [XmlArrayItem("Point")]
        public Point[] Line { get; set; }

        [XmlElement("RoutePathGeneralization")]
        public RoutePathGeneralization[] RoutePathGeneralization { get; set; }
    }

    /// <summary>
    /// Represents a BingMaps RoutePathGeneralization field as defined in http://msdn.microsoft.com/en-us/library/ff701718.aspx
    /// </summary>
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class RoutePathGeneralization
    {
        [XmlElement(typeof(int), ElementName = "Index")]
        public int[] PathIndices { get; set; }

        public double LatLongTolerance { get; set; }
    }

    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/ff701725.aspx
    /// </summary>
    public enum ConfidenceLevel
    {
        High,
        Medium,
        Low,
        Unknown
    }

    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/ff701707.aspx
    /// </summary>
    public enum AuthenticationResultCode
    {
        ValidCredentials,
        InvalidCredentials,
        CredentialsExpired,
        NotAuthorized,
        NoCredentials,
        None
    }

    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/ff728811.aspx
    /// </summary>
    public enum EntityType
    {
        Address,
        AdminDivision1,
        AdminDivision2,
        AdminDivision3,
        AdministrativeBuilding,
        AdministrativeDivision,
        AgriculturalStructure,
        Airport,
        AirportRunway,
        AmusementPark,
        AncientSite,
        Aquarium,
        Archipelago,
        Autorail,
        Basin,
        Battlefield,
        Bay,
        Beach,
        BorderPost,
        Bridge,
        BusinessCategory,
        BusinessCenter,
        BusinessName,
        BusinessStructure,
        BusStation,
        Camp,
        Canal,
        Cave,
        CelestialFeature,
        Cemetery,
        Census1,
        Census2,
        CensusDistrict,
        Channel,
        Church,
        CityHall,
        Cliff,
        ClimateRegion,
        Coast,
        CommunityCenter,
        Continent,
        ConventionCenter,
        CountryRegion,
        Courthouse,
        Crater,
        CulturalRegion,
        Current,
        Dam,
        Delta,
        Dependent,
        Desert,
        DisputedArea,
        DrainageBasin,
        Dune,
        EarthquakeEpicenter,
        Ecoregion,
        EducationalStructure,
        ElevationZone,
        Factory,
        FerryRoute,
        FerryTerminal,
        FishHatchery,
        Forest,
        FormerAdministrativeDivision,
        FormerPoliticalUnit,
        FormerSovereign,
        Fort,
        Garden,
        GeodeticFeature,
        GeoEntity,
        GeographicPole,
        Geyser,
        Glacier,
        GolfCourse,
        GovernmentStructure,
        Heliport,
        Hemisphere,
        HigherEducationFacility,
        HistoricalSite,
        Hospital,
        HotSpring,
        Ice,
        IndigenousPeoplesReserve,
        IndustrialStructure,
        InformationCenter,
        InternationalDateline,
        InternationalOrganization,
        Island,
        Isthmus,
        Junction,
        Lake,
        LandArea,
        Landform,
        LandmarkBuilding,
        LatitudeLine,
        Library,
        Lighthouse,
        LinguisticRegion,
        LongitudeLine,
        MagneticPole,
        Marina,
        Market,
        MedicalStructure,
        MetroStation,
        MilitaryBase,
        Mine,
        Mission,
        Monument,
        Mosque,
        Mountain,
        MountainRange,
        Museum,
        NauticalStructure,
        NavigationalStructure,
        Neighborhood,
        Oasis,
        ObservationPoint,
        Ocean,
        OfficeBuilding,
        Park,
        ParkAndRide,
        Pass,
        Peninsula,
        Plain,
        Planet,
        Plate,
        Plateau,
        PlayingField,
        Pole,
        PoliceStation,
        PoliticalUnit,
        PopulatedPlace,
        Postcode,
        Postcode1,
        Postcode2,
        Postcode3,
        Postcode4,
        PostOffice,
        PowerStation,
        Prison,
        Promontory,
        RaceTrack,
        Railway,
        RailwayStation,
        RecreationalStructure,
        Reef,
        Region,
        ReligiousRegion,
        ReligiousStructure,
        ResearchStructure,
        Reserve,
        ResidentialStructure,
        RestArea,
        River,
        Road,
        RoadBlock,
        RoadIntersection,
        Ruin,
        Satellite,
        School,
        ScientificResearchBase,
        Sea,
        SeaplaneLandingArea,
        ShipWreck,
        ShoppingCenter,
        Shrine,
        Site,
        SkiArea,
        Sovereign,
        SpotElevation,
        Spring,
        Stadium,
        StatisticalDistrict,
        Structure,
        TectonicBoundary,
        TectonicFeature,
        Temple,
        TimeZone,
        TouristStructure,
        Trail,
        TransportationStructure,
        Tunnel,
        UnderwaterFeature,
        UrbanRegion,
        Valley,
        Volcano,
        Wall,
        Waterfall,
        WaterFeature,
        Well,
        Wetland,
        Zoo
    }

    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/gg650392.aspx
    /// </summary>
    public enum ManeuverType
    {
        ArriveFinish,
        ArriveIntermediate,
        BearLeft,
        BearLeftThenBearLeft,
        BearLeftThenBearRight,
        BearLeftThenTurnLeft,
        BearLeftThenTurnRight,
        BearRight,
        BearRightThenBearLeft,
        BearRightThenBearRight,
        BearRightThenTurnLeft,
        BearRightThenTurnRight,
        BearThenKeep,
        BearThenMerge,
        Continue,
        DepartIntermediateStop,
        DepartIntermediateStopReturning,
        DepartStart,
        EnterRoundabout,
        ExitRoundabout,
        EnterThenExitRoundabout,
        KeepLeft,
        KeepOnRampLeft,
        KeepOnRampRight,
        KeepOnRampStraight,
        KeepRight,
        KeepStraight,
        KeepToStayLeft,
        KeepToStayRight,
        KeepToStayStraight,
        Merge,
        None,
        RampThenHighwayLeft,
        RampThenHighwayRight,
        RampThenHighwayStraight,
        RoadNameChange,
        Take,
        TakeRampLeft,
        TakeRampRight,
        TakeRampStraight,
        TakeTransit,
        Transfer,
        TransitArrive,
        TransitDepart,
        TurnBack,
        TurnLeft,
        TurnLeftThenBearLeft,
        TurnLeftThenBearRight,
        TurnLeftThenTurnLeft,
        TurnLeftThenTurnRight,
        TurnRight,
        TurnRightThenBearLeft,
        TurnRightThenBearRight,
        TurnRightThenTurnLeft,
        TurnRightThenTurnRight,
        TurnThenMerge,
        TurnToStayLeft,
        TurnToStayRight,
        Unknown,
        UTurn,
        Wait,
        Walk
    }

    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/hh441731.aspx
    /// </summary>
    public enum WarningType
    {
        Accident,
        AdminDivisionChange,
        BlockedRoad,
        CheckTimetable,
        Congestion,
        CountryChange,
        DisabledVehicle,
        GateAccess,
        GetOffTransit,
        GetOnTransit,
        IllegalUTurn,
        MassTransit,
        Miscellaneous,
        NoIncident,
        None,
        Other,
        OtherNews,
        OtherTrafficIncidents,
        PlannedEvents,
        PrivateRoad,
        RestrictedTurn,
        RoadClosures,
        RoadHazard,
        ScheduledConstruction,
        SeasonalClosures,
        Tollbooth,
        TollRoad,
        TollZoneEnter,
        TollZoneExit,
        TrafficFlow,
        TransitLineChange,
        UnpavedRoad,
        Weather
    }

    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/ff701718.aspx
    /// </summary>
    public enum IconType
    {
        None,
        Airline,
        Auto,
        Bus,
        Ferry,
        Train,
        Walk,
        Other
    }

    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/ff701718.aspx
    /// </summary>
    public enum SideOfStreet
    {
        Left,
        Right,
        Unknown
    }
}
