using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.DTO
{
    public class SingleCpuUtilizationDTO
    {
        public int CpuIdx { get; set; }

        public double UtilizationPercentage { get; set; }
    }
}
