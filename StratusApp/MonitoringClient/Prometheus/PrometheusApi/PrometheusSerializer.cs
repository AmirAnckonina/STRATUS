using MonitoringClient.Prometheus.Enums;
using MonitoringClient.Prometheus.PrometheusConverter;
using MonitoringClient.Prometheus.PrometheusModels.GeneralResponseModels;
using MonitoringClient.Prometheus.PrometheusModels.MetricModels;
using MonitoringClient.Prometheus.PrometheusModels.SingleResultModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.DevTools.V112.Debugger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MonitoringClient.Prometheus.PrometheusApi
{
    public class PrometheusSerializer
    {
        public PrometheusSerializer() { }

        public List<T> DeserializeRawResponse<T>(PrometheusQueryType queryType, string respContent)
        {
            BasePrometheusResultConverter baseConverter;

            PrometheusConcreteResultType concreteResultType = GetPrometheusConcreteResultByQueryType(queryType);

            switch (concreteResultType)
            {
                case PrometheusConcreteResultType.EmptyMetricAndSingleValue:
                    baseConverter = new BasePrometheusResultConverter(PrometheusConcreteResultType.EmptyMetricAndSingleValue);
                    break;

                case PrometheusConcreteResultType.EmptyMetricAndValuesList:
                    baseConverter = new BasePrometheusResultConverter(PrometheusConcreteResultType.EmptyMetricAndValuesList);
                    break;

                case PrometheusConcreteResultType.CpuMetricAndSingleValue:
                    baseConverter = new BasePrometheusResultConverter(PrometheusConcreteResultType.CpuMetricAndSingleValue);
                    break;

                default:
                    throw new Exception();

            }

            JObject rawJObj = JObject.Parse(respContent);
            JToken resultToken = rawJObj["data"]?["result"];
            string resultTokenStr = resultToken.FirstOrDefault().ToString();

            if (typeof(T) == typeof(EmptyMetricAndSingleValueResult))
            {
                EmptyMetricAndSingleValueResult a = JsonConvert.DeserializeObject<EmptyMetricAndSingleValueResult>(resultTokenStr);
            }
            T? promResult = JsonConvert.DeserializeObject<T>(resultTokenStr, baseConverter);

            return new List<T> { promResult };

        }

        private PrometheusConcreteResultType GetPrometheusConcreteResultByQueryType(PrometheusQueryType queryType)
        {
            switch (queryType)
            {
                case PrometheusQueryType.GetAvgCpuUsageUtilization:
                case PrometheusQueryType.GetMaxCpuUsageUtilization:
                case PrometheusQueryType.GetTotalMemorySizeInGB:
                case PrometheusQueryType.GetAvgFreeMemorySizeInGB:
                case PrometheusQueryType.GetTotalDiskSizeInGB:
                case PrometheusQueryType.GetAvgFreeDiskSpaceInGB:
                    return PrometheusConcreteResultType.EmptyMetricAndSingleValue;

                default:
                    return PrometheusConcreteResultType.EmptyMetricAndValuesList;


            }
        }
    }
}
