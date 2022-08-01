using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using ec2startstopserver.Services;
using Amazon.Lambda.CloudWatchEvents.ScheduledEvents;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ec2startstopserver
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(ScheduledEvent input, ILambdaContext context)
        {
            LambdaLogger.Log("Cron job triggered, beginning to stop ec2 server.");
            string emailmessage = String.Empty;
            string emailsubject = String.Empty;
            LambdaLogger.Log(input.Time.Hour.ToString());
            SetEmailMessage(ref emailsubject, ref emailmessage, input.Time);
            
            string response = await Ec2Services.ServerCallAsync();
            if (response != null) {
                emailmessage += response;                
            }
            await SesServices.SendNotificationAsync(emailsubject, emailmessage);

            LambdaLogger.Log($"{emailmessage} Completed task.");
        }
        public void SetEmailMessage(ref string subject, ref string message, DateTime date) 
        {
            switch (date.Hour)
            {
                case 13: // 17:00 UTC
                    subject = "AWS - Inicio de encendido de servidores.";
                    message = "Ha comenzado el ciclo de encendido de servidores.";
                    break;                
                case 23: // 23:00 UTC
                    subject = "AWS - Inicio del segundo ciclo de cambio de servidor.";
                    break;
                default: // 02:00 UTC
                    subject = "AWS - Inicio de apagado de servidores.";
                    message = "Ha comenzado el ciclo de apagado de servidores.";
                    break;
            }
            
        }
    }
}