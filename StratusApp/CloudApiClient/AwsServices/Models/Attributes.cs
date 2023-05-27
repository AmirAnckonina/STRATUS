using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudApiClient.AwsServices.Models
{
    [Serializable]
    public class Attributes
    {
        public string enhancedNetworkingSupported { get; set; }
        public string intelTurboAvailable { get; set; }
        public string memory { get; set; }
        public string dedicatedEbsThroughput { get; set; }
        public string vcpu { get; set; }
        public string classicnetworkingsupport { get; set; }
        public string capacitystatus { get; set; }
        public string locationType { get; set; }
        public string storage { get; set; }
        public string instanceFamily { get; set; }
        public string operatingSystem { get; set; }
        public string intelAvx2Available { get; set; }
        public string regionCode { get; set; }
        public string physicalProcessor { get; set; }
        public string clockSpeed { get; set; }
        public string ecu { get; set; }
        public string networkPerformance { get; set; }
        public string servicename { get; set; }
        public string gpuMemory { get; set; }
        public string vpcnetworkingsupport { get; set; }
        public string instanceType { get; set; }
        public string tenancy { get; set; }
        public string usagetype { get; set; }
        public string normalizationSizeFactor { get; set; }
        public string intelAvxAvailable { get; set; }
        public string processorFeatures { get; set; }
        public string servicecode { get; set; }
        public string licenseModel { get; set; }
        public string currentGeneration { get; set; }
        public string preInstalledSw { get; set; }
        public string location { get; set; }
        public string processorArchitecture { get; set; }
        public string marketoption { get; set; }
        public string operation { get; set; }
        public string availabilityzone { get; set; }
    }
}
