using System;
using System.Collections.Generic;
using Amazon;
using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using Amazon.EC2;
using Amazon.Pricing;
using Amazon.Pricing.Model;
namespace CloudApiClient
{
    public class CloudApiClient
    {
        static void PrintInstanceData()
        {
            var session = new AmazonSessionAWSCredentials("", "");

            // Create an EC2 client
            var ec2Client = new AmazonEC2Client(session, RegionEndpoint.USEast1);
            var cloudwatchClient = new AmazonCloudWatchClient(session, RegionEndpoint.USEast1);

            // Get the EC2 instance usage data
            var response = cloudwatchClient.GetMetricStatistics(new GetMetricStatisticsRequest
            {
                Namespace = "AWS/EC2",
                MetricName = "CPUUtilization",
                Dimensions = new List<Dimension> {
                new Dimension {
                    Name = "InstanceId",
                    Value = "i-0512bbb55f1a2f85c"
                }
            },
                StartTime = DateTime.UtcNow.AddDays(-2),
                EndTime = DateTime.UtcNow,
                Period = 86400,
                Statistics = new List<string> { "Minimum", "Maximum", "Average", "Sum" }
            });

            var datapoints = response.Datapoints;

            double avgCpuUsage, maxCpuUsage, minCpuUsage, sumCpuUsage;

            if (datapoints.Count > 0)
            {
                avgCpuUsage = datapoints[0].Average;
                maxCpuUsage = datapoints[0].Maximum;
                minCpuUsage = datapoints[0].Minimum;
                sumCpuUsage = datapoints[0].Sum;
            }
            else
            {
                avgCpuUsage = 0.0;
                maxCpuUsage = 0.0;
                minCpuUsage = 0.0;
                sumCpuUsage = 0.0;
            }

            Console.WriteLine($"Avg cpu usage: {avgCpuUsage}, Max cpu usage: {maxCpuUsage}, Min cpu usage: {minCpuUsage}, Sum cpu usage: {sumCpuUsage}");
        }

        static void ShowPricesOfVms()
        {
            var ec2Client = new AmazonEC2Client(RegionEndpoint.USEast1);

            var response = ec2Client.DescribeInstances();

            var currentInstanceType = response.Reservations[0].Instances[0].InstanceType;
            var currentInstanceId = response.Reservations[0].Instances[0].InstanceId;

            Console.WriteLine($"Current Instance: {currentInstanceId} - Type: {currentInstanceType}");

            var pricingClient = new AmazonPricingClient(RegionEndpoint.USEast1);

            var response = pricingClient.GetProducts(new GetProductsRequest
            {
                ServiceCode = "AmazonEC2",
                Filters = new List<Filter> {
                new Filter {
                    Type = "TERM_MATCH",
                    Field = "operatingSystem",
                    Value = "Linux"
                },
                new Filter {
                    Type = "TERM_MATCH",
                    Field = "preInstalledSw",
                    Value = "NA"
                },
                new Filter {
                    Type = "TERM_MATCH",
                    Field = "capacitystatus",
                    Value = "Used"
                },
                new Filter {
                    Type = "TERM_MATCH",
                    Field = "tenancy",
                    Value = "Shared"
                },
                new Filter {
                    Type = "TERM_MATCH",
                    Field = "location",
                    Value = "US East (N. Virginia)"
                }
            },
                MaxResults = 100
            });

            var instanceData = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();

            foreach (var product in response["PriceList"])
            {
                string sku = product["product"]["sku"];
                string instance_type = product["product"]["attributes"]["instanceType"];
                string instance_family = product["product"]["attributes"]["instanceFamily"];
                string usage_type = product["terms"]["OnDemand"].Keys.ToList()[0];
                float price = float.Parse(product["terms"]["OnDemand"][usage_type]["priceDimensions"]["USD"]["pricePerUnit"]["USD"]);

                if (!instance_data.ContainsKey(instance_family))
                {
                    instance_data[instance_family] = new Dictionary<string, Dictionary<string, object>>();
                }

                instance_data[instance_family][instance_type] = new Dictionary<string, object>
    {
        { "SKU", sku },
        { "UsageType", usage_type },
        { "Price", price }
    };
            }

            string current_instance_family = current_instance_type.Split('.')[0];
            float current_instance_price = (float)instance_data[current_instance_family][current_instance_type]["Price"];

            Console.WriteLine("Possible Instances:");
            foreach (var instance_type in instance_data[current_instance_family].Keys)
            {
                float price = (float)instance_data[current_instance_family][instance_type]["Price"];

                if (price < current_instance_price)
                {
                    Console.WriteLine($" - {instance_type}: $ {price:.2f} (Cheaper than current instance)");
                }
                else if (price > current_instance_price)
                {
                    Console.WriteLine($" - {instance_type}: $ {price:.2f} (More expensive than current instance)");
                }
                else
                {
                    Console.WriteLine($" - {instance_type}: $ {price:.2f} (Same price as current instance)");
                }
            }
        }
}