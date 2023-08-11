using System.Globalization;
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
                case QueryOverTimePeriod.Week:
                    startTime = endTime.AddDays(-7);
                    queryStep = QueryOverTimePeriod.Day;
                    break;
                default:
                    throw new Exception("Query overall time isn't supported.");
            }

            return (startTime, queryStep);
        }

        public List<CpuUsageData> FillCpuUsageDataList(DateTime startTime, DateTime endTime, QueryOverTimePeriod step, List<List<string>>? timestampsAndValues)
        {
            List<CpuUsageData> cpuUsageDataList = new List<CpuUsageData>();

            Dictionary<DateTime, double> timestampValueMap = new Dictionary<DateTime, double>();

            foreach (List<string> tsAndVal in timestampsAndValues)
            {
                DateTime dateTime = ConvertTimestampToDateTime(tsAndVal[0]);
                double value = double.Parse(tsAndVal[1]);

                // Group timestamps by the step (hour, day, month, etc.)
                DateTime groupedDateTime = GroupDateTimeByStep(step, dateTime);

                timestampValueMap[groupedDateTime] = value;
            }

            DateTime loopDateTime = startTime;

            while (loopDateTime <= endTime)
            {
                // Group loop DateTime by the step (hour, day, month, etc.)
                DateTime groupedLoopDateTime = GroupDateTimeByStep(step, loopDateTime);

                double value = 0.0;

                if (timestampValueMap.ContainsKey(groupedLoopDateTime))
                {
                    value = timestampValueMap[groupedLoopDateTime];
                }

                cpuUsageDataList.Add(new CpuUsageData
                {
                    Date = FormatDateBasedOnStep(step, loopDateTime),
                    Usage = value
                });

                IncrementLoopDateTime(step, ref loopDateTime);
            }

            return cpuUsageDataList;
        }

        // Helper method to group DateTime based on the step
        private DateTime GroupDateTimeByStep(QueryOverTimePeriod step, DateTime dateTime)
        {
            switch (step)
            {
                case QueryOverTimePeriod.Hour:
                    return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
                case QueryOverTimePeriod.Day:
                    return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
                case QueryOverTimePeriod.Week:
                    return dateTime.Date.AddDays(-(int)dateTime.DayOfWeek);
                case QueryOverTimePeriod.Month:
                    return new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0);
                case QueryOverTimePeriod.Year:
                    return new DateTime(dateTime.Year, 1, 1, 0, 0, 0);
                default:
                    return dateTime;
            }
        }


        // Helper method to format the date based on the step
        private string FormatDateBasedOnStep(QueryOverTimePeriod step, DateTime dateTime)
        {
            CultureInfo englishCulture = new CultureInfo("en-US"); // Set CultureInfo to English

            switch (step)
            {
                case QueryOverTimePeriod.Hour:
                    return dateTime.ToString("HH:mm", englishCulture);
                case QueryOverTimePeriod.Day:
                case QueryOverTimePeriod.Week:
                case QueryOverTimePeriod.Month:
                    return dateTime.ToString("dd MMM", englishCulture);
                case QueryOverTimePeriod.Year:
                    return dateTime.ToString("MMM", englishCulture);
                default:
                    return dateTime.ToString(englishCulture);
            }
        }

        // Helper method to increment the loop DateTime based on the step
        private void IncrementLoopDateTime(QueryOverTimePeriod step, ref DateTime loopDateTime)
        {
            switch (step)
            {
                case QueryOverTimePeriod.Hour:
                    loopDateTime = loopDateTime.AddHours(1);
                    break;
                case QueryOverTimePeriod.Day:
                case QueryOverTimePeriod.Week:
                    loopDateTime = loopDateTime.AddDays(1);
                    break;
                case QueryOverTimePeriod.Month:
                    loopDateTime = loopDateTime.AddMonths(1);
                    break;
            }
        }


        private static DateTime ConvertTimestampToDateTime(string timestampAsString)
        {
            // Convert Unix timestamp to DateTime
            long timestamp = (long)double.Parse(timestampAsString);
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);

            // If you want to work with a DateTime object without the offset information
            DateTime dateTime = dateTimeOffset.UtcDateTime;

            return dateTime;
        }
    }
}
