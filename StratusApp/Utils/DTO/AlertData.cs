using System;

namespace Utils.DTO
{
    public class AlertData
    {
        public string MachineId { get; set; }
        public eAlertType Type { get; set; }
        public DateTime CreationTime { get; set; }
        public double PercentageUsage { get; set; }

        public AlertData() { }
    }
}
