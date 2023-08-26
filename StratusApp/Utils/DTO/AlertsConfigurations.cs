using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DTO
{
    public class AlertsConfigurations
    {
        private const int defaultCpuPercentageThreshold = 70;
        private const int defaultMemoryPercentageThreshold = 70;
        private const int defaultStoragePercentageThreshold = 70;
        private const long defaultIntervalTimeToAlert = 1000 * 60;

        [BsonId]
        public ObjectId ObjectId { get; set; }
        [BsonElement("Email")]
        public string UserEmail { get; set; }
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

        public AlertsConfigurations()
        {
            CpuThreshold = defaultCpuPercentageThreshold;
            MemoryThreshold = defaultMemoryPercentageThreshold;
            DiskThreshold = defaultStoragePercentageThreshold;
            IntervalTimeMilisec = defaultIntervalTimeToAlert;
        }

    }
}