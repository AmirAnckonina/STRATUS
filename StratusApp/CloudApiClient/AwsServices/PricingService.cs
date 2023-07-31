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
using Amazon.EC2.Model;
using Amazon.EC2;
using Newtonsoft.Json.Linq;
using Amazon.Runtime;
using CloudApiClient.AwsServices.Models;
using Newtonsoft.Json;
using Utils.DTO;
using CloudApiClient.AwsServices.AwsUtils;

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

        public async Task<List<AwsInstanceDetails>> GetOptionalVms(InstanceFilterHelper instanceFilters, int maxResults, Task<InstanceDetails> currentInstanceDetails)
        {
            var potentialInstances = new List<AwsInstanceDetails>();
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
                    var singlePotentialInstance = new AwsInstanceDetails()
                    {
                        Specifications = new InstanceSpecifications()
                        {
                            Storage = new Utils.DTO.Storage(),
                            OperatingSystem = product.Attributes.operatingSystem,
                            VCPU = int.Parse(product.Attributes.vcpu),
                            Memory = Memory.Parse(product.Attributes.memory),
                            Price = new Price() { Value = (double)pricePlan.priceInUSD.Value, CurrencyType = Utils.Enums.eCurrencyType.Dollar, PeriodTime = Utils.Enums.ePeriodTime.Hour },
                        },
                        Type = product.Attributes.instanceType,
                        InstanceId = product.Sku,
                    };
                    potentialInstances.Add(singlePotentialInstance);
                }

                return potentialInstances;
            }
            catch
            {
                return potentialInstances;
            }
        }
    }
}

