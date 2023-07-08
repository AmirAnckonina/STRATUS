using Amazon;
using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using Amazon.Runtime;
using CloudApiClient.DTO;

namespace CloudApiClient.AwsServices
{
    public class CloudWatchService
    {
        private AmazonCloudWatchClient _cloudWatchClient;

        public CloudWatchService(AWSCredentials credentials, RegionEndpoint region) 
        {
            _cloudWatchClient = new AmazonCloudWatchClient(credentials, region);
        }

        public async Task<List<CpuUsageData>> GetInstanceCpuUsageOverTime(string instanceId, string filterTime)
        {
            if (instanceId == null || instanceId.Equals("undifined"))
            {
                return new List<CpuUsageData>();   
            }

            // Set the dimensions for the CPUUtilization metric
            //var dimensions = new List<Dimension>()
            //{
            //    new Dimension() { Name = "InstanceId", Value = instanceId }
            //};

            var periodDuration = GetSecondsPassed(filterTime);

            DateTime currentStartTime = periodDuration.startTime;
            DateTime endTime = DateTime.UtcNow;
            DateTime currentEndTime = filterTime == "Day" ? currentStartTime.AddHours(1) : filterTime == "Year" ? currentStartTime.AddMonths(1) : currentStartTime.AddDays(1);
            List<CpuUsageData> array = new List<CpuUsageData>();

            while (currentStartTime <= endTime)
            {
                // Create a request to get the CPUUtilization metric data
                var request = new GetMetricStatisticsRequest()
                {
                    StartTimeUtc = currentStartTime,
                    EndTimeUtc = currentEndTime,
                    Period = periodDuration.secondsPassed,
                    Namespace = "AWS/EC2",
                    MetricName = "CPUUtilization",
                    Dimensions = new List<Dimension>
                {
                    new Dimension
                    {
                        Name = "InstanceId",
                        Value = instanceId
                    }
                },
                    Statistics = new List<string> { "Average" },
                };                               
                
                try
                {
                    var response = await _cloudWatchClient.GetMetricStatisticsAsync(request);
                    CpuUsageData usageData = null;

                    if (response.Datapoints.Count == 0)
                    {
                        if (filterTime == "Month" || filterTime == "Week")
                        {
                            usageData = new CpuUsageData()
                            {
                                Date = $"{currentStartTime.Day}.{currentStartTime.Month}",
                                Usage = 0,
                            };
                        }
                        else if(filterTime == "Year")
                        {
                            usageData = new CpuUsageData()
                            {
                                Date = $"{currentStartTime.Month}",
                                Usage = 0,
                            };
                        }
                        else
                        {
                            usageData = new CpuUsageData()
                            {
                                Date = $"{currentStartTime.Hour}:{currentStartTime.Minute}",
                                Usage = 0,
                            };
                        }
                    }
                    else
                    {
                        foreach (var result in response.Datapoints)
                        {
                            if (filterTime != "Day" && (result.Timestamp < endTime || currentStartTime == endTime))
                            {
                                if (filterTime == "Year")
                                {
                                    usageData = new CpuUsageData()
                                    {
                                        Date = $"{result.Timestamp.Month}",
                                        Usage = result.Average * 100,
                                    };
                                }
                                else
                                {
                                    usageData = new CpuUsageData()
                                    {
                                        Date = $"{result.Timestamp.Day}.{result.Timestamp.Month}",
                                        Usage = result.Average * 100,
                                    };
                                }
                            }
                            else if(filterTime == "Day")
                            {
                                usageData = new CpuUsageData()
                                {
                                    Date = $"{result.Timestamp.Hour}:{result.Timestamp.Minute}",
                                    Usage = result.Average * 100,
                                };
                            }                               
                        }
                    }

                    array.Add(usageData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                currentStartTime = filterTime == "Day" ? currentStartTime.AddHours(1) : filterTime == "Year" ? currentStartTime.AddMonths(1): currentStartTime.AddDays(1);
                currentEndTime = filterTime == "Day" ? currentEndTime.AddHours(1) : filterTime == "Year" ? currentEndTime.AddMonths(1): currentEndTime.AddDays(1);
            }

            return array;
        }

        public (int secondsPassed, DateTime startTime) GetSecondsPassed(string timePeriod)
        {
            DateTime now = DateTime.UtcNow; // Get the current UTC time

            DateTime startTime;
            int secondsPassed;

            switch (timePeriod)
            {
                case "Day":
                    startTime = now.Date; // Get the start of the current day
                    secondsPassed = 60 * 60;
                    break;
                case "Week":
                    startTime = now.Date.AddDays(-(int)now.DayOfWeek); // Get the start of the current week
                    secondsPassed = 60 * 24 * 60;
                    break;
                case "Month":
                    startTime = now.Date.AddDays(-30); // Get the start of the current month
                    secondsPassed = 60 * 24 * 60;
                    break;
                case "Year":
                    startTime = now.Date.AddDays(-(int)now.DayOfYear).AddDays(1); ; // Get the start of the current year
                    secondsPassed = 60 * 24 * 30;
                    break;
                default:
                    startTime = DateTime.MinValue; // Invalid time period, set startTime to DateTime.MinValue
                    secondsPassed = 0; // Invalid time period, set secondsPassed to 0
                    break;
            }

            return (secondsPassed, startTime);
        }



        private int getPeriodicTime(string filterTime)
        {
            int periodicTTime = 0;

            switch(filterTime)
            {
                case "DAY":
                    periodicTTime = 24 * 3600;
                    break;
                case "WEEK":
                    periodicTTime = 24 * 3600;
                    break;
                case "Month":
                    periodicTTime = 24 * 3600;
                    break;

            }

            return periodicTTime;
        }

        //Please NOTE to change the hard-coded instanceID
        public async Task<List<Datapoint>> GetInstanceCPUStatistics(string instanceId)
        {
            // Get the EC2 instance usage data
            if (instanceId == null) return new List<Datapoint>();

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
