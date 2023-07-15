using System.Timers;
using Utils.DTO;

namespace MonitoringClient
{
    internal class AlertManager
    {
        private System.Timers.Timer _timer;
        private const int CPU_PERCENTAGE_THRESHOLD = 70;
        private const int MEMORY_PERCENTAGE_THRESHOLD = 70;
        private const int STORAGE_PERCENTAGE_THRESHOLD = 70;

        private readonly List<AlertData> _alerts = new List<AlertData>();

        public AlertManager()
        {
            InitTimer();
        }

        internal List<AlertData> GetAlertTable()
        {
            //TBD: return alert table
            return _alerts;
        }

        private void InitTimer(double interval = 1000 * 60)
        {
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
            _alerts.Add(new AlertData()
            {
                MachineId = "1",
                Type = eAlertType.CPU,
                CreationTime = DateTime.Now,
                UnderUsageDetectedTime = DateTime.Now.AddMinutes(-10)
            });
        }

        ~AlertManager() { _timer.Stop(); }
    }
}
