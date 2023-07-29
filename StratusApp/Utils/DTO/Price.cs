using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Utils.Enums;

namespace Utils.DTO
{
    public class Price
    {
        [BsonElement("value")]
        public double Value { get; set; }

        [BsonElement("currency")]
        [BsonRepresentation(BsonType.String)]
        public eCurrencyType CurrencyType { get; set; }

        [BsonElement("PeriodTime")]
        [BsonRepresentation(BsonType.String)]
        public ePeriodTime PeriodTime { get; set; }
        
        public string PriceAsString { get { return this.ToString(); } }  
        
        public override string ToString()
        {
            var metadata = EnumExtensions.GetDisplayAttributesFrom(CurrencyType, typeof(eCurrencyType));
            var periodTime = Enum.GetName(typeof(ePeriodTime), PeriodTime);

            return Value.ToString() + metadata.Name + "per " + periodTime;
        }

        public static Price? Parse(string priceString, ePeriodTime periodTime)
        {
            // Extract the currency symbol and numeric value from the price string
            string currencySymbol = priceString.Substring(0, 1);
            string numericString = priceString.Substring(1);

            // Parse the numeric value as a double
            if (double.TryParse(numericString, out double value))
            {
                // Create a new Price object and set its properties
                Price price = new Price
                {
                    Value = value,
                    CurrencyType = EnumExtensions.GetCurrencyType(currencySymbol),
                    PeriodTime = periodTime,
                };

                return price;
            }

            return null;
        }        
    }
}
