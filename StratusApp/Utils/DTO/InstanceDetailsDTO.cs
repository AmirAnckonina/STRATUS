using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.DTO
{
    public class InstanceDetailsDTO
    {
        //Replacing Instance-id of aws, this is an IP addr
        public string InstanceAddr { get; set; }

        public string OperatingSystem { get; set; } = "Linux";

        public string VirtualCpus { get; set; }

        public string MemorySize { get; set; }

        public string DiskSize { get; set; }

        public string Price { get; set; } = "0.00$ Per hour";
    }
}
