using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ThumbNailMaker
{
    public static class ResizeProfilePictures
    {
        [FunctionName("ResizeProfilePictures")]
        public static void Run([BlobTrigger("userprofileimagecontainer/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, 
            string name,
            [Blob("userprofilesmallimagecontainer/{name}", FileAccess.Write, Connection = "AzureWebJobsStorage")] Stream imageSmall,
            [Blob("userprofilemediumimagecontainer/{name}", FileAccess.Write, Connection = "AzureWebJobsStorage")] Stream imageMedium,
            ILogger log)
        {
            try
            {
                IImageFormat format;

                using(Image<Rgba32> InputFormatter = Image.Load<Rgba32>(myBlob, out format))
                {
                    ResizeImageAndSave(InputFormatter, imageSmall, ImageSize.Small, format);
                }

                myBlob.Position = 0;

                using (Image<Rgba32> InputFormatter = Image.Load<Rgba32>(myBlob, out format))
                {
                    ResizeImageAndSave(InputFormatter, imageMedium, ImageSize.Medium, format);
                }
            }
            catch (Exception e)
            {
                log.LogError(e, $"unable to process the blob");
            }
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }

        private static void ResizeImageAndSave(Image<Rgba32> input, Stream output, ImageSize size, IImageFormat format)
        {
            var dimensions = imageDimensionsTable[size];

            input.Mutate(x => x.Resize(width: dimensions.Item1, height: dimensions.Item2));
            input.Save(output, format);
        }

        public enum ImageSize { ExtraSmall, Small, Medium };

        private static Dictionary<ImageSize, (int, int)> imageDimensionsTable = new Dictionary<ImageSize, (int, int)>()         {
            { ImageSize.Small, (100, 100) },
            { ImageSize.Medium, (200, 200) }
        };
    }
}
