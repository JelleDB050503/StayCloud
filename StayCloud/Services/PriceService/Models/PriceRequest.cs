namespace PriceService.Models
{
    public class PriceRequest
    {
        public int Nights { get; set; }
        public int Guests { get; set; }
        public string Seasons { get; set; }
    }
}