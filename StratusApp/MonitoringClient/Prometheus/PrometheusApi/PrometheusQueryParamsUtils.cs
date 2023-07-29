using MonitoringClient.Prometheus.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Enums;

namespace MonitoringClient.Prometheus.PrometheusApi
{
    internal static class PrometheusQueryParamsUtils
    {
        public static string ParseTimePeriodToPrometheusTimeFilterFormat(QueryOverTimePeriod timePeriod)
        {
            string timeFilter;

            switch (timePeriod)
            {
                case QueryOverTimePeriod.Year:
                    timeFilter = "1y";
                    break;

                case QueryOverTimePeriod.Day:
                    timeFilter = "1d";
                    break;

                case QueryOverTimePeriod.Month:
                default:
                    timeFilter = "30d";
                    break;
            }

            return timeFilter;
        }

        public static PrometheusQueryType ParseQueryTypeStrToQueryTypeEnum(string queryTypeStr)
        {
            PrometheusQueryType queryType =
                (PrometheusQueryType)Enum.Parse(typeof(PrometheusQueryType), queryTypeStr);

            return queryType;
        }

        public static string GetExperssionQueryString(PrometheusExpressionQueryType expQueryType)
        {
            string expQueryTypeStr;

            switch (expQueryType)
            {
                case PrometheusExpressionQueryType.RangeQuery:
                    expQueryTypeStr = $"query_range";
                    break;

                case PrometheusExpressionQueryType.InstantQuery:
                    expQueryTypeStr = $"query";
                    break;

                default:
                    throw new Exception("Unknown expQueryType");
            }

            return expQueryTypeStr;
        }

        public static string ParseDateTimeToPromQLDateTimeStrFormat(DateTime dateTime)
        {
            string formattedDateTime = dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            return formattedDateTime;
        }



    }
}
