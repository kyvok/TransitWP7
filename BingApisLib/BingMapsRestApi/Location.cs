namespace BingApisLib.BingMapsRestApi
{
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents a BingMaps Location resource as defined in http://msdn.microsoft.com/en-us/library/ff701725.aspx
    /// </summary>
    [DebuggerStepThrough]
    [DataContract(Name = "Location", Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
    public class Location
    {
        private BoundingBox _boundingBox;

        [DataMember(Name = "name", IsRequired = false)]
        public string Name { get; set; }

        [DataMember(Name = "point", IsRequired = false)]
        public Point Point { get; set; }

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

        [DataMember(Name = "entityType", IsRequired = false)]
        public EntityType EntityType { get; set; }

        [DataMember(Name = "address", IsRequired = false)]
        public Address Address { get; set; }

        [DataMember(Name = "confidence", IsRequired = false)]
        public ConfidenceLevel Confidence { get; set; }
    }
}