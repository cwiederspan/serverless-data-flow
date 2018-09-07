using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Azure.Documents;

namespace DataFlow.Functions {

    public static class DataProcessor3 {

        [FunctionName("ProcessDataStage3")]
        public static async Task Run(
            [CosmosDBTrigger("mydata", "items", ConnectionStringSetting = "AzureWebJobsCosmosDb", LeaseCollectionName = "Leases", CreateLeaseCollectionIfNotExists = true)] IReadOnlyList<Document> documents,
            [Table("sampledata", Connection = "AzureWebJobsStorage")] IAsyncCollector<TableEntry> table,
            ILogger log
        ) {

            var tasks = documents.Select(async doc => {
                await table.AddAsync(new TableEntry {
                    //PartitionKey = doc.Company,
                    //RowKey = doc.Id,
                    //Name = doc.Name,
                    //Company = doc.Company,
                    //Address = doc.Address,
                    //Email = doc.Email,
                    //Phone = doc.Phone
                });
            }).ToList();

            // Get a list of all of the IDs that were written to CosmosDB
            await Task.WhenAll(tasks);
        }
    }
}
