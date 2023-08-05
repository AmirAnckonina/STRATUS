using MonitoringClient.Prometheus.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringClient.Prometheus.PrometheusApi
{
    public class QueryBuilderMapper
    {
        private readonly Dictionary
            <PrometheusQueryType, Func<PrometheusQueryParams, string>> _queryBuilderMapper;

        public QueryBuilderMapper()
        {
            _queryBuilderMapper = new Dictionary<PrometheusQueryType, Func<PrometheusQueryParams, string>>() { };
           

        }

        /*public static Func<PrometheusQueryParams, string> GetQueryBuilderFunc(PrometheusQueryType queryType)
        {
            //return _queryBuilderMapper[queryType];
        }*/
    }
}
