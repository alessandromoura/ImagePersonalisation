using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.EventGrid.Models;

namespace AnalyzeImage
{
    public static class Functions
    {
        [FunctionName("ReceiveImage")]
        public static void ReceiveImage(
            [EventGridTrigger] EventGridEvent eventGridEvent, 
            [Blob("{data.url}", FileAccess.Read)] Stream receivedImage,
            [Blob("approved/{data.url}", FileAccess.Write)] out Stream approvedImage,
            ILogger log)
        {
            log.LogInformation(eventGridEvent.Data.ToString());

            approvedImage = receivedImage;
        }
    }
}
