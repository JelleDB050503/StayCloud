namespace BookingService.Models
{
    public class BookingResponse
    {
        public string ConfirmationCode { get; set; }
        public decimal TotalPrice { get; set; }
        public string AccommodationType { get; set; }
        public string StayType { get; set; }
        public int TotalNights { get; set; }
        public string Guestname { get; set; }
        public string Email { get; set; }

        public BookingResponse(string confirmationCode, decimal totalPrice, string accommodationType, string stayType, int totalNights, string guestName, string email)
        {
            ConfirmationCode = confirmationCode;
            TotalPrice = totalPrice;
            AccommodationType = accommodationType;
            StayType = stayType;
            TotalNights = totalNights;
            Guestname = guestName;
            Email = email;
        }
    }
}