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
            QueryOverTimePeriod timePeriod = (QueryOverTimePeriod)Enum.Parse(typeof(QueryOverTimePeriod), timePeriodStr);
            return timePeriod;
        }

        /*public QueryOverTimePeriod GetSingleQueryOverTimePeriod(QueryOverTimePeriod overallQueryTimePeriod)
        {
            QueryOverTimePeriod singleOverTime;


            return singleOverTime;
        }

        public QueryTimeOffsetType GetTimeOffsetType(QueryOverTimePeriod overallQueryTimePeriod)
        {
            QueryTimeOffsetType timeOffsetType;


            return timeOffsetType;
        }*/


    }
}
