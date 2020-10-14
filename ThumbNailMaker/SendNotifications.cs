using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using System.IO;

namespace ThumbNailMaker
{
    public static class SendNotifications
    {
        [FunctionName("SendNotifications")]
        public static void Run(
            [QueueTrigger("notificationqueue", Connection = "AzureWebJobsStorage")]string myQueueItem,
            [SendGrid(ApiKey = "SendgridAPIKey")] out SendGridMessage message,
            [Blob("userregistrationmaillogs/{rand-guid}.log", Connection = "AzureWebJobsStorage")]TextWriter outputBlob,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            dynamic inputJson = JsonConvert.DeserializeObject(myQueueItem);

            string firstName = inputJson.FirstName;
            string lastName = inputJson.LastName;
            string email = inputJson.Email;
            string emailContent;

            log.LogInformation($"Email {email}, {firstName + " " + lastName}");
            
            message = new SendGridMessage();

            message.SetSubject("New User got registered succesfully.");
            message.SetFrom("jamtuba@gmail.com");
            message.AddTo(email, firstName + " " + lastName);

            emailContent = "Thank you <b>" + firstName + " " + lastName + "</b> for your registration.<br><br>" + "Below are the details that you have provided us<br><br>" +
                "<b>First name:</b> " + firstName + "<br>" +
                "<b>Last name:</b> " + lastName + "<br>" +
                "<b>Email address:</b> " + email + "<br><br><br>" +
                "Best Regards," + "<br>" + "Website Team";

            message.AddContent("text/html", emailContent);
            outputBlob.WriteLine(emailContent);
        }
    }
}
