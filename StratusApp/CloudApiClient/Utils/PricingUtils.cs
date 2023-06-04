using Amazon.CostExplorer.Model;
using CloudApiClient.AwsServices.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace CloudApiClient.Utils
{
    internal class PricingUtils
    {
        public PricingUtils() { }

        public PricePlan BuildPricePlanFromPriceListString(string rawPriceListItem, string productSku)
        {
            PricePlan onDemandPricePlan = new PricePlan();

            JObject priceListJson = JObject.Parse(rawPriceListItem);
            string regexProductSku = $"^{productSku}";

            // Here we should match the product Sku with the beggining of the PriceDimenstions key.
            var allPriceDimensions = priceListJson?["terms"]?["OnDemand"]?.Values<JProperty>().FirstOrDefault()?.Value["priceDimensions"];

            if (allPriceDimensions != null)
            {
                var firstPriceDimension = allPriceDimensions.Values<JProperty>().FirstOrDefault();

                string? priceInString = firstPriceDimension?.Value["pricePerUnit"]?["USD"]?.ToString();
                if (priceInString != null)
                { 
                    onDemandPricePlan.priceInUSD = decimal.Parse(priceInString); 
                }
                onDemandPricePlan.unit = firstPriceDimension?.Value["unit"]?.ToString();
                onDemandPricePlan.description = firstPriceDimension?.Value["description"]?.ToString();
            }
            else
            {
                throw new Exception("Price dimensions field not found or not parsed successfully.");
            }
               

            return onDemandPricePlan;
        }

        public Product BuildProductFromPriceListString(string rawPriceListItem)
        {
            Product product = new Product();

            //Parse the price list item JSON using JObject
            JObject priceListJson = JObject.Parse(rawPriceListItem);
            var rawProduct = priceListJson?["product"];

            if (rawProduct != null) 
            {
                string rawProductJson = rawProduct.ToString();
                product = JsonConvert.DeserializeObject<Product>(rawProductJson);
            }

            else
            {
                throw new Exception("Product field not found or not parsed successfully.");
            }

            return product;
        }        
    }
}
