using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Utils.DTO
{
    public class InstanceSpecifications
    {       
        [BsonElement("operatingSystem")]
        public string OperatingSystem { get; set; }

        [BsonElement("price")]
        public Price Price { get; set; }

        [BsonElement("storage")]
        public Storage Storage { get; set; }

        [BsonElement("memory")]
        public Memory Memory { get; set; }

        [BsonElement("vCPU")]
        public int VCPU { get; set; }
    }
}
