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

        //public event 

        public AlertManager()
        {
            InitTimer();
        }

        internal List<AlertData> GetAlertTable()
        {
            //TBD: return alert table
            return new List<AlertData> {};
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
        }

        ~AlertManager() { _timer.Stop(); }
    }
}
