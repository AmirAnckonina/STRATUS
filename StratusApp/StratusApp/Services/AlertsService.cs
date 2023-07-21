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
using System.Reflection.PortableExecutable;
using System;
using System.Timers;
using Utils.DTO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;

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
        private const string INTERVAL_FILTER = "1d";
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

        internal async Task<List<AlertData>> GetAlertsCollection()
        {
            var result = new List<AlertData>();
            var alertsData =  _mongoDatabase.GetCollectionAsList(ALERTS_TABLE).Result;            

            foreach (var alert in alertsData)
            {
                var alertDataToAdd = BsonSerializer.Deserialize<AlertData>(alert);

                if (alertDataToAdd != null)
                {
                    result.Add(alertDataToAdd);
                }
            }

            return result;
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
            string machineId = "34.125.220.240";
            //foreach user:
            //foreach user instance:

            double avgCpuUsageUtilization = _prometheusClient.GetAvgCpuUsageUtilization(machineId, INTERVAL_FILTER).Result;
            double avgFreeDiskSpaceInGB = _prometheusClient.GetAvgFreeDiskSpaceInGB(machineId, INTERVAL_FILTER).Result;
            double avgFreeMemorySizeInGB = _prometheusClient.GetAvgFreeMemorySizeInGB(machineId, INTERVAL_FILTER).Result;

            _alerts.Clear();
            DetectAndInsertLowUsage(machineId, avgCpuUsageUtilization, eAlertType.CPU);
            DetectAndInsertLowUsage(machineId, avgFreeDiskSpaceInGB, eAlertType.STORAGE);
            DetectAndInsertLowUsage(machineId, avgFreeMemorySizeInGB, eAlertType.MEMORY);

            InsertAlertsToDB();
        }

        private void InsertAlertsToDB()
        {
            var alertsCollection = _mongoDatabase.GetCollection<AlertData>(ALERTS_TABLE);

            alertsCollection.InsertManyAsync(_alerts);
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
