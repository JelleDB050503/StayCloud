using BookingService.Models;
using BookingService.Contexts;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace BookingService.Services.Auth
{
    public class UserService : IUserService
    {
        private readonly BookingDbContext _context;

        public UserService(BookingDbContext context)
        {
            _context = context;
        }

        // Gebruiker registreren
        public async Task<User?> RegisterAsync(string username, string email, string password)
        {
            if (await UserExistsAsync(username))
            {
                return null;
            }

            //Nieuwe gebruiker aanmaken + passwoord hashen
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "user"
            };

            // Gebruiker opslaan in DB
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        //Gebruiker inloggen
        public async Task<User?> LoginAsync(string username, string password)
        {
            // gebruiker zoeken op username
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return null;
            }

            //Vergelijking ingegeven password met hashedpassword
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null;
            }

            return user;
        }


        //Check of gebruiker bestaat
        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u=>u.Username == username);
        }
    }
}