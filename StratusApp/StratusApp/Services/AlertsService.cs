using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using OpenQA.Selenium;
using StratusApp.Data;
using StratusApp.Models;
using StratusApp.Models.Responses;
using StratusApp.Services.MongoDBServices;
using System.Reflection.PortableExecutable;
using System;
using System.Timers;
using Utils.DTO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;
using DTO;
using Amazon.Runtime.Documents;
using MongoDB.Driver;
using System.Linq.Expressions;
using MonitoringClient.Prometheus;
using MonitoringClient.Prometheus.PrometheusApi;
using Utils.Enums;
using StratusApp.Services.Collector;
using System.Collections.Concurrent;

namespace StratusApp.Services
{
    public class AlertsService
    {
        private const int defaultCpuPercentageThreshold = 70;
        private const int defaultMemoryPercentageThreshold = 70;
        private const int defaultStoragePercentageThreshold = 70;
        private const double defaultIntervalTimeToAlert = 1000 * 60;

        private readonly MongoDBService _mongoDatabase;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly PrometheusClient _prometheusClient;
        private readonly ConcurrentDictionary<string, System.Timers.Timer> _timers; 
        private readonly CollectorService _collectorService;
        private System.Timers.Timer _timer;
        private readonly Dictionary<eAlertType, int> _alertTypesConverter;
        private readonly ConcurrentDictionary<string, int> _cpuPercentageThreshold;
        private readonly ConcurrentDictionary<string, int> _memoryPercentageThreshold;
        private readonly ConcurrentDictionary<string, int> _storagePercentageThreshold;
        private readonly ConcurrentDictionary<string, double> _intervalTimeToAlert;

        private const string INTERVAL_FILTER = "day";

        private readonly List<AlertData> _alerts = new List<AlertData>();

        public EmailService EmailService { get; internal set; }

        public AlertsService(MongoDBService mongoDatabase, CollectorService collectorService, IHttpContextAccessor httpContextAccessor) 
        {
            _mongoDatabase = mongoDatabase;
            _httpContextAccessor = httpContextAccessor;
            //_prometheusClient = new PrometheusClient();
            _collectorService = collectorService;
            _timers = new ConcurrentDictionary<string, System.Timers.Timer>();

            _alertTypesConverter = new Dictionary<eAlertType, int>()
            {
                [eAlertType.CPU] = defaultCpuPercentageThreshold,
                [eAlertType.STORAGE] = defaultStoragePercentageThreshold,
                [eAlertType.MEMORY] = defaultMemoryPercentageThreshold,
            };

            InitTimer(defaultIntervalTimeToAlert);
        }
        private void InitTimerByUserSession(double interval)
        {
            string userSession = _httpContextAccessor.HttpContext.Request.Cookies["Stratus"];
            if (userSession != null)
            {
                _timers[userSession] = new System.Timers.Timer();
                _timers[userSession].Interval = interval; // should be confiugre by the user
                _timers[userSession].Elapsed += timer_Elapsed;
                _timers[userSession].Start();
            }
        }
        internal async Task<List<AlertData>> GetAlertsCollection()
        {
            var result = new List<AlertData>();
            //TODO get alerts by user using cookies
            var alertsData =  _mongoDatabase.GetCollectionAsList<AlertData>(eCollectionName.Alerts).Result;

            foreach (var alert in alertsData)
            {
                if (alert != null)
                {
                    result.Add(alert);
                }
            }

            return result;
        }    

        private void InitTimer(double interval)
        {
            // update table with new data and delete records that machine id that was terminated

            _timer = new System.Timers.Timer();
            _timer.Interval = interval; // should be confiugre by the user
            _timer.Elapsed += timer_Elapsed;
            //_timer.Start();
        }

        private void timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            //foreach user:
            _alerts.Clear();
            var instances = _mongoDatabase.GetCollectionAsList<AwsInstanceDetails>(eCollectionName.Instances).Result;

            foreach (var instance in instances)
            {
                double avgCpuUsageUtilization = _collectorService.GetAvgCpuUsageUtilization(instance.InstanceAddress, INTERVAL_FILTER).Result;
                double avgFreeDiskSpaceInGB = _collectorService.GetAvgFreeDiskSpaceInGB(instance.InstanceAddress, INTERVAL_FILTER).Result;
                double avgFreeMemorySizeInGB = _collectorService.GetAvgFreeMemorySizeInGB(instance.InstanceAddress, INTERVAL_FILTER).Result;

                DetectAndInsertLowUsage(instance.InstanceAddress, avgCpuUsageUtilization, eAlertType.CPU);
                DetectAndInsertLowUsage(instance.InstanceAddress, avgFreeDiskSpaceInGB, eAlertType.STORAGE);
                DetectAndInsertLowUsage(instance.InstanceAddress, avgFreeMemorySizeInGB, eAlertType.MEMORY);
            }

            //Send mail to user
            //EmailService.SendAlertsEmailAsync("omer2541996@gmail.com", _alerts);
            //EmailService.SendAlertsEmailAsync("chen10.berger@gmail.com", _alerts);
            //EmailService.SendAlertsEmailAsync("amir.anckonina@gmail.com", _alerts);
            //EmailService.SendAlertsEmailAsync("hbinsky.mta@gmail.com", _alerts);

            InsertAlertsToDB();
        }

        private async void InsertAlertsToDB()
        {
            //insert with user id
            
            await _mongoDatabase.InsertMultipleDocuments(eCollectionName.AlertConfigurations, _alerts);
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

        internal bool SetConfigurations(AlertsConfigurations alertsConfigurations)
        {
            bool result = false;

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
            string userSession = _httpContextAccessor.HttpContext.Request.Cookies["Stratus"];
            _cpuPercentageThreshold[userSession] = alertsConfigurations.CpuThreshold;
            _memoryPercentageThreshold[userSession] = alertsConfigurations.MemoryThreshold;
            _storagePercentageThreshold[userSession] = alertsConfigurations.DiskThreshold;
            _intervalTimeToAlert[userSession] = alertsConfigurations.IntervalTimeMilisec;
            _timers[userSession].Interval = defaultIntervalTimeToAlert;
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
            switch(alertsConfigurations.IntervalPeriod)
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
        }

        ~AlertsService() { _timer.Stop(); }
    }
}
