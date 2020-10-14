using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;

namespace ThumbNailMaker
{
    public static class RegisterUser
    {
        [FunctionName("RegisterUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [Table("tblUserProfile", Connection = "AzureWebJobsStorage")] CloudTable objUserProfileTable,
            [Queue("userprofileimagesqueue", Connection = "AzureWebJobsStorage")] IAsyncCollector<string> objUserProfileQueueItem,
            [Queue("notificationqueue", Connection = "AzureWebJobsStorage")] IAsyncCollector<string> NotificationQueueItem,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string firstName = null, lastName = null;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            firstName ??= data?.firstname;
            lastName ??= data?.lastname;

            string profilePicUrl = data.ProfilePicUrl;
            await objUserProfileQueueItem.AddAsync(profilePicUrl);

            UserProfile objUserProfile = new UserProfile(firstName, lastName);
            TableOperation objTblOperationInsert = TableOperation.Insert(objUserProfile);
            await objUserProfileTable.ExecuteAsync(objTblOperationInsert);

            await NotificationQueueItem.AddAsync("");

            return (lastName + firstName) != null
                ? (ActionResult)new OkObjectResult($"Hello, {firstName + " " + lastName}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
