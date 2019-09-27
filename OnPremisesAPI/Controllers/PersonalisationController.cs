using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using OnPremisesAPI.Models;

namespace OnPremisesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonalisationController : ControllerBase
    {
        [HttpPost]
        [Route("customers/{customerID}/account/{accountNumber}")]
        public async Task<IActionResult> Post(string customerId, string accountNumber, [FromBody]PersonalisedImage image)
        {
            try
            {
                //string connectionString = Environment.GetEnvironmentVariable("SB_CONNECTION_STRING_IMAGE");
                string connectionString = "DefaultEndpointsProtocol=https;AccountName=imagepersonalisation;AccountKey=GObakQP0oEeBCcKy8JD4Wm6yDs+oZzsdoNvwl/Qd3E5hhrMHY1mJ+M4lzqidB09lBS0OlSaFDfZPrekz5q+Qtg==;EndpointSuffix=core.windows.net";

                // Create connection to the storage account
                CloudStorageAccount storageAccount;
                if (CloudStorageAccount.TryParse(connectionString, out storageAccount))
                {
                    // Create a container for the customer
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                    CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference($"cif{customerId}");
                    

                    var containerCreated = await cloudBlobContainer.CreateIfNotExistsAsync();

                    if (containerCreated)
                    {
                        // Set permissions on the container
                        BlobContainerPermissions permissions = new BlobContainerPermissions
                        {
                            PublicAccess = BlobContainerPublicAccessType.Blob
                        };
                        await cloudBlobContainer.SetPermissionsAsync(permissions);

                    }

                    // Upload image
                    // Transform the Base64 string to binary
                    byte[] data = System.Convert.FromBase64String(image.ImageBase64);
                    MemoryStream ms = new MemoryStream(data);

                    // Upload the file
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference($"received/image_{customerId}_{accountNumber}_pending.jpg");
                    await cloudBlockBlob.UploadFromStreamAsync(ms);

                }

                return Ok($"{customerId}/{accountNumber}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
