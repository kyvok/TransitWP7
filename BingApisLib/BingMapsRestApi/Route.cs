namespace BingApisLib.BingMapsRestApi
{
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents a BingMaps Route resource as defined in http://msdn.microsoft.com/en-us/library/ff701718.aspx
    /// </summary>
    [DebuggerStepThrough]
    [DataContract(Name = "Route", Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class Route
    {
        private BoundingBox _boundingBox;

        [DataMember(Name = "id", IsRequired = false)]
        public string Id { get; set; }

        [DataMember(Name = "bbox", IsRequired = false)]
        public double[] Bbox { get; set; }

        public BoundingBox BoundingBox
        {
            get
            {
                if (this._boundingBox == null && this.Bbox != null && this.Bbox.Length == 4)
                {
                    this._boundingBox = new BoundingBox(this.Bbox[0], this.Bbox[1], this.Bbox[2], this.Bbox[3]);
                }

                return this._boundingBox;
            }

            set
            {
                this._boundingBox = value;
            }
        }

        [DataMember(Name = "distanceUnit", IsRequired = false)]
        public string DistanceUnit { get; set; }

        [DataMember(Name = "durationUnit", IsRequired = false)]
        public string DurationUnit { get; set; }

        [DataMember(Name = "travelDistance", IsRequired = false)]
        public double TravelDistance { get; set; }

        [DataMember(Name = "travelDuration", IsRequired = false)]
        public double TravelDuration { get; set; }

        [DataMember(Name = "routeLegs", IsRequired = false)]
        [XmlElement("RouteLeg")]
        public RouteLeg[] RouteLegs { get; set; }

        [DataMember(Name = "routePath", IsRequired = false)]
        [XmlElement("RoutePath")]
        public RoutePath[] RoutePaths { get; set; }
    }
}