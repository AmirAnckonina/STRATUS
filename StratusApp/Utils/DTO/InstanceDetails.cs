using Amazon.CloudWatch.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Utils.DTO
{
    public class InstanceDetails
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }

        [BsonElement("userId")]
        public ObjectId UserId { get; set; }

        [BsonElement("IP")]
        public string IP { get; set; }
        [BsonElement("operatingSystem")]
        public string OperatingSystem { get; set; }

        [BsonElement("price")]
        public decimal? Price { get; set; }

        [BsonElement("storage")]
        public string Storage { get; set; }

        [BsonElement("memory")]
        public int TotalVolumesSize;

        [BsonElement("vCPU")]
        public string VCPU { get; set; }

        [BsonElement("instanceId")]
        public string InstanceId { get; set; }

        [BsonElement("type")]
        public string Type { get; set; }

        [BsonIgnore]
        public List<Datapoint> CpuStatistics { get; set; }

        [BsonIgnore]
        public int TotalStorageSize { get; set; }

        [BsonIgnore]
        public string? Unit { get; set; }

        [BsonIgnore]
        public string? PriceDescription {get;set;}
    }
}
