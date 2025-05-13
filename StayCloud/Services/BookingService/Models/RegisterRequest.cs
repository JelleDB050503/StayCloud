using System.ComponentModel.DataAnnotations;

namespace BookingService.Models
{
    public class RegisterRequest
    {
        [Required]
        [MinLength(3, ErrorMessage = "Gebruikersnaam moet minstens 3 karakters lang zijn.")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Geen geldig e-mailadres.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = "Wachtwoord moet minstens 8 karakters lang zijn.")]
        public string Password { get; set; } = string.Empty;
    }
}