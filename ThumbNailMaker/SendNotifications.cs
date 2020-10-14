using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using System;
using System.IO;
using System.Text;

namespace ThumbNailMaker
{
    public static class SendNotifications
    {
        [FunctionName("SendNotifications")]
        public static void Run(
            [QueueTrigger("notificationqueue", Connection = "AzureWebJobsStorage")]string myQueueItem,
            [SendGrid(ApiKey = "SendgridAPIKey")] out SendGridMessage message,
            IBinder binder,
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

            message.AddAttachment(firstName + "_" + lastName + ".log", Convert.ToBase64String(Encoding.UTF8.GetBytes(emailContent)),
                "text/plain",
                "attachment",
                "Logs"
                );

            using (var emailLogBloboutput = binder.Bind<TextWriter>(new BlobAttribute($"userregistrationmaillogs/{ inputJson.RowKey }.log")))
            {
                emailLogBloboutput.WriteLine(emailContent);
            }
        }
    }
}
