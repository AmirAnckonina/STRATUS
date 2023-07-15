using System;

namespace Utils.DTO
{
    public class AlertData
    {
        public string MachineId { get; set; }
        public eAlertType Type { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime UnderUsageDetectedTime { get; set; }
        public float PercentageUsage { get; set; }

        public AlertData() { }
    }
}
