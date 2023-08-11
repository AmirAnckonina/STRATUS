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
                // Convert Unix timestamp to DateTime
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(long.Parse(tsAndVal[0]));

                // If you want to work with a DateTime object without the offset information
                DateTime dateTime = dateTimeOffset.UtcDateTime;

                cpuUsageDataList.Add(new CpuUsageData
                {
                    Date = dateTime.ToString(),
                    Usage = double.Parse(tsAndVal[1])
                });
            }

            return cpuUsageDataList;
        }
    }
}
