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
using CloudApiClient.AwsServices.Models;
using Newtonsoft.Json;
using Utils.DTO;

namespace CloudApiClient.AwsServices
{

    public class PricingService
    {
        private AmazonPricingClient _pricingClient;
        private readonly PricingUtils _pricingUtils;

        public PricingService(AWSCredentials credentials)
        {
            // NOTE that pricing Api working ONLY with USEast1 and APSouth1.
            _pricingClient = new AmazonPricingClient(credentials, RegionEndpoint.USEast1);
            _pricingUtils = new PricingUtils();
        }

        public async Task<List<InstanceDetails>> GetOptionalVms(InstanceFilterHelper instanceFilters, int maxResults, Task<InstanceDetails> currentInstanceDetails)
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

                foreach (string priceListItem in getProductsResponse.PriceList)
                {
                    var rawPriceListItem = priceListItem;

                    Product product = _pricingUtils.BuildProductFromPriceListString(rawPriceListItem);
                    PricePlan pricePlan = _pricingUtils.BuildPricePlanFromPriceListString(rawPriceListItem, product.Sku);
                    //TODo: Add the current instance details to filter the potential instances list.
                    var singlePotentialInstance = new InstanceDetails()
                    {
                        Id = product.Sku,
                        Type = product.Attributes.instanceType,
                        Storage = product.Attributes.storage,
                        OperatingSystem = product.Attributes.operatingSystem,
                        CpuSpecifications = product.Attributes.vcpu,
                        Price = pricePlan.priceInUSD,
                        Unit = pricePlan.unit,
                        PriceDescription = pricePlan.description
                    };
                    potentialInstances.Add(singlePotentialInstance);
                }

                return potentialInstances;
            }
            catch (Exception ex)
            {
                /// Change exception architecture to try-catch on controller.
                /// 
                return new List<InstanceDetails>();
            }
        }
    }
}

