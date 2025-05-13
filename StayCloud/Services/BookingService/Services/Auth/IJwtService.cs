using BookingService.Models;

namespace BookingService.Services.Auth
{
    public interface IJwtService
    {
         string GenerateToken(User user);
    }
}