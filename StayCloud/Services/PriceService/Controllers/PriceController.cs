using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using PriceService.Models;

namespace PriceService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PriceController : ControllerBase
    {
        // Bereken de totale prijs obv request input
        [HttpPost("calculate")]
        public IActionResult Calculate([FromBody] PriceRequest request)
        {
            // nachten per verblijfstype
            int nightsPerStay = request.StayType.ToLower() switch
            {
                "weekend" => 2,
                "midweek" => 4,
                "week" => 7,
                _ => 0
            };

            if (nightsPerStay == 0)
                return BadRequest("Ongeldig verblijfstype. Kies uit 'weekend', 'midweek' of 'week'");
            // totale verblijfsduur in nachten
            int totalNights = nightsPerStay * request.Weeks;

            //basisprijs ophalen per verblijf, seizoen en type
            decimal basePricePerStay = GetBasePrice(request.Season.ToLower(), request.StayType.ToLower());

            if (basePricePerStay == 0)
                return BadRequest("Ongeldig seizoen ingevoerd.");
            decimal basePrice = basePricePerStay * request.Weeks;

            // Extra kosten per nacht (elec + touristtaks)
            decimal nightlyCost = totalNights * 12m; // m om duidelijk te maken dat het om een valuta (decimal) gaat

            //Verplichte eindschoonmaak
            decimal cleaning = 40m;

            // 10 euro per hond per verblijf
            decimal dogs = request.NumDogs * 10m;

            //Totale prijs
            decimal total = basePrice + nightlyCost + cleaning + dogs;

            // retourneren van info
            return Ok(new PriceResponse
            {
                Total = total,
                BasePrice = basePrice,
                NightlyCosts = nightlyCost,
                CleaningCost = cleaning,
                DogFee = dogs,
                TotalNights = totalNights
            });
        }

        //functie die juiste basisprijs ophaalt obv seizoen en verblijfstype
        private decimal GetBasePrice(string season, string stayType)
        {
            var prices = new Dictionary<string, (decimal weekend, decimal midweek, decimal week)>
            {
                ["laagseizoen"] = (240, 320, 540),
                ["pasen"] = (320, 400, 660),
                ["tussenseizoen"] = (270, 360, 630),
                ["hoogseizoen"] = (330, 440, 710),
                ["herfst"] = (280, 370, 650),
                ["kerst"] = (330, 440, 710),
                ["nieuwjaar"] = (330, 440, 710),
            };

            if (!prices.ContainsKey(season))
                return 0;

            return stayType switch
            {
                "weekend" => prices[season].weekend,
                "midweek" => prices[season].midweek,
                "week" => prices[season].week,
                _ => 0
            };
        }
    }

}