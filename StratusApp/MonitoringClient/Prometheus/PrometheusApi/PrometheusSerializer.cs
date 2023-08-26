using MonitoringClient.Prometheus.Enums;
using MonitoringClient.Prometheus.PrometheusConverter;
using MonitoringClient.Prometheus.PrometheusModels.OldPrometheusModels_bkup;
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
using MonitoringClient.Prometheus.Exceptions;

namespace MonitoringClient.Prometheus.PrometheusApi
{
    public class PrometheusSerializer
    {
        public PrometheusSerializer() { }

        public T DeserializeRawResponse<T>(PrometheusQueryType queryType, string respContent)
        {
            //BasePrometheusResultConverter baseConverter;
            string resultTokenStr;
            JObject rawJObj = JObject.Parse(respContent);
            JToken resultToken = rawJObj["data"]?["result"];

            PrometheusConcreteResultType concreteResultType = GetPrometheusConcreteResultByQueryType(queryType);

            switch (concreteResultType)
            {
                case PrometheusConcreteResultType.EmptyMetricAndSingleValue:
                    resultTokenStr = resultToken?.FirstOrDefault()?.ToString();
                    //baseConverter = new BasePrometheusResultConverter(PrometheusConcreteResultType.EmptyMetricAndSingleValue);
                    break;

                case PrometheusConcreteResultType.EmptyMetricAndValuesList:
                    resultTokenStr = resultToken?.FirstOrDefault()?.ToString();
                    //baseConverter = new BasePrometheusResultConverter(PrometheusConcreteResultType.EmptyMetricAndValuesList);
                    break;

                case PrometheusConcreteResultType.ListOfCpuMetricAndSingleValue:
                    resultTokenStr = resultToken?.ToString();
                    //baseConverter = new BasePrometheusResultConverter(PrometheusConcreteResultType.ListOfCpuMetricAndSingleValue);
                    break;

                default:    
                    throw new PrometheusClientException("Unsupported result for the requested query."); 

            }
        
/*            if (typeof(T) == typeof(EmptyMetricAndSingleValueResult))
            {
                EmptyMetricAndSingleValueResult a = JsonConvert.DeserializeObject<EmptyMetricAndSingleValueResult>(resultTokenStr);
            }*/

            if (resultTokenStr != null)
            {
                return JsonConvert.DeserializeObject<T>(resultTokenStr); //, baseConverter);
            }
            else
            {
                throw new PrometheusClientException("there's no data to present, probably the query result is empty.");
            }

        }

        private PrometheusConcreteResultType GetPrometheusConcreteResultByQueryType(PrometheusQueryType queryType)
        {
            switch (queryType)
            {

                case PrometheusQueryType.GetAvgCpuUsageUtilizationOverTime:
                    return PrometheusConcreteResultType.EmptyMetricAndValuesList;


                case PrometheusQueryType.GetAvgCpuUtilizationByCpu:
                case PrometheusQueryType.GetNumberOfvCPU:
                    return PrometheusConcreteResultType.ListOfCpuMetricAndSingleValue;

                default:
                    return PrometheusConcreteResultType.EmptyMetricAndSingleValue;
            }
        }
    }
}
