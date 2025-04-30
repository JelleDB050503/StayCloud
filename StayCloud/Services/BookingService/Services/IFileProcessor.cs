using BookingService.Models;
using System.Threading.Tasks;

namespace BookingService.Services
{
    public interface IFileProcessor
    {
        Task GenerateAndUploadContractAsync(BookingResponse booking);
    }
}