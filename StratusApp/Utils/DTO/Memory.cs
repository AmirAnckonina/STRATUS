using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Utils.Enums;
using Utils.Utils;

namespace Utils.DTO
{
    public class Memory
    {
        [BsonElement("value")]
        public double Value { get; set; }
        [BsonElement("unit")]
        [BsonRepresentation(BsonType.String)]
        public eSizeUnit Unit { get; set; }
        public string AsString { get { return ToString(); } }

        public Memory() 
        {
        } 

        public Memory(double value, eSizeUnit unit)
        {
            Value = value;
            Unit = unit;
        }

        public static Memory Parse(string memoryString)
        {
            var match = Regex.Match(memoryString, @"^(\d+(?:\.\d+)?)\s+(KiB|KB|MiB|MB|GiB|GB|TiB|TB)$");
            if (match.Success)
            {
                double value = double.Parse(match.Groups[1].Value);
                string unitString = match.Groups[2].Value;

                if (UnitConvertorMapper.UnitConvertor.TryGetValue(unitString, out eSizeUnit unit))
                {
                    return new Memory { Value = value, Unit = unit };
                }
            }

            throw new Exception("Parsing memory procedure failed.");
        }

        public override string ToString()
        {
            return Math.Round(Value,3).ToString() + " " + Enum.GetName(typeof(eSizeUnit), Unit);
        }
    }
}
