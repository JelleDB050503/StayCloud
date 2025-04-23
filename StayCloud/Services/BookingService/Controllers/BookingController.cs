using Microsoft.AspNetCore.Mvc;
using BookingService.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BookingService.Helpers;


namespace BookingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {

        private readonly HttpClient _httpClient;

        //Tijdelijke opslag van boekingen
        private static List<BookingResponse> _bookings = new();

        //Constructor HttpClient dependency injection
        public BookingController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // POST: boeking creeren
        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
        {
            //  Validatie van input
            if (!BookingValidator.IsValidAccommodation(request.AccommodationType))
                return BadRequest("Ongeldig accommodatietype.");

            if (!BookingValidator.IsValidStayType(request.StayType))
                return BadRequest("Ongeldig verblijfstype.");

            if (!BookingValidator.IsValidSeason(request.Season))
                return BadRequest("Ongeldig seizoen.");

            // Request opbouwen voor externe PriceService
            var priceRequest = new
            {
                accommodationType = request.AccommodationType,
                stayType = request.StayType,
                weeks = request.Weeks,
                season = request.Season,
                numGuests = request.NumGuests,
                numDogs = request.NumDogs
            };

            // Stuur request naar live PriceService (via link op Azure)
            var response = await _httpClient.PostAsJsonAsync("https://staycloud-jdb.azurewebsites.net/api/price/calculate", priceRequest);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Prijsberekening mislukt via de PriceService. BookingController");
            }

            //JSON response lezen van PriceRequest
            var priceData = await response.Content.ReadFromJsonAsync<PriceResponse>();

            // BookingsResponse opstellen
            var bookingsResponse = new BookingResponse(
                confirmationCode: Guid.NewGuid().ToString(), // Guid genereert unieke code
                totalPrice: priceData?.Total ?? 0,            // totaalbedrap ophalen uit PriceResponse
                accommodationType: request.AccommodationType,
                stayType: request.StayType,
                totalNights: priceData?.TotalNights ?? 0 // aantal nachten uit PriceService
            );

            _bookings.Add(bookingsResponse); // Boekingen toevoegen aan de lijst

            return Ok(bookingsResponse);
        }

        // Alle boekingen ophalen via GET
        [HttpGet("all")]
        public IActionResult GetAllBookings()
        {
            return Ok(_bookings);
        }


        // Boekingen verwijderen obv confirmatie code via DELETE
        [HttpDelete("{confirmationCode}")]
        public IActionResult DeleteBooking(string confirmationCode)
        {
            var booking = _bookings.FirstOrDefault(b => b.ConfirmationCode == confirmationCode);
            if (booking == null)
            {
                return NotFound("Boeking is niet gevonden");
            }

            _bookings.Remove(booking);
            return Ok($"Boeking {confirmationCode} is verwijderd");
        }
    }

}