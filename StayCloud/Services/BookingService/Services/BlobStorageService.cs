using Azure.Storage.Blobs;
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
    }
}