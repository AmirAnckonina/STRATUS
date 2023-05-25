using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudApiClient
{
    public class VirtualMachineBasicData
    {
        public string Id { get; set; }
        public string OperatingSystem { get; set; }
        public decimal Price { get; set; }
        public string CpuSpecifications { get; set; }
        public List<string> Storage { get; set; }
        public string Unit { get; set; }
    }
}
