using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitoringClient.Prometheus.PrometheusModels.SingleResultModels;
using MonitoringClient.Prometheus.Enums;


namespace MonitoringClient.Prometheus.PrometheusConverter
{
    public class BasePrometheusResultConverter : JsonConverter
    {
        private PrometheusConcreteResultType _concreteResultType;

        public BasePrometheusResultConverter(PrometheusConcreteResultType concreteResultType) 
        {
            _concreteResultType = concreteResultType;
        }

        static JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new BasePrometheusResultSpecifiedConcreteClassConverter() };

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(BasePrometheusResult));
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);

            switch (_concreteResultType)
            {
                case PrometheusConcreteResultType.EmptyMetricAndSingleValue:

                    return JsonConvert.DeserializeObject<EmptyMetricAndSingleValue>(
                        jo.ToString(), SpecifiedSubclassConversion);

                case PrometheusConcreteResultType.EmptyMetricAndValuesList:
                    return JsonConvert.DeserializeObject<EmptyMetricAndValuesList>(
                        jo.ToString(), SpecifiedSubclassConversion);

                case PrometheusConcreteResultType.ListOfCpuMetricAndSingleValue:
                    return JsonConvert.DeserializeObject<List<CpuMetricAndSingleValue>>(
                        jo.ToString(), SpecifiedSubclassConversion);
                default:
                    throw new Exception();
            }
            throw new NotImplementedException();
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException(); // won't be called because CanWrite returns false
        }
    }
}
