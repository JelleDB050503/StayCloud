namespace PriceService.Models
{
    public class PriceResponse
    {
        public decimal Total {get; set;}   // Totale prijs
        public decimal BasePrice {get; set;} // Basisprijs hangt af van seizoen en staytype
        public decimal NightlyCosts {get; set;}
        public decimal CleaningCost {get; set;} // 40 euro standaard
        public decimal DogFee {get; set;} // 10 euro per hond per verblijf
        public string Currency {get; set;} = "EURO";
        public int TotalNights {get; set;}
    }
    
}