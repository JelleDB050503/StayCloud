namespace BookingService.Models
{
    public class BookingRequest
    {
        public required string AccommodationType { get; set; }
        public required string StayType { get; set; }
        public int Weeks { get; set; } = 1;
        public required string Season { get; set; }
        public int NumGuests { get; set; }
        public int NumDogs { get; set; }
        public required string GuestName { get; set; }
        public required string Email { get; set; }
    }
}