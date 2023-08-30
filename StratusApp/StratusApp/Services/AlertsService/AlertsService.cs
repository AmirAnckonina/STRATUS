using DTO;
using MongoDB.Driver;
using OpenQA.Selenium;
using StratusApp.Services.Collector;
using StratusApp.Services.MongoDBServices;
using System.Timers;
using Utils.DTO;
using Utils.Enums;
using Utils.Utils;

namespace StratusApp.Services.AlertsService
{
    public class AlertsService
    {
        private readonly MongoDBService _mongoDatabase;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CollectorService _collectorService;        
        private readonly AlertsManager _alertsManager;
        private readonly object _alertLock = new object();

        private const string INTERVAL_FILTER = "month";        

        public EmailService EmailService { get; internal set; }

        public AlertsService(MongoDBService mongoDatabase, CollectorService collectorService, IHttpContextAccessor httpContextAccessor)
        {
            _mongoDatabase = mongoDatabase;
            _httpContextAccessor = httpContextAccessor;
            _alertsManager = new AlertsManager();
            _collectorService = collectorService;            

            InitTimersAsync();
        }

        private async void InitTimersAsync()
        {
            var configurations = await _mongoDatabase.GetCollectionAsList<AlertsConfigurations>(eCollectionName.AlertConfigurations);

            foreach (var config in configurations)
            {
                InitTimerByUserConfig(config);
            }
        }

        private void InitTimerByUserConfig(AlertsConfigurations userAlertConfig)
        {
            //string userSession = _httpContextAccessor.HttpContext.Request.Cookies["Stratus"];
            
            if (userAlertConfig != null)
            {
                var alertTimer = new AlertTimer(userAlertConfig.UserEmail, userAlertConfig);

                _alertsManager.Add(new KeyValuePair<string, AlertTimer>(userAlertConfig.UserEmail, alertTimer));
                _alertsManager.AlertTimerElapsed += alertsManager_AlertTimerElapsed;
            }
        }

        private void alertsManager_AlertTimerElapsed(object? sender, EventArgs e)
        {
            AlertTimer alertTimer = sender as AlertTimer;
            List<AlertData> alerts = new List<AlertData>();

            if(alertTimer != null)
            {
                lock (_alertLock)
                {
                    var instances = _mongoDatabase.GetCollectionAsList<AwsInstanceDetails>(eCollectionName.Instances).Result;

                    foreach (var instance in instances)
                    {
                        if (instance.UserEmail == alertTimer.UserEmail)
                        {
                            double avgCpuUsageUtilization = _collectorService.GetAvgCpuUsageUtilization(instance.InstanceAddress, INTERVAL_FILTER).Result;
                            double avgFreeDiskSpaceInGB = _collectorService.GetAvgDiskSpaceUsagePercentage(instance.InstanceAddress, INTERVAL_FILTER).Result;
                            double avgFreeMemorySizeInGB = _collectorService.GetAvgMemorySizeUsagePercentage(instance.InstanceAddress, INTERVAL_FILTER).Result;

                            DetectAndInsertLowUsage(alerts, alertTimer.UserEmail, instance.InstanceAddress, avgCpuUsageUtilization, eAlertType.CPU);
                            DetectAndInsertLowUsage(alerts, alertTimer.UserEmail, instance.InstanceAddress, avgFreeDiskSpaceInGB, eAlertType.STORAGE);
                            DetectAndInsertLowUsage(alerts, alertTimer.UserEmail, instance.InstanceAddress, avgFreeMemorySizeInGB, eAlertType.MEMORY);
                        }
                    }

                    //Send mail to user
                    EmailService?.SendAlertsEmailAsync(alertTimer.UserEmail, alerts);

                    InsertAlertsToDB(alerts);
                }
            }
        }

        internal async Task<List<AlertData>> GetAlertsCollection()
        {
            var result = new List<AlertData>();
            string userSession = GetUserSession();
            var alertsData = await _mongoDatabase.GetCollectionAsList<AlertData>(eCollectionName.Alerts);

            foreach (var alert in alertsData)
            {
                if (alert != null && alert.Email == userSession)
                {
                    result.Add(alert);
                }
            }

            return result;
        }

        private string GetUserSession()
        {
            return SessionUtils.GetSessionId(_httpContextAccessor);
        }

        private async void InsertAlertsToDB(List<AlertData> alerts)
        {
            await _mongoDatabase.InsertMultipleDocuments(eCollectionName.Alerts, alerts);
        }

        private void DetectAndInsertLowUsage(List<AlertData> alerts, string userEmail, string machineId, double avgUsage, eAlertType eAlertType)
        {
            if (avgUsage <= _alertsManager.GetThresholdValue(userEmail, eAlertType))
            {
                alerts.Add(new AlertData()
                {
                    Email = userEmail,
                    MachineId = machineId,
                    Type = eAlertType,
                    CreationTime = DateTime.Now,
                    PercentageUsage = avgUsage,
                });
            }
        }

        internal bool SetConfigurations(AlertsConfigurations alertsConfigurations)
        {
            bool result = false;
            string userSession = GetUserSession();

            alertsConfigurations.UserEmail = userSession;

            try
            {
                SetIntervalTime(alertsConfigurations);
                SetThresholdValues(alertsConfigurations);
                var deleteResult = ResetAlertsConfigurationsCollection();

                if (deleteResult.IsAcknowledged)
                {
                    InsertAlertsConfigurationsToDB(alertsConfigurations);

                    result = true;
                }
            }
            catch { }

            return result;
        }

        private void SetThresholdValues(AlertsConfigurations alertsConfigurations)
        {
            _alertsManager.UpdateThresholdValue(alertsConfigurations.UserEmail, alertsConfigurations.CpuThreshold, eAlertType.CPU);
            _alertsManager.UpdateThresholdValue(alertsConfigurations.UserEmail, alertsConfigurations.MemoryThreshold, eAlertType.MEMORY);
            _alertsManager.UpdateThresholdValue(alertsConfigurations.UserEmail, alertsConfigurations.DiskThreshold, eAlertType.STORAGE);
        }

        private async void InsertAlertsConfigurationsToDB(AlertsConfigurations alertsConfigurations)
        {
            //insert for specific id

            await _mongoDatabase.InsertDocument(eCollectionName.AlertConfigurations, alertsConfigurations);
        }

        private DeleteResult ResetAlertsConfigurationsCollection()
        {
            // need to reset only for specific user !!

            return _mongoDatabase.DeleteDocuments(eCollectionName.AlertConfigurations, Builders<AlertsConfigurations>.Filter.Empty).Result;
        }

        private void SetIntervalTime(AlertsConfigurations alertsConfigurations)
        {
            switch (alertsConfigurations.IntervalPeriod)
            {
                case "hour":
                    alertsConfigurations.IntervalTimeMilisec = 1000 * 60 * 60 * alertsConfigurations.IntervalPeriodValue; break;
                case "day":
                    alertsConfigurations.IntervalTimeMilisec = 1000 * 60 * 60 * 24 * alertsConfigurations.IntervalPeriodValue; break;
                case "week":
                    alertsConfigurations.IntervalTimeMilisec = 1000 * 60 * 60 * 24 * 7 * alertsConfigurations.IntervalPeriodValue; break;
                case "month":
                    alertsConfigurations.IntervalTimeMilisec = 1000 * 60 * 60 * 24 * 20 * alertsConfigurations.IntervalPeriodValue; break;
            }

            _alertsManager.UpdateIntervalTime(alertsConfigurations.UserEmail, alertsConfigurations.IntervalTimeMilisec);
        }

        public Task RegisterToAlerts()
        {
            string userSession = GetUserSession();
            AlertsConfigurations alertsConfigurations = new AlertsConfigurations(); //default values

            alertsConfigurations.UserEmail = userSession;
            InitTimerByUserConfig(alertsConfigurations);
            SetConfigurations(alertsConfigurations);

            return Task.CompletedTask;

        }
    }
}
