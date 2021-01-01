using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace ec2startstopserver.Services
{
    public class SesServices
    {
        private static readonly string _fromemail = Environment.GetEnvironmentVariable("fromemail");
        private static readonly string _emails = Environment.GetEnvironmentVariable("emails");
        private static readonly IAmazonSimpleEmailService _Client = new AmazonSimpleEmailServiceClient();
        public SesServices()
        {

        }

        public static async Task SendNotificationAsync(string subject, string message) {
            
            List<string> toemails = new List<string>();
            string[] listemails = _emails.Contains(",") ? _emails.Split(',') : new[] { _emails };
            toemails.AddRange(listemails);
            SendEmailRequest req = new SendEmailRequest { 
                Source = _fromemail,
                Destination = new Destination { 
                    ToAddresses = toemails    
                },                
                Message = new Message { 
                  Subject = new Content(subject),
                  Body = new Body { 
                    Html = new Content(message)
                  }
                }
            };
            await _Client.SendEmailAsync(req);
        }
    }
}
