using Azure;
using Azure.Storage.Blobs;//
using Azure.Storage.Blobs.Models;
using System;
using System.Threading.Tasks;

namespace Poc_blob_storage
{
    class ProgramCopy
    {
     
        static async Task Main(string[] args)
        {
            string connectionString = "";
            string containerName = "";
            string blobName = "";
            string filePath = "";

            // ""
            Console.WriteLine("Hello World!");


            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            // Get the container client object
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("containebloblake");

            // List all blobs in the container
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                Console.WriteLine("\t" + blobItem.Name);
            }

            // Get a reference to a container named "sample-container" and then create it
            BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
            container.Create();

            // Get a reference to a blob named "sample-file" in a container named "sample-container"
            var blobs = container.GetBlobs();

            // Upload local file
            //blob.Upload(filePath);
        }

        async static Task ListContainers(BlobServiceClient blobServiceClient,
                                string prefix,
                                int? segmentSize)
        {
            try
            {
                // Call the listing operation and enumerate the result segment.
                var resultSegment =
                    blobServiceClient.GetBlobContainersAsync(BlobContainerTraits.Metadata, prefix, default)
                    .AsPages(default, segmentSize);

                await foreach (Azure.Page<BlobContainerItem> containerPage in resultSegment)
                {
                    foreach (BlobContainerItem containerItem in containerPage.Values)
                    {
                        Console.WriteLine("Container name: {0}", containerItem.Name);
                    }

                    Console.WriteLine();
                }
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }
    }
}
