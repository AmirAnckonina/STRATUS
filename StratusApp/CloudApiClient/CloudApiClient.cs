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
        //private AmazonCloudWatchClient _cloudWatchClient;
        private RegionEndpoint _region;
        private readonly PricingService _pricingService;
        private readonly EC2Service _ec2Service;
        private readonly CloudWatchService _cloudWatchService;
        private readonly CostExplorerService _costExplorerService;
        private readonly AWSScraper _awsScraper;

        public CloudApiClient()
        {
            //_credentials = new BasicAWSCredentials();
            _region = RegionEndpoint.USEast1;
            //_cloudWatchClient = new AmazonCloudWatchClient(_credentials, RegionEndpoint.USEast2);
            //_ec2Client = new AmazonEC2Client(_credentials, _region);
            _pricingService = new PricingService(_credentials);
            _ec2Service = new EC2Service(_credentials, _region);
            _cloudWatchService = new CloudWatchService(_credentials, _region);
            _costExplorerService = new CostExplorerService(_credentials, RegionEndpoint.USEast2);
            _awsScraper = new AWSScraper();

        }

        //Please NOTE to change the hard-coded instanceID
        public async Task<List<Datapoint>> GetInstanceCPUStatistics(string instanceId)
        {
            return await _cloudWatchService.GetInstanceCPUStatistics(instanceId);
        }

        // Split it according to the services!!! 
        public async Task<List<DTO.InstanceDetails>> GetInstanceFormalData()
        {
            var vms = new List<DTO.InstanceDetails>();

            DescribeInstancesResponse descInstancesResponse = await _ec2Service.DescribeInstancesAsync();

            foreach (var reservation in descInstancesResponse.Reservations)
            {
                foreach (var instance in reservation.Instances)
                {
                    if (instance != null && instance.State.Name == "running") // filter out non-running instances if desired
                    {

                        var vm = new DTO.InstanceDetails
                        {
                            Id = instance.InstanceId,
                            OperatingSystem = instance.PlatformDetails,
                            Price = await _costExplorerService.GetInstancePrice(instance.InstanceId),
                            CpuSpecifications = $"{instance.CpuOptions.CoreCount} Core/s, {instance.CpuOptions.ThreadsPerCore} threads per Core",
                            TotalStorageSize = await GetInstanceTotalVolumesSize(instance.InstanceId),
                            //string.Join(", ", instance.BlockDeviceMappings.Select<InstanceBlockDeviceMapping, string>(bdm => $"{bdm.DeviceName}")).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                        };

                        vms.Add(vm);
                    }
                }
            }

            return vms;
        }

        // What is That?
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
            return await _cloudWatchService.GetInstanceCpuUsageOverTime(instanceId); 
        }

        public async Task<List<Instance>> GetInstances()
        {
            return await _ec2Service.GetInstances();
        }

        public async Task<List<DTO.InstanceDetails>> GetMoreFittedInstances(string instanceId)
        {
            // Get the current VM CPU usage metrics
            var currentInstanceDetails = GetInstanceBasicDetails(instanceId);

            InstanceFilterHelper currentVMUsageFilters = CreateVMInstanceFilters(currentInstanceDetails.Result);

            List<DTO.InstanceDetails> fittedInstances = await GetOptionalVms(currentVMUsageFilters, 100);

            // What is That?
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

        // What is that? should be arranged.
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
        }

        public async Task<string> GetInstanceOperatingSystem(string instanceId)
        {
            return await _ec2Service.GetInstanceOperatingSystem(instanceId);
        }

        public async Task<List<Volume>> GetInstanceVolumes(string instanceId)
        {
            return await _ec2Service.GetInstanceVolumes(instanceId);
        }

        public async Task<int> GetInstanceTotalVolumesSize(string instanceId)
        {
            List<Volume> volumes = await GetInstanceVolumes(instanceId);
            int totalVolumeSize = 0;

            foreach (Volume vol in volumes)
            {
                totalVolumeSize += vol.Size;
            }

            return totalVolumeSize;
        }

        //What is That?
        /*static List<double> GetCurrentVMCPUUsage(string accessKey, string secretKey, RegionEndpoint region, string instanceId)
        {
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
        }*/

        // What is that?
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
            DTO.InstanceDetails instanceDetails = new DTO.InstanceDetails();

            // Operating System
            instanceDetails.OperatingSystem = await GetInstanceOperatingSystem(instanceId);

            // Cpu Usage
            instanceDetails.CpuStatistics = await GetInstanceCPUStatistics(instanceId);

            //Memory 

            // Volume Usage
            //instanceDetails.TotalVolumesSize = GetInstanceTotalVolumesSize(instanceId);

            // Price
            instanceDetails.Price = await _costExplorerService.GetInstancePrice(instanceId);

            return instanceDetails;

        }

        //notice that this method does not return the total volume size, still has work to do//
        public async Task<double> GetCurrentInstanceVolumesUsage(string instanceId)
        {
            _awsScraper.ScrapeInstancePrices();
            return await _cloudWatchService.GetCurrentInstanceVolumesUsage(instanceId);
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
