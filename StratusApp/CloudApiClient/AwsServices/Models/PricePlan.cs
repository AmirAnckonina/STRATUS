using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Pricing.Model;

namespace CloudApiClient.AwsServices.Models
{
    [Serializable]
    public class PricePlan
    {
        public string? unit { get; set; }
        public string? description { get; set; }
        public decimal? priceInUSD { get; set; }
        //public PricePerUnit? pricePerUnit { get; set; }
    }
}
