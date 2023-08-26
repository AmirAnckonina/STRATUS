

using DTO;
using MongoDB.Bson;
using System.Timers;
using Utils.Enums;

namespace StratusApp.Services.AlertsService
{
    public class AlertTimer
    {
        private System.Timers.Timer _timer;
        private readonly Dictionary<eAlertType, int> _alertTypesConverter;

        public EventHandler AlertTimerElapsed { get; set; }
        public string UserEmail { get; set; }

        public AlertTimer(string userEmail, AlertsConfigurations alertsConfigurations)
        {
            UserEmail = userEmail;
            
            _alertTypesConverter = new Dictionary<eAlertType, int>()
            {
                [eAlertType.CPU] = alertsConfigurations.CpuThreshold,
                [eAlertType.STORAGE] = alertsConfigurations.DiskThreshold,
                [eAlertType.MEMORY] = alertsConfigurations.MemoryThreshold,
            };

            InitTimer(alertsConfigurations.IntervalTimeMilisec);
        }

        private void InitTimer(long intervalTimeMilisec)
        {
            _timer = new System.Timers.Timer();
            _timer.Interval = intervalTimeMilisec;
            _timer.Elapsed += timer_Elapsed;

            StartTimer();
        }

        public void StartTimer()
        {
            Task.Run(() =>
            {
                _timer?.Start();
            });
        }

        private void timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            AlertTimerElapsed?.Invoke(this, new EventArgs());
        }

        public void StopTimer()
        {
            _timer?.Stop();
            _timer.Elapsed -= timer_Elapsed;
        }

        public void UpdateIntervalTime(long interval)
        {
            _timer.Interval = interval;
        }

        public void UpdateThresholdValue(int value, eAlertType type)
        {
            _alertTypesConverter[type] = value;  
        }

        public int GetThresholdValueByType(eAlertType type)
        {
            return _alertTypesConverter[type];
        }
    }
}
