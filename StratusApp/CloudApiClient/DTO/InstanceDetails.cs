using Amazon.CloudWatch.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudApiClient.DTO
{
    public class InstanceDetails
    {
        public string Id { get; set; }

        public string OperatingSystem { get; set; }

        public decimal? Price { get; set; }

        public string CpuSpecifications { get; set; }

        public List<Datapoint> CpuStatistics { get; set; }

        public string Storage { get; set; }

        public int TotalVolumesSize;

        public string? Unit { get; set; }

        public string? PriceDescription {get;set;}
    }
}
