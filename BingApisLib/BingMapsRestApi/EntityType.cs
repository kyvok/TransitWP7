using System.Runtime.Serialization;

namespace BingApisLib.BingMapsRestApi
{
    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/ff728811.aspx
    /// </summary>
    [DataContract(Name = "EntityType")]
    public enum EntityType
    {
        [EnumMember]
        Address,
        [EnumMember]
        AdminDivision1,
        [EnumMember]
        AdminDivision2,
        [EnumMember]
        AdminDivision3,
        [EnumMember]
        AdministrativeBuilding,
        [EnumMember]
        AdministrativeDivision,
        [EnumMember]
        AgriculturalStructure,
        [EnumMember]
        Airport,
        [EnumMember]
        AirportRunway,
        [EnumMember]
        AmusementPark,
        [EnumMember]
        AncientSite,
        [EnumMember]
        Aquarium,
        [EnumMember]
        Archipelago,
        [EnumMember]
        Autorail,
        [EnumMember]
        Basin,
        [EnumMember]
        Battlefield,
        [EnumMember]
        Bay,
        [EnumMember]
        Beach,
        [EnumMember]
        BorderPost,
        [EnumMember]
        Bridge,
        [EnumMember]
        BusinessCategory,
        [EnumMember]
        BusinessCenter,
        [EnumMember]
        BusinessName,
        [EnumMember]
        BusinessStructure,
        [EnumMember]
        BusStation,
        [EnumMember]
        Camp,
        [EnumMember]
        Canal,
        [EnumMember]
        Cave,
        [EnumMember]
        CelestialFeature,
        [EnumMember]
        Cemetery,
        [EnumMember]
        Census1,
        [EnumMember]
        Census2,
        [EnumMember]
        CensusDistrict,
        [EnumMember]
        Channel,
        [EnumMember]
        Church,
        [EnumMember]
        CityHall,
        [EnumMember]
        Cliff,
        [EnumMember]
        ClimateRegion,
        [EnumMember]
        Coast,
        [EnumMember]
        CommunityCenter,
        [EnumMember]
        Continent,
        [EnumMember]
        ConventionCenter,
        [EnumMember]
        CountryRegion,
        [EnumMember]
        Courthouse,
        [EnumMember]
        Crater,
        [EnumMember]
        CulturalRegion,
        [EnumMember]
        Current,
        [EnumMember]
        Dam,
        [EnumMember]
        Delta,
        [EnumMember]
        Dependent,
        [EnumMember]
        Desert,
        [EnumMember]
        DisputedArea,
        [EnumMember]
        DrainageBasin,
        [EnumMember]
        Dune,
        [EnumMember]
        EarthquakeEpicenter,
        [EnumMember]
        Ecoregion,
        [EnumMember]
        EducationalStructure,
        [EnumMember]
        ElevationZone,
        [EnumMember]
        Factory,
        [EnumMember]
        FerryRoute,
        [EnumMember]
        FerryTerminal,
        [EnumMember]
        FishHatchery,
        [EnumMember]
        Forest,
        [EnumMember]
        FormerAdministrativeDivision,
        [EnumMember]
        FormerPoliticalUnit,
        [EnumMember]
        FormerSovereign,
        [EnumMember]
        Fort,
        [EnumMember]
        Garden,
        [EnumMember]
        GeodeticFeature,
        [EnumMember]
        GeoEntity,
        [EnumMember]
        GeographicPole,
        [EnumMember]
        Geyser,
        [EnumMember]
        Glacier,
        [EnumMember]
        GolfCourse,
        [EnumMember]
        GovernmentStructure,
        [EnumMember]
        Heliport,
        [EnumMember]
        Hemisphere,
        [EnumMember]
        HigherEducationFacility,
        [EnumMember]
        HistoricalSite,
        [EnumMember]
        Hospital,
        [EnumMember]
        HotSpring,
        [EnumMember]
        Ice,
        [EnumMember]
        IndigenousPeoplesReserve,
        [EnumMember]
        IndustrialStructure,
        [EnumMember]
        InformationCenter,
        [EnumMember]
        InternationalDateline,
        [EnumMember]
        InternationalOrganization,
        [EnumMember]
        Island,
        [EnumMember]
        Isthmus,
        [EnumMember]
        Junction,
        [EnumMember]
        Lake,
        [EnumMember]
        LandArea,
        [EnumMember]
        Landform,
        [EnumMember]
        LandmarkBuilding,
        [EnumMember]
        LatitudeLine,
        [EnumMember]
        Library,
        [EnumMember]
        Lighthouse,
        [EnumMember]
        LinguisticRegion,
        [EnumMember]
        LongitudeLine,
        [EnumMember]
        MagneticPole,
        [EnumMember]
        Marina,
        [EnumMember]
        Market,
        [EnumMember]
        MedicalStructure,
        [EnumMember]
        MetroStation,
        [EnumMember]
        MilitaryBase,
        [EnumMember]
        Mine,
        [EnumMember]
        Mission,
        [EnumMember]
        Monument,
        [EnumMember]
        Mosque,
        [EnumMember]
        Mountain,
        [EnumMember]
        MountainRange,
        [EnumMember]
        Museum,
        [EnumMember]
        NauticalStructure,
        [EnumMember]
        NavigationalStructure,
        [EnumMember]
        Neighborhood,
        [EnumMember]
        Oasis,
        [EnumMember]
        ObservationPoint,
        [EnumMember]
        Ocean,
        [EnumMember]
        OfficeBuilding,
        [EnumMember]
        Park,
        [EnumMember]
        ParkAndRide,
        [EnumMember]
        Pass,
        [EnumMember]
        Peninsula,
        [EnumMember]
        Plain,
        [EnumMember]
        Planet,
        [EnumMember]
        Plate,
        [EnumMember]
        Plateau,
        [EnumMember]
        PlayingField,
        [EnumMember]
        Pole,
        [EnumMember]
        PoliceStation,
        [EnumMember]
        PoliticalUnit,
        [EnumMember]
        PopulatedPlace,
        [EnumMember]
        Postcode,
        [EnumMember]
        Postcode1,
        [EnumMember]
        Postcode2,
        [EnumMember]
        Postcode3,
        [EnumMember]
        Postcode4,
        [EnumMember]
        PostOffice,
        [EnumMember]
        PowerStation,
        [EnumMember]
        Prison,
        [EnumMember]
        Promontory,
        [EnumMember]
        RaceTrack,
        [EnumMember]
        Railway,
        [EnumMember]
        RailwayStation,
        [EnumMember]
        RecreationalStructure,
        [EnumMember]
        Reef,
        [EnumMember]
        Region,
        [EnumMember]
        ReligiousRegion,
        [EnumMember]
        ReligiousStructure,
        [EnumMember]
        ResearchStructure,
        [EnumMember]
        Reserve,
        [EnumMember]
        ResidentialStructure,
        [EnumMember]
        RestArea,
        [EnumMember]
        River,
        [EnumMember]
        Road,
        [EnumMember]
        RoadBlock,
        [EnumMember]
        RoadIntersection,
        [EnumMember]
        Ruin,
        [EnumMember]
        Satellite,
        [EnumMember]
        School,
        [EnumMember]
        ScientificResearchBase,
        [EnumMember]
        Sea,
        [EnumMember]
        SeaplaneLandingArea,
        [EnumMember]
        ShipWreck,
        [EnumMember]
        ShoppingCenter,
        [EnumMember]
        Shrine,
        [EnumMember]
        Site,
        [EnumMember]
        SkiArea,
        [EnumMember]
        Sovereign,
        [EnumMember]
        SpotElevation,
        [EnumMember]
        Spring,
        [EnumMember]
        Stadium,
        [EnumMember]
        StatisticalDistrict,
        [EnumMember]
        Structure,
        [EnumMember]
        TectonicBoundary,
        [EnumMember]
        TectonicFeature,
        [EnumMember]
        Temple,
        [EnumMember]
        TimeZone,
        [EnumMember]
        TouristStructure,
        [EnumMember]
        Trail,
        [EnumMember]
        TransportationStructure,
        [EnumMember]
        Tunnel,
        [EnumMember]
        UnderwaterFeature,
        [EnumMember]
        UrbanRegion,
        [EnumMember]
        Valley,
        [EnumMember]
        Volcano,
        [EnumMember]
        Wall,
        [EnumMember]
        Waterfall,
        [EnumMember]
        WaterFeature,
        [EnumMember]
        Well,
        [EnumMember]
        Wetland,
        [EnumMember]
        Zoo
    }
}