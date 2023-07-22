using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DTO
{
    public class AlertsConfigurations
    {
        [BsonId]
        ObjectId UserId { get; set; }
        [BsonElement("CpuThreshold")]
        public int CpuThreshold { get; set; }
        [BsonElement("memoryThreshold")]
        public int MemoryThreshold { get; set; }
        [BsonElement("diskThreshold")]
        public int DiskThreshold { get; set; }
        [BsonElement("intervalTimeMilisec")]
        public long IntervalTimeMilisec { get; set; }
        [BsonIgnore]
        public string IntervalPeriod { get; set; }
        [BsonIgnore]
        public long IntervalPeriodValue { get; set; }
    }
}