using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Lambda.Core;

namespace ec2startstopserver.Services
{
    public class Ec2Services
    {
        private static IAmazonEC2 _Client = new AmazonEC2Client();
        // Environment variables should be setup with commas. ex: i-ieirusak123,i-jdksa21e2k2lew
        private static string ec2instances = Environment.GetEnvironmentVariable("instanceid");
        private static List<string> _InstanceId; 
        public Ec2Services()
        {

        }
        public static async Task<string> ServerCallAsync()
        {
            string response = String.Empty;
            // If there is no instances id on lambda environment variables, return null.
            if (String.IsNullOrEmpty(ec2instances)) 
            {
                LambdaLogger.Log("No ec2 instances id on lambda environment variables.");
                return response;
            }
            _InstanceId = ec2instances.Trim().Split(',').ToList();
            var serverstatus = await DescribeInstanceAsync();
            foreach (var item in serverstatus.Reservations)
            {
                LambdaLogger.Log(item.Instances[0].Tags.FirstOrDefault(t => t.Key == "Name").Value);                
            }
            if (serverstatus.Reservations[0].Instances[0].State.Name.Equals("Running"))
            {
                await StopInstanceAsync();
            }
            else if (serverstatus.Reservations[0].Instances[0].State.Name.Equals("Stopped"))
            {
                response = await StartInstanceAsync();
            }

            return response;
        }
        static async Task StopInstanceAsync()
        {
            StopInstancesRequest req = new StopInstancesRequest
            {
                InstanceIds = _InstanceId
            };
            await _Client.StopInstancesAsync(req);
        }
        static async Task<string> StartInstanceAsync() {
            string response = String.Empty;
            StartInstancesRequest req = new StartInstancesRequest
            {
                InstanceIds = _InstanceId
            };
            StartInstancesResponse res = await _Client.StartInstancesAsync(req);
            // We take 5 seconds to wait for public dns be provisioned on the running servers.
            Thread.Sleep(5000);
            // Iterate through the response to obtain values that would be used to sent an email.
            if (res.StartingInstances.Count > -1)
            {
                var serverstatus = await DescribeInstanceAsync();                
                foreach (var item in serverstatus.Reservations)
                {
                    response += $"<br />Instance Name:{item.Instances[0].Tags.FirstOrDefault(t=>t.Key=="Name").Value} - Public DNS:{item.Instances[0].PublicDnsName}";
                    LambdaLogger.Log($"Instance Name:{item.Instances[0].Tags.FirstOrDefault(t => t.Key == "Name").Value} - Public DNS:{item.Instances[0].PublicDnsName}");
                }
            }
            return response;
        }
        static async Task<DescribeInstancesResponse> DescribeInstanceAsync()
        {
            DescribeInstancesRequest req = new DescribeInstancesRequest
            {
                InstanceIds = _InstanceId
            };
            return await _Client.DescribeInstancesAsync(req);
        }
    }
}
