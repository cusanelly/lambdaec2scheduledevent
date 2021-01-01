using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.EC2;
using Amazon.EC2.Model;

namespace ec2startstopserver.Services
{
    public class Ec2Services
    {        
        private static readonly IAmazonEC2 _Client = new AmazonEC2Client();        
        private static string _InstanceId = Environment.GetEnvironmentVariable("instanceid");
        public Ec2Services()
        {
            
        }

        public static async Task ServerCallAsync() {
            var serverstatus = await DescribeInstanceAsync();
            if (serverstatus.Reservations[0].Instances[0].State.Name.Equals("Running"))
            {
                await StopInstanceAsync();
            }            
        }        
        static async Task StopInstanceAsync()
        {
            StopInstancesRequest req = new StopInstancesRequest
            {
                InstanceIds = new List<string> { _InstanceId }                
            };
            await _Client.StopInstancesAsync(req);
        }
        static async Task<DescribeInstancesResponse> DescribeInstanceAsync()
        {
            DescribeInstancesRequest req = new DescribeInstancesRequest
            {
                InstanceIds = new List<string> { _InstanceId }
            };
            return await _Client.DescribeInstancesAsync(req);
        }
    }
}
