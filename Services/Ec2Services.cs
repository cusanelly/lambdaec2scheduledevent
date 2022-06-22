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
            //foreach (var item in serverstatus.Reservations)
            //{
            //    LambdaLogger.Log(item.Instances[0].InstanceId);
            //}
            if (serverstatus.Reservations[0].Instances[0].State.Name.Equals("Running"))
            {
                await StopInstanceAsync();
            }
            else if (serverstatus.Reservations[0].Instances[0].State.Name.Equals("Stopped")) {
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
            Thread.Sleep(5000);
            if (res.StartingInstances.Count > 0)
            {
                var serverstatus = await DescribeInstanceAsync();
                foreach (var item in serverstatus.Reservations)
                {
                    response += $"InstanceId:{item.Instances[0].InstanceId} - IPAddress:{item.Instances[0].PublicDnsName} ";
                    LambdaLogger.Log($"InstanceId:{item.Instances[0].InstanceId} - IPAddress:{item.Instances[0].PublicDnsName}");
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
