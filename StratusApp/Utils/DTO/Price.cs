using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
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
        public string PriceAsString { get { return this.ToString(); } }  
        
        public override string ToString()
        {
            var metadata = EnumExtensions.GetDisplayAttributesFrom(CurrencyType, typeof(eCurrencyType));
            
            return Value.ToString() + metadata.Name;
        }
    }
}
