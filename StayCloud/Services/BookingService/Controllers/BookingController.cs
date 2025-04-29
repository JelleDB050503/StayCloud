using Microsoft.AspNetCore.Mvc;
using BookingService.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BookingService.Helpers;
using BookingService.Services;


namespace BookingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {

        private readonly HttpClient _httpClient;
        private readonly ICosmosDbService _cosmosDbService;
        private readonly IBlobStorageService _blobStorageService;

        //Constructor HttpClient dependency injection
        public BookingController(HttpClient httpClient, ICosmosDbService cosmosDbService, IBlobStorageService blobStorageService)
        {
            _httpClient = httpClient;
            _cosmosDbService = cosmosDbService;
            _blobStorageService = blobStorageService;
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
            var booking = new BookingResponse(
                confirmationCode: Guid.NewGuid().ToString(), // Guid genereert unieke code
                totalPrice: priceData?.Total ?? 0,            // totaalbedrap ophalen uit PriceResponse
                accommodationType: request.AccommodationType,
                stayType: request.StayType,
                totalNights: priceData?.TotalNights ?? 0, // aantal nachten uit PriceService
                guestName: request.GuestName,
                email: request.Email
            );

            await _cosmosDbService.AddBookingAsync(booking); // Boekingen toevoegen aan de lijst

            // Tekst huurovereenkomst (blob storage)
            string agreementText = $"Bevestiging: {booking.ConfirmationCode}\n" +
                                    $"Naam: {booking.Guestname}\n" +
                                    $"Email: {booking.Email}\n" +
                                    $"Accommodatie: {booking.AccommodationType}\n" +
                                    $"Verblijfstype: {booking.StayType}\n" +
                                    $"Aantal nachten: {booking.TotalNights}\n" +
                                    $"Prijs: €{booking.TotalPrice}";

            // huurovereenkomst uploaden als .txt
            await _blobStorageService.UploadContractAsync($"{booking.ConfirmationCode}.txt", agreementText);

            return Ok(booking);
        }

        // Alle boekingen ophalen via GET
        [HttpGet("all")]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _cosmosDbService.GetBookingsAsync();
            return Ok(bookings);
        }


        // Boekingen verwijderen obv confirmatie code via DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(string id)
        {
            await _cosmosDbService.DeleteBookingAsync(id);
            return Ok($"Boeking {id} is verwijderd");
        }

        // Opzoeken van boekingen op naam en/of confirmation code
        [HttpGet("search")]
        public async Task<IActionResult> SearchBooking([FromQuery] string? name, [FromQuery] string? confirmationCode)
        {
            var allBookings = await _cosmosDbService.GetBookingsAsync();
            //Filter lijst
            var results = allBookings.Where(b =>
            // name is opgegeven --> controleer of naam overeenkomt
            (string.IsNullOrEmpty(name) || b.Guestname?.ToLower().Contains(name.ToLower()) == true) &&
            // controleer of confirmationcode exact overeenkomt indien opgegeven
            (string.IsNullOrEmpty(confirmationCode) || b.Id.ToLower() == confirmationCode.ToLower())).ToList();

            if (!results.Any())
            {
                return NotFound("Geen boeking gevonden op basis van naam en confirmatie nummer.");
            }

            return Ok(results);
        }

        // Boekingen bijwerken obv confirmationCode PUT
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateBooking(string id, [FromBody] BookingRequest updatedBooking)
        {

            var allBookings = await _cosmosDbService.GetBookingsAsync();
            // Zoek bestaande boeking
            var existingBooking = allBookings.FirstOrDefault(b => b.Id == id);
            if (existingBooking == null)
                return NotFound($"Geen boeking gevonden met boekingsnummer: {id}");

            // Valideren van de input
            if (!BookingValidator.IsValidAccommodation(updatedBooking.AccommodationType))
                return BadRequest("Ongeldig accommodatietype");

            if (!BookingValidator.IsValidSeason(updatedBooking.Season))
                return BadRequest("Ongeldig seizoen");

            if (!BookingValidator.IsValidStayType(updatedBooking.StayType))
                return BadRequest("Ongeldig verblijfstype.");

            // Prijs opnieuw berekenen
            var priceRequest = new 
            {
                accommodationType = updatedBooking.AccommodationType,
                stayType = updatedBooking.StayType,
                weeks = updatedBooking.Weeks,
                season = updatedBooking.Season,
                numGuests = updatedBooking.NumGuests,
                numDogs = updatedBooking.NumDogs
            };

            // Nieuwe prijs berekenen via priceservice
            var response = await _httpClient.PostAsJsonAsync("https://staycloud-jdb.azurewebsites.net/api/price/calculate", priceRequest);
            if (!response.IsSuccessStatusCode)
                return BadRequest("Prijsberekening mislukt via PriceService tijdens update.");

            var priceData = await response.Content.ReadFromJsonAsync<PriceResponse>();

            // Velden updaten van boeking
            existingBooking.AccommodationType = updatedBooking.AccommodationType;
            existingBooking.StayType = updatedBooking.StayType;
            existingBooking.Guestname = updatedBooking.GuestName;
            existingBooking.Email = updatedBooking.Email;
            existingBooking.TotalNights = priceData?.TotalNights ?? 0;
            existingBooking.TotalPrice = priceData?.Total?? 0;

            await _cosmosDbService.UpdateBookingAsync(existingBooking);

            return Ok(existingBooking);
        }
    }

}