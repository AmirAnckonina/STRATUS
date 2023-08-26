using Amazon.CloudWatch.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Utils.DTO
{
    public abstract class InstanceDetails
    {
        [BsonElement("specifications")]
        public InstanceSpecifications Specifications { get; set; }

        [BsonIgnore]
        public InstanceUsageData UsageData { get; set; }

        [BsonId]
        public ObjectId ObjectId { get; set; }

        [BsonElement("Email")]
        public string UserEmail { get; set; }

        [BsonElement("IP")]
        public string InstanceAddress { get; set; }
    }
}
