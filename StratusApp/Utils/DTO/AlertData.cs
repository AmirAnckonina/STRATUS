using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using Utils.Enums;

namespace Utils.DTO
{
    public class AlertData
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("machineId")]
        public string MachineId { get; set; }
        [BsonElement("type")]
        public eAlertType Type { get; set; }
        [BsonElement("creationTime")]
        public DateTime CreationTime { get; set; }
        [BsonElement("usagePercentages")]
        public double PercentageUsage { get; set; }

        public AlertData() { }
    }
}
