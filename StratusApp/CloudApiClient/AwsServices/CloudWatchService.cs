using Amazon;
using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using Amazon.Runtime;
using CloudApiClient.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudApiClient.AwsServices
{
    public class CloudWatchService
    {
        private AmazonCloudWatchClient _cloudWatchClient;

        public CloudWatchService(AWSCredentials credentials, RegionEndpoint region) 
        {
            _cloudWatchClient = new AmazonCloudWatchClient(credentials, region);
        }

        public async Task<List<double>> GetInstanceCpuUsageOverTime(string instanceId)
        {
            var cpuUsageDataByDays = new List<CpuUsageData>();

            if (instanceId == null)
            {
                return new List<double>();
            }

            // Set the dimensions for the CPUUtilization metric
            var dimensions = new List<Amazon.CloudWatch.Model.Dimension>()
            {
                new Amazon.CloudWatch.Model.Dimension() { Name = "InstanceId", Value = instanceId }
            };

            //calculate the total days past in the month
            DateTime currentDate = DateTime.Today;
            int daysPassed = currentDate.Day;

            // Set the start and end time for the metric data
            var startTime = DateTime.UtcNow.AddDays(daysPassed * -1);
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

        //Please NOTE to change the hard-coded instanceID
        public async Task<List<Datapoint>> GetInstanceCPUStatistics(string instanceId)
        {
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


        // CHEN implementation
        //notice that this method does not return the total volume size, still has work to do//
        public async Task<double> GetCurrentInstanceVolumesUsage(string instanceId)
        {
            var getMetricStatisticsRequest = new GetMetricStatisticsRequest
            {
                Namespace = "AWS/EC2",
                MetricName = "CPUUtilization",
                Dimensions = new List<Amazon.CloudWatch.Model.Dimension>
                {
                    new Amazon.CloudWatch.Model.Dimension
                    {
                        Name = "InstanceId",
                        Value = instanceId
                    }
                },
                Statistics = new List<string> { "Average" },
                Period = 300, // 5 minutes
                StartTime = DateTime.UtcNow.AddMinutes(-5),
                EndTime = DateTime.UtcNow
            };

            var getMetricStatisticsResponse = await _cloudWatchClient.GetMetricStatisticsAsync(getMetricStatisticsRequest);

            var dataPoints = getMetricStatisticsResponse.Datapoints;

            if (dataPoints.Any())
            {
                var latestDataPoint = dataPoints.OrderByDescending(dp => dp.Timestamp).FirstOrDefault();

                if (latestDataPoint != null)
                {
                    return latestDataPoint.Average;
                }
            }
            return 0;
        }
    }
}
