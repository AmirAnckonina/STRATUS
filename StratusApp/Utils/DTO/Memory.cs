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

        public static Memory Parse(string memoryString)
        {
            // Define a dictionary for unit conversion factors
            Dictionary<string, eMemoryUnit> unitConversions = new Dictionary<string, eMemoryUnit>
            {
                {"KiB", eMemoryUnit.KB},
                {"MiB", eMemoryUnit.MB},
                {"GiB", eMemoryUnit.GB},
                {"TiB", eMemoryUnit.TB},
                {"KB", eMemoryUnit.KB},
                {"MB", eMemoryUnit.MB},
                {"GB", eMemoryUnit.GB},
                {"TB", eMemoryUnit.TB}
            };

            var match = Regex.Match(memoryString, @"^(\d+(?:\.\d+)?)\s+(KiB|KB|MiB|MB|GiB|GB|TiB|TB)$");
            if (match.Success)
            {
                double value = double.Parse(match.Groups[1].Value);
                string unitString = match.Groups[2].Value;

                if (unitConversions.TryGetValue(unitString, out eMemoryUnit unit))
                {
                    return new Memory { Value = value, Unit = unit };
                }
            }

            return null;
        }

        public override string ToString()
        {
            return Value.ToString() + " " + Enum.GetName(typeof(eMemoryUnit), Unit);
        }
    }
}
