using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net;

namespace ThumbNailMaker
{
    public static class CreateProfilePicture
    {
        [FunctionName("CreateProfilePicture")]
        public static void Run([Blob("userprofileimagecontainer/{rand-guid}", FileAccess.Write, Connection = "AzureWebJobsStorage")] Stream outputBlob,
            [QueueTrigger("userprofileimagesqueue", Connection = "AzureWebJobsStorage")]string myQueueItem, 
            ILogger log)
        {
            byte[] imageData = null;
            using(var wc = new WebClient())
            {
                imageData = wc.DownloadData(myQueueItem);
            }
            outputBlob.WriteAsync(imageData, 0, imageData.Length);
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
