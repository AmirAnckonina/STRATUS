using System;
using System.Collections.Generic;
using System.ComponentModel;
using Amazon;
using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using Amazon.Runtime;
using Amazon.EC2;
using Amazon.Runtime.SharedInterfaces;
using Amazon.Pricing;
using System.Text.Json;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Net;
using Amazon.Pricing.Model;
using Amazon.CostExplorer.Model;
using Amazon.EC2.Model;
using EC2Model = Amazon.EC2.Model;
using System.Net.Sockets;
using CloudApiClient.DTO;
using CloudApiClient.Utils;
using Microsoft.VisualBasic;
using System.Linq.Expressions;
using Amazon.CostExplorer;
using DateInterval = Amazon.CostExplorer.Model.DateInterval;
using CloudApiClient.AwsServices;


namespace CloudApiClient
{
    public class CloudApiClient
    {
        private BasicAWSCredentials _credentials;
        private AmazonCloudWatchClient _cloudWatchClient;
        private RegionEndpoint _region;
        private AmazonEC2Client _ec2Client;
        private readonly PricingService _pricingService;

        public CloudApiClient()
        {
            _credentials = new BasicAWSCredentials("AKIA5HZY22LQTC2MGB5K", "yf0dbGCgKCeMaZelIWsExJCmuJx3bdgoPkR7lQl0");
            _region = RegionEndpoint.USEast2;
            _cloudWatchClient = new AmazonCloudWatchClient(_credentials, RegionEndpoint.USEast2);
            _ec2Client = new AmazonEC2Client(_credentials, _region);
            _pricingService = new PricingService(_credentials);
        }

       /* public async Task<List<Datapoint>> GetInstanceMemory (string instanceId)
        {

        }*/

        //Please NOTE to change the hard-coded instanceID
        public async Task<List<Datapoint>> GetInstanceCPUStatistics(string instanceId)
        {
            instanceId = "i-0e7b7b70d1327c5a6";
            // Get the EC2 instance usage data

            var response = await _cloudWatchClient.GetMetricStatisticsAsync(new GetMetricStatisticsRequest
            {
                Namespace = "AWS/EC2",
                MetricName = "CPUUtilization",
                Dimensions = new List<Amazon.CloudWatch.Model.Dimension> {
                new Amazon.CloudWatch.Model.Dimension {
                    Name = "InstanceId",
                    Value = instanceId
                }
            },
                StartTime = DateTime.UtcNow.AddDays(-2),
                EndTime = DateTime.UtcNow,
                Period = 86400,
                Statistics = new List<string> { "Minimum", "Maximum", "Average", "Sum" }
                //Statistics = new List<string> { "Average" }
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

        /*public async Task<List<Datapoint>> GetInstanceCPUStatistics()
        {
            // Get the EC2 instance usage data

            var response = await _cloudWatchClient.GetMetricStatisticsAsync(new GetMetricStatisticsRequest
            {
                Namespace = "AWS/EC2",
                MetricName = "CPUUtilization",
                Dimensions = new List<Amazon.CloudWatch.Model.Dimension> {
                new Amazon.CloudWatch.Model.Dimension {
                    Name = "InstanceId",
                    Value = "i-0e7b7b70d1327c5a6"
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
        }*/

        public async Task<List<DTO.InstanceDetails>> GetInstanceFormalData()
        {
            //var ec2Client = new AmazonEC2Client(_credentials, RegionEndpoint.USEast2);
            var request = new DescribeInstancesRequest();
            var response = await _ec2Client.DescribeInstancesAsync(request);

            var vms = new List<DTO.InstanceDetails>();

            foreach (var reservation in response.Reservations)
            {
                foreach (var instance in reservation.Instances)
                {
                    if (instance.State.Name == "running") // filter out non-running instances if desired
                    {

                        var vm = new DTO.InstanceDetails
                        {
                            Id = instance.InstanceId,
                            OperatingSystem = instance.PlatformDetails,
                            Price = await GetInstancePrice(instance.InstanceId),
                            CpuSpecifications = $"{instance.CpuOptions.CoreCount} Core/s, {instance.CpuOptions.ThreadsPerCore} threads per Core",
                            Storage = string.Join(", ", instance.BlockDeviceMappings.Select<InstanceBlockDeviceMapping, string>(bdm => $"{bdm.DeviceName}")).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                        };

                        vms.Add(vm);
                    }
                }
            }

            return vms;
        }

        private async Task<decimal> GetInstancePrice(string instanceId)
        {
            instanceId = "i-0e7b7b70d1327c5a6";
            try
            {
                var costExplorerClient = new AmazonCostExplorerClient(_credentials, _region);
                {
                    var request = new GetCostAndUsageRequest
                    {
                        TimePeriod = new DateInterval
                        {
                            Start = "2023-05-15",
                            End = "2023-05-25"
                        },
                        Filter = new Amazon.CostExplorer.Model.Expression
                        {
                            //Dimensions = new DimensionValues
                            //{
                            //    Key = "SERVICE",
                            //    Values = new List<string> { "Amazon Elastic Compute Cloud - Compute" }
                            //},
                            Tags = new TagValues
                            {
                                Key = "InstanceId",
                                Values = new List<string> { instanceId }
                            }
                        },
                        Granularity = "DAILY",
                        Metrics = new List<string> { "AmortizedCost" }
                    };

                    var response = await costExplorerClient.GetCostAndUsageAsync(request);

                    if (response.ResultsByTime.Count > 0)
                    {
                        var costResult = response.ResultsByTime[0].Total;
                        decimal.TryParse(costResult["AmortizedCost"].Amount, out decimal totalCost);
                        return totalCost;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }

            return 0;
        }


        //private async string GetOperatingSystem(string platform)
        //{
        //    switch (platform.ToLower())
        //    {
        //        case "windows":
        //            return "Windows";
        //        default:
        //            return "Linux";
        //    }
        //}


        public async Task<List<double>> GetInstanceCpuUsageOverTime(string instanceId, string filterTime)
        {
            //instanceId = "i-0e7b7b70d1327c5a6";

            var cpuUsageDataByDays = new List<CpuUsageData>();

            if (instanceId == null)
            {
                return new List<double>();
            }

            // Set the dimensions for the CPUUtilization metric
            var dimensions = new List<Amazon.CloudWatch.Model.Dimension>()
            {
                new Amazon.CloudWatch.Model.Dimension() { Name = "InstanceId", Value = instanceId/*"i-00329d0c2a2aac67b"*/ }
            };

            //calculate the total days past in the month
            DateTime currentDate = DateTime.Today;
            int daysPassed = currentDate.Day;
            int monthPassed = currentDate.Month;
            int hourPassed = currentDate.Hour;

            // Set the start and end time for the metric data
            var startTime = DateTime.UtcNow.AddDays(daysPassed * -1);
            var endTime = DateTime.UtcNow;
            int peroid = 60 * 60 * 24;

            switch(filterTime)
            {
                case "Day":
                    startTime = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 0, 0, 0, 0);            
                    endTime = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 23, 59, 59, 999);
                    peroid = 60 * 60;
                    break;
                case "Week":
                    startTime = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0, 0);
                    endTime = new DateTime(currentDate.Year, currentDate.Month, 7, 23, 59, 59, 999);
                    peroid = 60 * 60 * 24;
                    break;
                case "Month":
                    startTime = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0, 0);
                    endTime = new DateTime(currentDate.Year, currentDate.Month, daysPassed, 23, 59, 59, 999);
                    peroid = 60 * 60 * 24;
                    break;
                case "Year":
                    startTime = new DateTime(currentDate.Year, 1, 1, 0, 0, 0, 0);
                    endTime = new DateTime(currentDate.Year, 12, daysPassed, 23, 59, 59, 999);
                    peroid = 60 * 60 * 24 * 30;
                    break;           
            }

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
                            Period = peroid,
                            Stat = "Maximum"
                        },
                        ReturnData = true
                    }
                },
                StartTimeUtc = startTime,
                EndTimeUtc = endTime
            };

            // Retrieve the metric data and create a list of CPU usage data objects
            List<double> array = new List<double>();
            try
            {
                var response = await _cloudWatchClient.GetMetricDataAsync(request);

                //padding zero's because the machine wasnt exsists all the days in the month
                if (filterTime == "Day" && response.MetricDataResults[0].Values.Count < hourPassed)
                {
                    array = Enumerable.Repeat(0.0, hourPassed - response.MetricDataResults[0].Values.Count).ToList();
                }

                //padding zero's because the machine wasnt exsists all the days in the month
                if (filterTime == "Month" && response.MetricDataResults[0].Values.Count < daysPassed)
                {
                    array = Enumerable.Repeat(0.0, daysPassed - response.MetricDataResults[0].Values.Count).ToList();
                }

                //padding zero's because the machine wasnt exsists all the days in the month
                if (filterTime == "Year" && response.MetricDataResults[0].Values.Count < monthPassed)
                {
                    array = Enumerable.Repeat(0.0, monthPassed - response.MetricDataResults[0].Values.Count).ToList();
                }

                foreach (var result in response.MetricDataResults[0].Values)
                {
                    var usageData = new CpuUsageData()
                    {
                        Date = startTime.ToShortDateString(),
                        Usage = result
                    };
                    cpuUsageDataByDays.Add(usageData);
                    startTime = startTime.AddDays(1);

                    array.Add(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            // Serialize the CPU usage data to a JSON string and return it as a response
            //var json = JsonConvert.SerializeObject(cpuUsageData);
            return array;
        }

        public async Task<List<Instance>> GetInstances()
        {
            var instances = new List<Instance>();

            var client = new AmazonEC2Client(RegionEndpoint.USWest2); // Replace with your desired regionEndPoint

            var request = new DescribeInstancesRequest
            {
                Filters = new List<Amazon.EC2.Model.Filter>
                {
                    new Amazon.EC2.Model.Filter
                    {
                        Name = "instance-state-name",
                        Values = new List<string> { "running" }
                    }
                }
            };

            var response = client.DescribeInstancesAsync(request);

            foreach (var reservation in response.Result.Reservations)
            {
                foreach (var instance in reservation.Instances)
                {
                    instances.Add(instance);
                }
            }

            return instances;
        }

        public async Task<List<DTO.InstanceDetails>> GetMoreFittedInstances(string instanceId)
        {
            var accessKey = _credentials.GetCredentials().AccessKey;
            var secretKey = _credentials.GetCredentials().SecretKey;
            var region = _cloudWatchClient.Config.RegionEndpoint;

            // Get the current VM CPU usage metrics
            var currentInstanceDetails = GetInstanceBasicDetails(instanceId);

            InstanceFilterHelper currentVMUsageFilters = CreateVMInstanceFilters(currentInstanceDetails.Result);

            List<DTO.InstanceDetails> fittedInstances = await GetOptionalVms(currentVMUsageFilters, 100);


            // availableInstances = await GetAvailableInstances(accessKey, secretKey, region);

            // Filter the available instances by those with a CPU max capacity of at least maxCPUCapacity
            //var filteredInstances = optionalVm.Where(instance =>
            //    instance.CpuOptions != null && instance.CpuOptions.CoreCount != null && instance.CpuOptions.ThreadsPerCore != null &&
            //    instance.CpuOptions.CoreCount * instance.CpuOptions.ThreadsPerCore * 100 >= maxCPUCapacity);
            // Loop through each filtered instance and print its details
            //foreach (var instance in filteredInstances)
            //{
            //    instancesToReturn.Add(instance);
            //    Console.WriteLine("Instance ID: {0}\nInstance Type: {1}\nMax CPU Capacity: {2}\n",
            //        instance.InstanceId, instance.InstanceType, instance.CpuOptions.CoreCount * instance.CpuOptions.ThreadsPerCore * 100);
            //}

            return fittedInstances;
        }

        private InstanceFilterHelper CreateVMInstanceFilters(DTO.InstanceDetails instance)
        {
            InstanceFilterHelper instanceFilterHelper = new();

            //instanceFilterHelper.AddFilter(FilterType.TERM_MATCH, "operatingSystem", instance.OperatingSystem);
            instanceFilterHelper.AddFilter(FilterType.TERM_MATCH, "operatingSystem", "Linux");
            //instanceFilterHelper.AddFilter(FilterType.TERM_MATCH, "price", instance.Price.ToString());
            //instanceFilterHelper.AddFilter(FilterType.TERM_MATCH, "cpuUsageAverage", instance.CpuStatistics[0].Average.ToString());
            instanceFilterHelper.AddFilter(FilterType.TERM_MATCH, "preInstalledSw", "NA");
            instanceFilterHelper.AddFilter(FilterType.TERM_MATCH, "capacitystatus", "Used");
            instanceFilterHelper.AddFilter(FilterType.TERM_MATCH, "tenancy", "Shared");
            instanceFilterHelper.AddFilter(FilterType.TERM_MATCH, "location", "US East (N. Virginia)");
            instanceFilterHelper.AddFilter(FilterType.TERM_MATCH, "memory", "192 Gib");
            //instanceFilterHelper.AddFilter(FilterType.TERM_MATCH, "Storage", instance.Storage);


            return instanceFilterHelper;
        }

        public async Task<List<DTO.InstanceDetails>> GetOptionalVms(InstanceFilterHelper instanceFilters, int maxResults)
        {
            return await _pricingService.GetOptionalVms(instanceFilters, maxResults);
           
            /*var vmDataList = new List<DTO.InstanceDetails>();
            try
            {
                var ec2Client = new AmazonEC2Client(_credentials, _region);
                //var pricingClient = new AmazonPricingClient(_credentials, _region);
                var pricingClient = new AmazonPricingClient(_credentials, RegionEndpoint.USEast1);

                var getProductsRequest = new GetProductsRequest
                {
                    ServiceCode = "AmazonEC2",
                    Filters = instanceFilters.Filters,
                    */
            /*Filters = new List<Amazon.Pricing.Model.Filter>
                    {
                         new Amazon.Pricing.Model.Filter { Type = "TERM_MATCH", Field = "operatingSystem", Value = "Linux" },
                         new Amazon.Pricing.Model.Filter { Type = "TERM_MATCH", Field = "preInstalledSw", Value = "NA" },
                         new Amazon.Pricing.Model.Filter { Type = "TERM_MATCH", Field = "capacitystatus", Value = "Used" },
                         new Amazon.Pricing.Model.Filter { Type = "TERM_MATCH", Field = "tenancy", Value = "Shared" },
                         new Amazon.Pricing.Model.Filter { Type = "TERM_MATCH", Field = "location", Value = "US East (N. Virginia)" }
                    },*//*
                    MaxResults = maxResults,
                };
                //var test = await pricingClient.ListPriceListsAsync(new ListPriceListsRequest());

                var getProductsResponse = await pricingClient.GetProductsAsync(getProductsRequest);

                foreach (var priceListItem in getProductsResponse.PriceList)
                {
                    var priceListItemJson = priceListItem;

                    // Parse the price list item JSON using JObject
                    var jObject = JObject.Parse(priceListItemJson);

                    var vmData = new DTO.InstanceDetails
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

                    vmDataList.Add(vmData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return vmDataList;*/
        }


        public async Task<string> GetInstanceOperatingSystem(string instanceId)
        {
            //instanceId = "i-0e7b7b70d1327c5a6";

            var request = new DescribeInstancesRequest
            {
                InstanceIds = new List<string> { instanceId }
            };

            var response = await _ec2Client.DescribeInstancesAsync(request);

            // The below line should be changed to retrieve our instance platform.
            //var instance = response.Reservations.SelectMany(r => r.Instances).FirstOrDefault();
            var instance = response.Reservations[0].Instances[0];

            if (instance != null)
            {
                return instance.PlatformDetails;
            }

            return string.Empty;
        }

        public async Task<List<Volume>> GetInstanceVolumes()
        {
            DescribeVolumesRequest descVolumeRequest = new DescribeVolumesRequest()
            {
                Filters = { new EC2Model.Filter { Name = "attachment.instance-id", Values = { "i-0e7b7b70d1327c5a6" } } }
            };
            DescribeVolumesResponse descVolumeResponse = await _ec2Client.DescribeVolumesAsync(descVolumeRequest);

            return descVolumeResponse.Volumes;
        }

        public async Task<int> GetInstanceTotalVolumesSize()
        {
            List<Volume> volumes = await GetInstanceVolumes();
            int totalVolumeSize = 0;

            foreach (Volume vol in volumes)
            {
                totalVolumeSize += vol.Size;
            }

            return totalVolumeSize;
        }

        static List<double> GetCurrentVMCPUUsage(string accessKey, string secretKey, RegionEndpoint region, string instanceId)
        {
            //instanceId =  "i-0e7b7b70d1327c5a6";
            // Instantiate an AmazonCloudWatchClient object with the specified credentials and regionEndPoint
            var cloudWatchClient = new AmazonCloudWatchClient(accessKey, secretKey, region);

            // Build a request to get the current CPU usage metrics for the instance
            var getMetricDataRequest = new GetMetricDataRequest
            {
                MetricDataQueries = new List<MetricDataQuery>
                {
                    new MetricDataQuery
                    {
                        Id = "m1",
                        MetricStat = new MetricStat
                        {
                            Metric = new Amazon.CloudWatch.Model.Metric
                            {
                                Namespace = "AWS/EC2",
                                MetricName = "CPUUtilization",
                                Dimensions = new List<Amazon.CloudWatch.Model.Dimension>
                                {
                                    new Amazon.CloudWatch.Model.Dimension
                                    {
                                        Name = "InstanceId",
                                        Value ="i-0e7b7b70d1327c5a6" // Replace this with a method that gets the current instance ID
                                    }
                                }
                            },
                            Period = 300,
                            Stat = "Average",
                            Unit = StandardUnit.Percent
                        }
                    }
                },
                StartTimeUtc = DateTime.UtcNow.AddMinutes(-5),
                EndTimeUtc = DateTime.UtcNow
            };

            // Send the request and store the response in getMetricDataResponse
            var getMetricDataResponse = cloudWatchClient.GetMetricDataAsync(getMetricDataRequest);

            // Extract the CPU utilization values from the response
            var cpuUtilization = new List<double>();
            foreach (var result in getMetricDataResponse.Result.MetricDataResults)
            {
                if (result.Values.Count > 0)
                {
                    cpuUtilization.Add(result.Values[0]);
                }
            }
            return cpuUtilization;
        }

        public async Task<List<Instance>> GetAvailableInstances(string accessKey, string secretKey, RegionEndpoint region)
        {
            // Instantiate an AmazonEC2Client object with the specified credentials and regionEndPoint
            var ec2Client = new AmazonEC2Client(accessKey, secretKey, region);

            // Build a request to get a list of all available instances in the regionEndPoint
            var describeInstancesRequest = new DescribeInstancesRequest();

            // Send the request and store the response in describeInstancesResponse
            var describeInstancesResponse = await ec2Client.DescribeInstancesAsync(describeInstancesRequest);

            // Extract the instances from the response
            var instances = new List<Instance>();
            foreach (var reservation in describeInstancesResponse.Reservations)
            {
                instances.AddRange(reservation.Instances);
            }

            return instances;
        }

        public async Task<DTO.InstanceDetails> GetInstanceBasicDetails(string instanceId)
        {
            //instanceId = "i-0e7b7b70d1327c5a6";

            DTO.InstanceDetails instanceDetails = new DTO.InstanceDetails();

            // Operating System
            instanceDetails.OperatingSystem = await GetInstanceOperatingSystem(instanceId);

            // Cpu Usage
            instanceDetails.CpuStatistics = await GetInstanceCPUStatistics(instanceId);

            // Volume Usage
            //instanceDetails.TotalVolumesSize = GetInstanceTotalVolumesSize(instanceId);

            // Price
            instanceDetails.Price = await GetInstancePrice(instanceId);

            return instanceDetails;

        }

        //public async Task<Datapoint> GetRecommendedVirtualMachines()
        //{
        //    // Your AWS credentials and regionEndPoint
        //    string accessKey = "YOUR_ACCESS_KEY";
        //    string secretKey = "YOUR_SECRET_KEY";
        //    RegionEndpoint regionEndPoint = RegionEndpoint.USEast2;
        //
        //    // The user's EC2 instance ID and CPU usage percentage
        //    string instanceId = "YOUR_INSTANCE_ID";
        //    double cpuUsage = 50.0;
        //
        //    // Get the EC2 instance and its current specs
        //    Instance instance =  GetInstance(accessKey, secretKey, regionEndPoint, instanceId);
        //    string instanceType = instance.InstanceType;
        //    int instanceCPU =  instance.CpuOptions.CoreCount * instance.CpuOptions.ThreadsPerCore;
        //    int instanceMemory = instance;
        //    double instancePrice = GetInstancePrice(accessKey, secretKey, regionEndPoint, instanceType);
        //
        //    // Get the available instance types in the same availability zone and their specs
        //    List<InstanceType> availableTypes = GetAvailableInstanceTypes(accessKey, secretKey, regionEndPoint, instance.Placement.AvailabilityZone);
        //    List<InstanceTypeSpec> availableSpecs = availableTypes.Select(type =>
        //    {
        //        return new InstanceTypeSpec
        //        {
        //            Type = type,
        //            CPU = type.CpuInfo.SustainedClockSpeedInGhz * type.CpuOptions.TargetCapacity,
        //            Memory = type.MemoryInfo.SizeInMiB,
        //            Price = GetInstancePrice(accessKey, secretKey, regionEndPoint, type.Value)
        //        };
        //    }).ToList();
        //
        //    // Find the cheapest instance type that can accommodate the current CPU usage
        //    var bestOption = availableSpecs.Where(spec => spec.CPU >= cpuUsage * instanceCPU / 100)
        //                                               .OrderBy(spec => spec.Price)
        //                                               .FirstOrDefault();
        //
        //    // Recommend the best instance type to the user, if found
        //    if (bestOption != null && bestOption.Price < instancePrice)
        //    {
        //        Console.WriteLine("Your current VM type: {0} (CPU: {1}, Memory: {2} MiB, Price: {3})", instanceType, instanceCPU, instanceMemory, instancePrice);
        //        Console.WriteLine("Recommended VM type: {0} (CPU: {1}, Memory: {2} MiB, Price: {3})", bestOption.Type, bestOption.CPU, bestOption.Memory, bestOption.Price);
        //    }
        //    else
        //    {
        //        Console.WriteLine("No better VM type found for your usage.");
        //    }
        //}
        //
        //private static Instance GetInstance(string accessKey, string secretKey, RegionEndpoint regionEndPoint, string instanceId)
        //{
        //    // Set up the AWS client for EC2
        //    AmazonEC2Client ec2Client = new AmazonEC2Client(accessKey, secretKey, regionEndPoint);
        //
        //    // Get the instance data from EC2
        //    DescribeInstancesRequest request = new DescribeInstancesRequest
        //    {
        //        InstanceIds = new List<string> { instanceId }
        //    };
        //
        //    DescribeInstancesResponse response = ec2Client.DescribeInstancesAsync(request);
        //    return response.Reservations[0].Instances[0];
        //}
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

    var AmazonPricingClient = new AmazonPricingClient(RegionEndpoint.USEast1);

    var response = AmazonPricingClient.GetProducts(new GetProductsRequest
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
