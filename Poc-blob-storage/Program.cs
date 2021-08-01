using Azure;
using Azure.Storage.Blobs;//
using Azure.Storage.Blobs.Models;
using Microsoft.Hadoop.Avro;
using Microsoft.Hadoop.Avro.Container;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc_blob_storage
{
    class Program
    {
        // TODO: Enter the connection string of your storage account here  
        const string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=storagebloblake;AccountKey=aHU9TnKZRszbhczuyMbsAlo46M5oztMwWLavH6ravhCY7EL54Yo+Tnup80YAeEO9g/j5jKtvDzssPbu3CcdIwQ==;EndpointSuffix=core.windows.net";

        // TODO: Enter the blob container name here  
        const string containerName = "containebloblake";
        static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        private static async Task MainAsync()
        {
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(containerName);

            var resultSegment =
               await blobContainer.ListBlobsSegmentedAsync(null, true, BlobListingDetails.All,
               null, null, null, null);

            //foreach (var cloudBlockBlob in resultSegment.Results.OfType<CloudBlockBlob>())
            //{
            //    await ProcessCloudBlockBlobAsync(cloudBlockBlob);
            //}


            //foreach (var cloudBlockBlob in resultSegment.Results.OfType<CloudBlockBlob>())
            //{
                await ProcessCloudBlockBlobAsync(resultSegment.Results.OfType<CloudBlockBlob>().LastOrDefault());
            //}
            Console.ReadLine();
        }

        private static async Task ProcessCloudBlockBlobAsync(CloudBlockBlob cloudBlockBlob)
        {
            var avroRecords = await DownloadAvroRecordsAsync(cloudBlockBlob);

            PrintSensorDatas(avroRecords);
        }

        private static void PrintSensorDatas(List<AvroRecord> avroRecords)
        {
            var SensorDatas = avroRecords.Select(avroRecord =>
            CreateSensorData(avroRecord));


     

            foreach (var SensorData in SensorDatas)
            {
                Console.WriteLine(SensorData);
            }
        }

        private static async Task<List<AvroRecord>> DownloadAvroRecordsAsync(CloudBlockBlob cloudBlockBlob)
        {
            var memoryStream = new MemoryStream();
            await cloudBlockBlob.DownloadToStreamAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            List<AvroRecord> avroRecords;
            using (var reader = AvroContainer.CreateGenericReader(memoryStream))
            {

                while (reader.MoveNext())
                {
                    foreach (dynamic record in reader.Current.Objects)
                    {                        var sequenceNumber = record.SequenceNumber;
                        var bodyText = Encoding.UTF8.GetString(record.Body);
                        Console.WriteLine($"{sequenceNumber}: {bodyText}");
                    }
                }

                using (var sequentialReader = new SequentialReader<dynamic>(reader))
                {
                    avroRecords = sequentialReader.Objects.OfType<AvroRecord>().ToList();
                }
            }

            return avroRecords;
        }

        private static SensorModel CreateSensorData(AvroRecord avroRecord)
        {
            var model = new SensorModel
            {
                ID  = avroRecord.GetField<Guid>("ID"),
                Name = avroRecord.GetField<string>("Name"),
                Type = avroRecord.GetField<string>("Type"),
                Content = avroRecord.GetField<object>("Content")              
            };
            return model;
        }
    }
}
