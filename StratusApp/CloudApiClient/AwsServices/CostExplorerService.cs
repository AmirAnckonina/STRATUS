using Amazon.CostExplorer.Model;
using Amazon.CostExplorer;
using Amazon.Runtime;
using Amazon.EC2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Utils.DTO;

namespace CloudApiClient.AwsServices
{
    internal class CostExplorerService
    {
        private AmazonCostExplorerClient _costExplorerClient;

        public CostExplorerService(AWSCredentials credentials, RegionEndpoint region)
        {
            //todo ==> uncomment 
            //_costExplorerClient = new AmazonCostExplorerClient(credentials, region);
        }
        public async Task<Price> GetInstancePrice(string instanceId)
        {
            //try
            //{ 
            //    {
            //        var request = new GetCostAndUsageRequest
            //        {
            //            TimePeriod = new DateInterval
            //            {
            //                Start = "2023-05-15",
            //                End = "2023-05-25"
            //            },
            //            Filter = new Amazon.CostExplorer.Model.Expression
            //            {
            //                //Dimensions = new DimensionValues
            //                //{
            //                //    Key = "SERVICE",
            //                //    Values = new List<string> { "Amazon Elastic Compute Cloud - Compute" }
            //                //},
            //                Tags = new TagValues
            //                {
            //                    Key = "InstanceId",
            //                    Values = new List<string> { instanceId }
            //                }
            //            },
            //            Granularity = "DAILY",
            //            Metrics = new List<string> { "AmortizedCost" }
            //        };

            //        var response = await _costExplorerClient.GetCostAndUsageAsync(request);

            //        if (response.ResultsByTime.Count > 0)
            //        {
            //            var costResult = response.ResultsByTime[0].Total;
            //            decimal.TryParse(costResult["AmortizedCost"].Amount, out decimal totalCost);
            //            return totalCost;
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //    return 0;
            //}

            // 

            Random random = new Random();
            return new Price() { Value = random.Next(10, 501), CurrencyType = Utils.Enums.eCurrencyType.Dollar };
        }
    }
}
