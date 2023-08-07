using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using EC2Model = Amazon.EC2.Model;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Sockets;
using CloudApiClient.AwsServices.AwsUtils;
using Utils.Enums;

namespace CloudApiClient.AwsServices
{
    public class EC2Service
    {
        private AmazonEC2Client _ec2Client;
        private readonly EC2ClientFactory _ec2ClientFactory;

        public EC2Service(AWSCredentials credentials, RegionEndpoint region, EC2ClientFactory eC2ClientFactory)
        {
            _ec2Client = new AmazonEC2Client(credentials);
            _ec2ClientFactory = eC2ClientFactory;
        }
        public bool StoreAWSCredentialsInSession(string accessKey, string secretKey)
        {
            return _ec2ClientFactory.StoreAWSCredentialsInSession(accessKey, secretKey);
        }
        public Dictionary<eAWSCredentials, string> GetAWSCredentialsFromSession()
        {
            return _ec2ClientFactory.GetAWSCredentialsFromSession();
        }
        public async Task<string> GetInstanceOperatingSystem(string instanceId)
        {
            var request = new DescribeInstancesRequest
            {
                InstanceIds = new List<string> { instanceId }
            };

            var response = await _ec2Client.DescribeInstancesAsync(request);

            // The below line should be changed to retrieve our instance platform.
            //var instance = response.Reservations.SelectMany(r => r.Instances).FirstOrDefault();
            var instance = response.Reservations[0].Instances[0];

            if (instance != null)
            {
                return instance.PlatformDetails;
            }

            return string.Empty;
        }

        public async Task<List<Volume>> GetInstanceVolumes(string instanceId)
        {
            DescribeVolumesRequest descVolumeRequest = new DescribeVolumesRequest()
            {
                Filters = { new EC2Model.Filter { Name = "attachment.instance-id", Values = { instanceId } } }
            };
            DescribeVolumesResponse descVolumeResponse = await _ec2Client.DescribeVolumesAsync(descVolumeRequest);

            return descVolumeResponse.Volumes;
        }

        public async Task<DescribeInstancesResponse> DescribeInstancesAsync()
        {
            return await _ec2Client.DescribeInstancesAsync(new DescribeInstancesRequest());
        }

        public async Task<List<Instance>> GetInstances()
        {
            var instances = new List<Instance>();

            var client = new AmazonEC2Client(RegionEndpoint.USWest2); // Replace with your desired regionEndPoint

            var request = new DescribeInstancesRequest
            {
                Filters = new List<Amazon.EC2.Model.Filter>
                {
                    new Amazon.EC2.Model.Filter
                    {
                        Name = "instance-state-name",
                        Values = new List<string> { "running" }
                    }
                }
            };

            var response = client.DescribeInstancesAsync(request);

            foreach (var reservation in response.Result.Reservations)
            {
                foreach (var instance in reservation.Instances)
                {
                    instances.Add(instance);
                }
            }

            return instances;
        }
    }
}
