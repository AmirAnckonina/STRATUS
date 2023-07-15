using Amazon.CloudWatch.Model;

namespace Utils.DTO
{
    public class InstanceDetails
    {
        public string Id { get; set; }
        public string Type { get; set; }

        public string OperatingSystem { get; set; }

        public decimal? Price { get; set; }

        public string CpuSpecifications { get; set; }

        public List<Datapoint> CpuStatistics { get; set; }

        public string Storage { get; set; }

        public int TotalStorageSize { get; set; }

        public int TotalVolumesSize;

        public string? Unit { get; set; }

        public string? PriceDescription {get;set;}
    }
}
