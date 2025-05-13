using BookingService.Models;

namespace BookingService.Services.Auth
{
    public interface IUserService
    {
        Task<User?> RegisterAsync(string username, string email, string password);
        Task<User?> LoginAsync(string username, string password);
        Task<bool> UserExistsAsync(string username);
    }
}