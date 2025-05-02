using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BookingService.Services
{
    //Opslaan van huurovereenkomsten in blob storage
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient _containerClient;

        //verbindig maken met correcte container
        public BlobStorageService(string connectionString, string containerName)
        {
            // connecteren via appsettings
            var blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            _containerClient.CreateIfNotExists(); // zorg dat container bestaat anders wordt deze aangemaakt
        }
        // bestanden opslaan in Blob storage

        public async Task UploadContractAsync(string fileName, string content)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content)); // tekst omzetten naar stream
            await blobClient.UploadAsync(stream, overwrite: true); //uploaden naar storage
        }

        //Bestanden downloaden uit blob 
        public async Task<Stream> DownloadContractAsync(string blobName)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(blobName);

            if (await blobClient.ExistsAsync())
            {
                var response = await blobClient.DownloadAsync();
                return response.Value.Content;
            }

            throw new RequestFailedException(404, "Blob not found");
        }

        // Huurovereenkomst verwijderen
        public async Task<bool> DeleteContractAsync(string blobName)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(blobName);

            var response = await blobClient.DeleteIfExistsAsync();
            return response.Value; // true als verwijderd, false als niet gevonden
        }
        //Lijst alle opgeslagen huurovereenkomsten
        public async Task<List<string>> ListContractsAsync()
        {
            var fileNames = new List<string>();

            await foreach (BlobItem blobItem in _containerClient.GetBlobsAsync())
            {
                fileNames.Add(blobItem.Name);
            }

            return fileNames;
        }
    }
}