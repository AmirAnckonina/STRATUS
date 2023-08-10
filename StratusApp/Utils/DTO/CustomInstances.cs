namespace Utils.DTO
{
    public class CustomInstances
    {
        public AlternativeInstance AlternativeInstance { get; set; }
        public AwsInstanceDetails InstanceDetails { get; set; }
        public double PriceDiff { get; private set; }
        public double VCpuDiff { get; private set; }
        public double MemoryDiff { get; private set; }
        public double StorageDiff { get; private set; }

        public CustomInstances(AwsInstanceDetails instance, AlternativeInstance alternative)
        {
            InstanceDetails = instance;
            AlternativeInstance = alternative;

            CalculateInstanceDiffrences();
        }
        
        public void CalculateInstanceDiffrences()
        {
            var pricePercentage = AlternativeInstance.Specifications.Price.Value / InstanceDetails.Specifications.Price.Value * 100;
            pricePercentage = pricePercentage < 100 ? 100 - pricePercentage : pricePercentage - 100;
            PriceDiff = pricePercentage < 100 ? pricePercentage * -1 : pricePercentage;

            var vCpuPercentage = AlternativeInstance.Specifications.VCPU / InstanceDetails.Specifications.VCPU * 100;
            vCpuPercentage = vCpuPercentage < 100 ? 100 - vCpuPercentage : vCpuPercentage - 100;
            VCpuDiff = vCpuPercentage < 100 ? vCpuPercentage * -1 : vCpuPercentage;

            var memortyPercentage = AlternativeInstance.Specifications.Memory.Value / InstanceDetails.Specifications.Memory.Value * 100;
            memortyPercentage = memortyPercentage < 100 ? 100 - memortyPercentage : memortyPercentage - 100;
            MemoryDiff = memortyPercentage < 100 ? memortyPercentage * -1 : memortyPercentage;

            //TODO storage diff
        }
    }
}
