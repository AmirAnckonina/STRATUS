using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Pricing;
using Amazon.Pricing.Model;
using Amazon.CloudWatch;
using CloudApiClient.Utils;
using Amazon.EC2.Model;
using Amazon.EC2;
using Newtonsoft.Json.Linq;
using Amazon.Runtime;
using CloudApiClient.DTO;

namespace CloudApiClient.AwsServices
{

    public class PricingService
    {
        private AmazonPricingClient _pricingClient;

        public PricingService(AWSCredentials credentials)
        {
            // NOTE that pricing Api working ONLY with USEast1 and APSouth1.
           _pricingClient = new AmazonPricingClient(credentials, RegionEndpoint.USEast1);
        }

        public async Task<List<InstanceDetails>> GetOptionalVms(InstanceFilterHelper instanceFilters, int maxResults)
        {
            var potentialInstances = new List<InstanceDetails>();
            try
            {
                var getProductsRequest = new GetProductsRequest
                {
                    ServiceCode = "AmazonEC2",
                    Filters = instanceFilters.Filters,
                    MaxResults = maxResults,
                };

                var getProductsResponse = await _pricingClient.GetProductsAsync(getProductsRequest);

                foreach (var priceListItem in getProductsResponse.PriceList)
                {
                    var priceListItemJson = priceListItem;

                    // Parse the price list item JSON using JObject
                    var jObject = JObject.Parse(priceListItemJson);

                    var vmData = new InstanceDetails
                    {
                        Id = (string)jObject["product"]["sku"],
                        OperatingSystem = (string)jObject["product"]["attributes"]["operatingSystem"],
                        Storage = new List<string>(),
                        // EREZ please note: CpuSpecs currently returns a List od Datapoint instead of string !!!
                        //CpuSpecifications = (string)jObject["product"]["attributes"]["vcpu"],
                    };

                    // Get the price dimensions
                    var priceDimensions = jObject["terms"]["OnDemand"].Values<JProperty>().FirstOrDefault()?.Value["priceDimensions"];

                    if (priceDimensions != null)
                    {
                        foreach (var priceDimension in priceDimensions.Values<JProperty>())
                        {
                            var pricePerUnit = (string)priceDimension.Value["pricePerUnit"]["USD"];
                            vmData.Price = decimal.Parse(pricePerUnit);
                            vmData.Unit = (string)priceDimension.Value["unit"];

                            break; // Consider only the first price dimension
                        }
                    }

                    // Process storage attributes
                    var storageAttributes = jObject["product"]?["attributes"]?["storage"];
                    if (storageAttributes != null)
                    {
                        if (storageAttributes is JObject storageObject)
                        {
                            foreach (var storageValue in storageObject.Values<string>())
                            {
                                vmData.Storage.Add(storageValue);
                            }
                        }
                        else if (storageAttributes is JValue storageValue)
                        {
                            vmData.Storage.Add(storageValue.Value.ToString());
                        }
                    }

                    potentialInstances.Add(vmData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return potentialInstances;
        }



    }    
}
