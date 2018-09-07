using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DataFlow.Functions {

    public static class DataProcessor1 {

        [FunctionName("ProcessDataStage1")]
        public static async Task Run(
            [BlobTrigger("zipped-data/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, 
            [Blob("zipped-data", Connection = "AzureWebJobsStorage")] CloudBlobContainer incomingContainer,
            [Blob("unzipped-data", Connection = "AzureWebJobsStorage")] CloudBlobContainer outputContainer,
            string name,
            ILogger log
        ) {

            using (var zip = new ZipArchive(myBlob)) {

                //Each entry here represents an individual file or a folder
                foreach (var entry in zip.Entries) {

                    // Creating an empty file pointer
                    var outputBlob = outputContainer.GetBlockBlobReference(entry.FullName);

                    using (var stream = entry.Open()) {

                        // Make sure the file is not empty
                        if (entry.Length > 0) {
                            await outputBlob.UploadFromStreamAsync(stream);
                        }
                    }
                }
            }

            // Delete the file we just processed
            var existingBlob = incomingContainer.GetBlockBlobReference(name);
            var wasDeleted = await existingBlob.DeleteIfExistsAsync();

            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}
