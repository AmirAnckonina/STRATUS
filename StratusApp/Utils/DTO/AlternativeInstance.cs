using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.DTO
{
    public class AlternativeInstance
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("type")]
        public string InstanceType { get; set; }
        [BsonElement("price")]
        public string HourlyRate { get; set; }
        [BsonElement("vCPU")]
        public string vCPU { get; set; }
        [BsonElement("memory")]
        public string Memory { get; set; }
        [BsonElement("storage")]
        public string Storage { get; set; }
        [BsonElement("networkPerformance")]
        public string NetworkPerformance { get; set; }
        [BsonElement("region")]
        public string region { get; set; }
        [BsonElement("operatingSystem")]
        public string operatingSystem { get; set; }

        public AlternativeInstance(string instanceName, string hourlyRate, string vCPU, string memory, string storage, string networkPerformance, string regionName, string operatingSystemName)
        {
            InstanceType = instanceName;
            HourlyRate = hourlyRate;
            this.vCPU = vCPU;
            Memory = memory;
            Storage = storage;
            NetworkPerformance = networkPerformance;
            region = regionName;
            operatingSystem = operatingSystemName;
        }
    }
}
