using System.ComponentModel.DataAnnotations;

namespace BookingService.Models
{
    public class ContractUpdateRequest
    {
        [Required]
        public string GuestName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string AccommodationType { get; set; } = string.Empty;
        [Required]
        public string StayType { get; set; } = string.Empty;
        [Range(1, 100)]
        public int TotalNights { get; set; }
        [Range(0, double.MaxValue)]
        public decimal TotalPrice { get; set; }
    }
}