using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DataFlow.Functions {

    public static class DataProcessor2 {

        [FunctionName("ProcessDataStage2")]
        public static async Task Run(
            [BlobTrigger("unzipped-data/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob,
            [Blob("unzipped-data", Connection = "AzureWebJobsStorage")] CloudBlobContainer container,
            [Table("sampledata", Connection = "AzureWebJobsStorage")] IAsyncCollector<TableEntry> table,
            [Queue("entries", Connection = "AzureWebJobsStorage")] IAsyncCollector<string> queue,
            string name,
            ILogger log
        ) {

            // Setup the Serializer settings to convert the property names from lowercase (in the json file)
            // to the Model's naming convention (upper-case)
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            List<DataEntry> entries;

            // Read the file JSON and deserialize into an object
            using (var reader = new StreamReader(myBlob)) {

                // Read the data
                var json = await reader.ReadToEndAsync();
                entries = JsonConvert.DeserializeObject<List<DataEntry>>(json, serializerSettings);
            }

            // Add each item to the collection
            var docTasks = entries.Select(async doc => {

                // Save the the table storage
                await table.AddAsync(new TableEntry {
                    PartitionKey = doc.Company,
                    RowKey = doc.Id,
                    Name = $"{doc.Name.First} {doc.Name.Last}",
                    Company = doc.Company,
                    Address = doc.Address,
                    Email = doc.Email,
                    Phone = doc.Phone
                });
                
                // Save to the queue
                await queue.AddAsync(doc.Id);

                // Return the document ID
                return doc.Id;

            }).ToList();

            // Get a list of all of the IDs that were written to CosmosDB
            var ids = await Task.WhenAll(docTasks);

            // Delete the file we just processed
            var existingBlob = container.GetBlockBlobReference(name);
            var wasDeleted = await existingBlob.DeleteIfExistsAsync();

            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}
