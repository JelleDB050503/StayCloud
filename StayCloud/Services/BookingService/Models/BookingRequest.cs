using System.ComponentModel.DataAnnotations;

namespace BookingService.Models
{
    public class BookingRequest
    {
        [Required]
        public required string AccommodationType { get; set; }
        [Required]
        public required string StayType { get; set; }
        [Range(1,52, ErrorMessage ="Aantal weken moet tussen de 1 en 52 liggen.")]
        public int Weeks { get; set; } = 1;
        [Required]
        public required string Season { get; set; }
        [Range(0, 4, ErrorMessage = "Aantal gasten mag maximaal 4 zijn.")]
        public int NumGuests { get; set; }
        [Range(0, 2, ErrorMessage = "Aantal honden mag maximaal 2 zijn.")]
        public int NumDogs { get; set; }
        [Required]
        [MinLength(3)]
        public required string GuestName { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }
}