using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;
using Utils.Enums;

namespace Utils.DTO
{
    public class Memory
    {
        [BsonElement("value")]
        public double Value { get; set; }
        [BsonElement("unit")]
        [BsonRepresentation(BsonType.String)]
        public eMemoryUnit Unit { get; set; }

        public override string ToString()
        {
            return Value.ToString() + " " + Enum.GetName(typeof(eMemoryUnit), Unit);
        }
    }
}
