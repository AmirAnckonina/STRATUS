using Utils.DTO;
using Utils.Enums;

namespace StratusApp.Services.Collector
{
    public class CollectorUtils
    {
        public CollectorUtils()
        {

        }

        public QueryOverTimePeriod ParseTimePeriodStrToTimePeriodEnum(string timePeriodStr)
        {
            QueryOverTimePeriod timePeriod = (QueryOverTimePeriod)Enum.Parse(typeof(QueryOverTimePeriod), timePeriodStr, true);
            return timePeriod;
        }

        public (DateTime, QueryOverTimePeriod) BuildTimeRangeQueryByTimePeriod(QueryOverTimePeriod overallTimePeriod, DateTime endTime)
        {
            DateTime startTime;
            QueryOverTimePeriod queryStep;

            switch (overallTimePeriod)
            {
                case QueryOverTimePeriod.Day:
                    startTime = endTime.AddDays(-1);
                    queryStep = QueryOverTimePeriod.Hour;
                    break;

                case QueryOverTimePeriod.Year:
                    startTime = endTime.AddYears(-1);
                    queryStep = QueryOverTimePeriod.Month;
                    break;

                case QueryOverTimePeriod.Month:
                case QueryOverTimePeriod.None:
                    startTime = endTime.AddMonths(-1);
                    queryStep = QueryOverTimePeriod.Day;
                    break;

                default:
                    throw new Exception("Query overall time isn't supported.");
            }

            return (startTime, queryStep);
        }

        public List<CpuUsageData> FillCpuUsageDataList(List<List<string>>? timestampsAndValues)
        {
            List<CpuUsageData> cpuUsageDataList = new List<CpuUsageData>();

            foreach (List<string> tsAndVal in timestampsAndValues)
            {
                cpuUsageDataList.Add(new CpuUsageData
                {
                    Date = tsAndVal[0],
                    Usage = double.Parse(tsAndVal[1])
                });
            }

            return cpuUsageDataList;
        }
    }
}
