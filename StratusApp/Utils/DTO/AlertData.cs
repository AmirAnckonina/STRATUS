using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using Utils.Enums;

namespace Utils.DTO
{
    public class AlertData
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }
        [BsonElement("email")]
        public string Email { get; set; }
        [BsonElement("machineId")]
        public string MachineId { get; set; }
        [BsonElement("type")]
        public eAlertType Type { get; set; }
        [BsonElement("creationTime")]
        public DateTime CreationTime { get; set; }
        [BsonElement("usagePercentages")]
        public double PercentageUsage { get; set; }

        public AlertData() { }

        public override string ToString()
        {
            return $"Machine: {MachineId}, Type: {Enum.GetName(typeof(eAlertType), Type)}, Creation Time: {CreationTime} Percent: {PercentageUsage}";
        }
    }
}
