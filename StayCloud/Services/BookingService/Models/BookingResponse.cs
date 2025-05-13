using System.Security.Policy;
using Newtonsoft.Json;

namespace BookingService.Models
{
    public class BookingResponse
    {
        [JsonProperty("id")]// CosmosDb komt overeen met 'id' 
        public string Id { get; set; } // string aangezien guid in controller klasse een string nodig heeft

        [JsonProperty("confirmationCode")]
        public string ConfirmationCode { get; set; }
        public decimal TotalPrice { get; set; }
        public string AccommodationType { get; set; }
        public string StayType { get; set; }
        public int TotalNights { get; set; }
        public string Guestname { get; set; }
        public string Email { get; set; }
        public bool IsApproved { get; set; } = false;


        public BookingResponse(string confirmationCode, decimal totalPrice, string accommodationType, string stayType, int totalNights, string guestName, string email, bool isApproved)
        {
            Id = confirmationCode;
            ConfirmationCode = confirmationCode;
            TotalPrice = totalPrice;
            AccommodationType = accommodationType;
            StayType = stayType;
            TotalNights = totalNights;
            Guestname = guestName;
            Email = email;
            IsApproved = isApproved;
        }
    }
}