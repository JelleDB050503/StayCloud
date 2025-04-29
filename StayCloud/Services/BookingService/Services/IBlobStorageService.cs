using System.Threading.Tasks;

namespace BookingService.Services
{
    public interface IBlobStorageService
    {
        Task UploadContractAsync(string fileName, string content);
    }
}