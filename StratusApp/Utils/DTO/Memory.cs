using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
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

        public static Memory? Parse(string memoryString)
        {
            // Use regex to extract the numeric value and memory type from the input string
            var match = Regex.Match(memoryString, @"^(\d+(?:\.\d+)?)\s+(KB|MB|GB|TB)$");
            if (match.Success)
            {
                double value = double.Parse(match.Groups[1].Value);
                eMemoryUnit unit = Enum.Parse<eMemoryUnit>(match.Groups[2].Value);

                return new Memory { Value = value, Unit = unit };
            }

            return null;
        }

        public override string ToString()
        {
            return Value.ToString() + " " + Enum.GetName(typeof(eMemoryUnit), Unit);
        }
    }
}
