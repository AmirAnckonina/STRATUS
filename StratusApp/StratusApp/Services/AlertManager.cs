using System.Timers;
using Utils.DTO;

namespace MonitoringClient
{
    internal class AlertManager
    {
        private System.Timers.Timer _timer;
        private readonly PrometheusClient _prometheusClient;
        private readonly Dictionary<eAlertType, int> _alertTypesConverter;


        private const int CPU_PERCENTAGE_THRESHOLD = 70;
        private const int MEMORY_PERCENTAGE_THRESHOLD = 70;
        private const int STORAGE_PERCENTAGE_THRESHOLD = 70;

        private readonly List<AlertData> _alerts = new List<AlertData>();

        public AlertManager(PrometheusClient prometheusClient)
        {
            _prometheusClient = prometheusClient;
            _alertTypesConverter = new Dictionary<eAlertType, int>()
            {
                [eAlertType.CPU] =  CPU_PERCENTAGE_THRESHOLD,
                [eAlertType.STORAGE] = STORAGE_PERCENTAGE_THRESHOLD,
                [eAlertType.MEMORY] =MEMORY_PERCENTAGE_THRESHOLD,
            };

            InitTimer();
        }

        internal List<AlertData> GetAlertTable()
        {
            //TBD: return alert table
            return _alerts;
        }

        private void InitTimer(double interval = 1000 * 60)
        {
            // update table with new data and delete records that machine id that was terminated
            _timer = new System.Timers.Timer();
            _timer.Interval = interval; // should be confiugre by the user
            _timer.Elapsed += timer_Elapsed;
            _timer.Start();
        }

        private void timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            //TBD:
            // send request to promethius
            // processing the response
            // update table with new data and delete records that machine id that was terminated

            //foreach user:
            //foreach user instance:


            double avgCpuUsageUtilization = _prometheusClient.GetAvgCpuUsageUtilization("id", "1d").Result;
            double avgFreeDiskSpaceInGB = _prometheusClient.GetAvgFreeDiskSpaceInGB("id", "1d").Result;
            double avgFreeMemorySizeInGB = _prometheusClient.GetAvgFreeMemorySizeInGB("id", "1d").Result;
            
            DetectAndInsertLowUsage("1", avgCpuUsageUtilization,eAlertType.CPU);
            DetectAndInsertLowUsage("1", avgFreeDiskSpaceInGB, eAlertType.STORAGE);
            DetectAndInsertLowUsage("1", avgFreeMemorySizeInGB, eAlertType.MEMORY);
        }

        private void DetectAndInsertLowUsage(string machineId, double avgUsage, eAlertType eAlertType)
        {
            if (avgUsage <= _alertTypesConverter[eAlertType])
            {
                _alerts.Add(new AlertData()
                {
                    MachineId = machineId,
                    Type = eAlertType,
                    CreationTime = DateTime.Now,
                    PercentageUsage = avgUsage,
                });
            }
        }

        ~AlertManager() { _timer.Stop(); }
    }
}
