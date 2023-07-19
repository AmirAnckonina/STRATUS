using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MonitoringClient;
using OpenQA.Selenium;
using StratusApp.Data;
using StratusApp.Models;
using StratusApp.Models.MongoDB;
using StratusApp.Models.Responses;
using StratusApp.Services.MongoDBServices;
using System.Timers;
using Utils.DTO;

namespace StratusApp.Services
{
    public class AlertsService
    {
        private readonly MongoDBService _mongoDatabase;
        private readonly PrometheusClient _prometheusClient;
        private System.Timers.Timer _timer;
        private readonly Dictionary<eAlertType, int> _alertTypesConverter;
        private const int CPU_PERCENTAGE_THRESHOLD = 70;
        private const int MEMORY_PERCENTAGE_THRESHOLD = 70;
        private const int STORAGE_PERCENTAGE_THRESHOLD = 70;
        private const string ALERTS_TABLE = "Alerts";
        private const long INTERVAL = 1000 * 60;

        private readonly List<AlertData> _alerts = new List<AlertData>();


        public AlertsService(MongoDBService mongoDatabase) 
        {
            _mongoDatabase = mongoDatabase;
            _prometheusClient = new PrometheusClient();
            _alertTypesConverter = new Dictionary<eAlertType, int>()
            {
                [eAlertType.CPU] = CPU_PERCENTAGE_THRESHOLD,
                [eAlertType.STORAGE] = STORAGE_PERCENTAGE_THRESHOLD,
                [eAlertType.MEMORY] = MEMORY_PERCENTAGE_THRESHOLD,
            };

            InitTimer();
        }

        internal List<BsonDocument> GetAlertsTable()
        {
            return _mongoDatabase.GetCollectionAsList(ALERTS_TABLE).Result;
        }    

        private void InitTimer(double interval = INTERVAL)
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
            string machineId = "1"; 
            //foreach user:
            //foreach user instance:

            double avgCpuUsageUtilization = _prometheusClient.GetAvgCpuUsageUtilization(machineId, "1d").Result;
            double avgFreeDiskSpaceInGB = _prometheusClient.GetAvgFreeDiskSpaceInGB(machineId, "1d").Result;
            double avgFreeMemorySizeInGB = _prometheusClient.GetAvgFreeMemorySizeInGB(machineId, "1d").Result;

            _alerts.Clear();
            DetectAndInsertLowUsage(machineId, avgCpuUsageUtilization, eAlertType.CPU);
            DetectAndInsertLowUsage(machineId, avgFreeDiskSpaceInGB, eAlertType.STORAGE);
            DetectAndInsertLowUsage(machineId, avgFreeMemorySizeInGB, eAlertType.MEMORY);
            InsertAlertsToDB();
        }

        private void InsertAlertsToDB()
        {
            foreach (AlertData alertData in _alerts)
            {
                _mongoDatabase.InsertDocument(ALERTS_TABLE, new AlertDocument()
                {
                    MahcineId = alertData.MachineId,
                    Type = alertData.Type,
                    CreationTime = alertData.CreationTime,
                    PercentageUsage = alertData.PercentageUsage,
                });
            }
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

        ~AlertsService() { _timer.Stop(); }
    }
}
