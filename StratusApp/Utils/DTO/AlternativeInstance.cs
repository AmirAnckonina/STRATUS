using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
namespace Utils.DTO
{
    public class AlternativeInstance
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("specifications")]
        public InstanceSpecifications Specifications { get; set; }
        [BsonElement("type")]
        public string InstanceType { get; set; }
        [BsonElement("networkPerformance")]
        public string NetworkPerformance { get; set; }
        [BsonElement("region")]
        public string Region { get; set; }
    }
}
