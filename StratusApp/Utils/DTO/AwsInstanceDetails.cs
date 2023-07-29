using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Utils.DTO
{
    public class AwsInstanceDetails : InstanceDetails
    {
        [BsonElement("instanceId")]
        public string InstanceId { get; set; }

        [BsonElement("type")]
        public string Type { get; set; }
    }
}
