using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ThumbNailMaker
{
    public static class CreateProfilePicture
    {
        [FunctionName("CreateProfilePicture")]
        public static void Run([QueueTrigger("userprofileimagesqueue", Connection = "AzureWebJobsStorage")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
