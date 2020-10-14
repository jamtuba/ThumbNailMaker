using System.Net.Http;
using System.Net.Mail;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;


namespace ThumbNailMaker
{
    public static class SendNotifications
    {
        [FunctionName("SendNotifications")]
        public static void Run(
            [QueueTrigger("notificationqueue", Connection = "AzureWebJobsStorage")]string myQueueItem,
            //[SendGrid(ApiKey = "SendgridAPIKey")] out SendGridMessage sendGridMessage, 
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
