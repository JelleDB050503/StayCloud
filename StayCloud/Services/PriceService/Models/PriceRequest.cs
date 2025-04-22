namespace PriceService.Models
{
    public class PriceRequest
    {
        public string AccomodationType { get; set; } = ""; // Keuze uit "caravan-1", "caravan-2", "chalet"
        public string StayType { get; set; } = ""; // "weekend", "midweek", "week"
        public int Weeks { get; set; } = 1; //default waarde
        public string Season { get; set; } = ""; // "laagseizoen" , "pasen", " tussenseizoen", "hoogseizoen", "herfst", "kerst", "nieuwjaar"
        public int NumGuests { get; set; }
        public int NumDogs { get; set; }
    }
}