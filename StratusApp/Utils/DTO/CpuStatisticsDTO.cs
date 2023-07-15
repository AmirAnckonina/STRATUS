using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.DTO
{
    public class CpuStatisticsDTO
    {
        public double? Average { get; set; }

        public double? Minimium { get; set; }

        public double? Maximum { get; set; }

        public double? Sum { get; set; }
    }
}
