using BookingService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingService.Services
{
    public interface ICosmosDbService
    {
        Task AddBookingAsync(BookingResponse booking);
        Task<IEnumerable<BookingResponse>> GetBookingsAsync();
        Task<IEnumerable<BookingResponse>> GetBookingsByNameAsync(string name);
        Task DeleteBookingAsync(string confirmationCode);
        Task UpdateBookingAsync(BookingResponse booking);
    }
}