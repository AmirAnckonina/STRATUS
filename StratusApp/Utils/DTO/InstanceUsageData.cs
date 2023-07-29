

using Amazon.CloudWatch.Model;
using MongoDB.Bson.Serialization.Attributes;

namespace Utils.DTO
{
    public class InstanceUsageData
    {
        [BsonElement("storage")]
        public Storage Storage { get; set; }

        [BsonElement("memory")]
        public Memory Memory;

        [BsonIgnore]
        public CpuStatisticsDTO CpuStatistics { get; set; }
    }
}
