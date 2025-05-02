using System.Threading.Tasks;

namespace BookingService.Services
{
    public interface IBlobStorageService
    {
        Task UploadContractAsync(string fileName, string content);
        Task<Stream> DownloadContractAsync(string blobName);
        Task<bool> DeleteContractAsync(string blobName);
        Task<List<string>> ListContractsAsync();
    }
}