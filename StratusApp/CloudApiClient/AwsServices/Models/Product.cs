using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CloudApiClient.AwsServices.Models
{
    [Serializable]
    public class Product
    {

        [JsonProperty("productFamily")]
        public string ProductFamily { get; set; }

        [JsonProperty("attributes")]
        public Attributes Attributes { get; set; }

        [JsonProperty("sku")]
        public string Sku { get; set; }

        
    }
}
