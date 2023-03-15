using System;
using System.Collections.Generic;
using System.ComponentModel;
using Amazon;
using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using Amazon.Runtime;

namespace CloudApiClient
{
    public class CloudApiClient
    {
        private BasicAWSCredentials _credentials;
        private AmazonCloudWatchClient _cloudWatchClient;

        public CloudApiClient()
        {
            _credentials = new BasicAWSCredentials("AKIA5HZY22LQTC2MGB5K", "yf0dbGCgKCeMaZelIWsExJCmuJx3bdgoPkR7lQl0");
            _cloudWatchClient = new AmazonCloudWatchClient(_credentials, RegionEndpoint.USEast2);
        }

        //public 
        public async Task<List<Datapoint>> GetInstanceData()
        {
            // Get the EC2 instance usage data

            var response = await _cloudWatchClient.GetMetricStatisticsAsync(new GetMetricStatisticsRequest
            {
                Namespace = "AWS/EC2",
                MetricName = "CPUUtilization",
                Dimensions = new List<Dimension> {
                new Dimension {
                    Name = "InstanceId",
                    Value = "i-00329d0c2a2aac67b"
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

            return datapoints;

            //Console.WriteLine($"Avg cpu usage: {avgCpuUsage}, Max cpu usage: {maxCpuUsage}, Min cpu usage: {minCpuUsage}, Sum cpu usage: {sumCpuUsage}");
        }

        public async Task<List<CpuUsageData>> GetInstanceCpuUsageOverTime()
        {
            // Set the dimensions for the CPUUtilization metric
            var dimensions = new List<Dimension>()
            {
                new Dimension() { Name = "InstanceId", Value = "i-00329d0c2a2aac67b" }
            };

            // Set the start and end time for the metric data
            var startTime = DateTime.UtcNow.AddDays(-7);
            var endTime = DateTime.UtcNow;

            // Create a request to get the CPUUtilization metric data
            var request = new GetMetricDataRequest()
            {
                MetricDataQueries = new List<MetricDataQuery>()
                {
                    new MetricDataQuery()
                    {
                        Id = "cpu",
                        MetricStat = new MetricStat()
                        {
                            Metric = new Amazon.CloudWatch.Model.Metric()
                            {
                                Namespace = "AWS/EC2",
                                MetricName = "CPUUtilization",
                                Dimensions = dimensions
                            },
                            Period = 3600 * 24,
                            Stat = "Average"
                        },
                        ReturnData = true
                    }
                },
                StartTimeUtc = startTime,
                EndTimeUtc = endTime
            };

            // Retrieve the metric data and create a list of CPU usage data objects
            var response = await _cloudWatchClient.GetMetricDataAsync(request);
            var cpuUsageDataByDays = new List<CpuUsageData>();

            foreach (var result in response.MetricDataResults[0].Values)
            {
                var usageData = new CpuUsageData()
                {
                    Date = startTime.ToShortDateString(),
                    Usage = result
                };
                cpuUsageDataByDays.Add(usageData);
                startTime = startTime.AddDays(1);
            }

            // Serialize the CPU usage data to a JSON string and return it as a response
            //var json = JsonConvert.SerializeObject(cpuUsageData);
            return cpuUsageDataByDays;
        }

    }
}
        /*
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
    }*/