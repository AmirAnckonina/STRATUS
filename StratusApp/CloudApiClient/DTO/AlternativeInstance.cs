using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudApiClient.DTO
{
    public class AlternativeInstance
    {
        public string InstanceName { get; set; }
        public string HourlyRate { get; set; }
        public string vCPU { get; set; }
        public string Memory { get; set; }
        public string Storage { get; set; }
        public string NetworkPerformance { get; set; }

        public AlternativeInstance(string instanceName, string hourlyRate, string vCPU, string memory, string storage, string networkPerformance)
        {
            InstanceName = instanceName;
            HourlyRate = hourlyRate;
            this.vCPU = vCPU;
            Memory = memory;
            Storage = storage;
            NetworkPerformance = networkPerformance;
        }
    }
}
