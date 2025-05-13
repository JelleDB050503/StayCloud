using Microsoft.AspNetCore.Mvc;
using BookingService.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BookingService.Helpers;
using BookingService.Services;
using Azure;
using Microsoft.AspNetCore.Authorization;


namespace BookingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {

        private readonly HttpClient _httpClient;
        private readonly ICosmosDbService _cosmosDbService;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IFileProcessor _fileProcessor;
        private readonly IEmailService _emailService;

        //Constructor HttpClient dependency injection
        public BookingController(HttpClient httpClient, ICosmosDbService cosmosDbService, IBlobStorageService blobStorageService, IFileProcessor fileProcessor, IEmailService emailService)
        {
            _httpClient = httpClient;
            _cosmosDbService = cosmosDbService;
            _blobStorageService = blobStorageService;
            _fileProcessor = fileProcessor;
            _emailService = emailService;
        }

        // POST: boeking creeren
        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
                email: request.Email,
                isApproved: false
            );

            await _cosmosDbService.AddBookingAsync(booking); // Boekingen toevoegen aan de lijst

            // huurovereenkomst uploaden als .txt
            await _fileProcessor.GenerateAndUploadContractAsync(booking);
            // Laad contractbestand uit blob storage
            var fileName = $"{booking.Id}.txt";
            var contractStream = await _blobStorageService.DownloadContractAsync(fileName);

            // Lees de stream als byte-array
            using var memoryStream = new MemoryStream();
            await contractStream.CopyToAsync(memoryStream);
            var attachmentBytes = memoryStream.ToArray();

            return Ok(booking);
        }

        // Alle boekingen ophalen via GET
        [Authorize(Roles = "admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _cosmosDbService.GetBookingsAsync();
            return Ok(bookings);
        }


        // Boekingen verwijderen obv confirmatie code via DELETE
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(string id)
        {
            var bookings = await _cosmosDbService.GetBookingsAsync();
            var exists = bookings.Any(b => b.Id == id);

            if (!exists)
                return NotFound($"Boeking met ID '{id}' bestaat niet.");

            await _cosmosDbService.DeleteBookingAsync(id);
            return Ok($"Boeking {id} is verwijderd");
        }

        // Opzoeken van boekingen op naam en/of confirmation code
        [Authorize(Roles = "admin")]
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
        [Authorize(Roles = "admin")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateBooking(string id, [FromBody] BookingRequest updatedBooking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


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
            existingBooking.TotalPrice = priceData?.Total ?? 0;

            await _cosmosDbService.UpdateBookingAsync(existingBooking);

            return Ok(existingBooking);
        }


        // Downloaden huurovereekomst
        [Authorize]
        [HttpGet("contract/{confirmationCode}")]
        public async Task<IActionResult> DownloadContract(string confirmationCode)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var fileName = $"{confirmationCode}.txt";
                var stream = await _blobStorageService.DownloadContractAsync(fileName);

                // Retourneer de inhoud van het bestand als een downloadbare file
                return File(stream, "text/plain", fileName);
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return NotFound("Contractbestand niet gevonden.");
            }
        }

        // opgeslagen huurovereenkomsten ophalen
        [Authorize(Roles = "admin")]
        [HttpGet("contracts")]
        public async Task<IActionResult> GetAllContracts()
        {
            var contracts = await _blobStorageService.ListContractsAsync();
            return Ok(contracts);
        }

        // Verwijderen huurovereekomst
        [Authorize(Roles = "admin")]
        [HttpDelete("contracts/{confirmationCode}")]
        public async Task<IActionResult> DeleteContract(string confirmationCode)
        {
            var fileName = $"{confirmationCode}.txt";
            var deleted = await _blobStorageService.DeleteContractAsync(fileName);

            if (!deleted)
                return NotFound("Huurovereenkomst niet gevonden in Blob Storage.");

            return Ok($"Contractbestand {fileName} is verwijderd.");
        }

        //PUT: huurovereenkomst aanpassen
        [Authorize(Roles = "admin")]
        [HttpPut("contract/{confirmationCode}")]
        public async Task<IActionResult> UpdateContract(string confirmationCode, [FromBody] ContractUpdateRequest update)
        {
            try
            {
                string fileName = $"{confirmationCode}.txt";

                // Bouw de aangepaste tekst voor het contract
                string updatedContent = $"""
            Bevestiging: {confirmationCode}
            Naam: {update.GuestName}
            Email: {update.Email}
            Accommodatie: {update.AccommodationType}
            Verblijfstype: {update.StayType}
            Aantal nachten: {update.TotalNights}
            Prijs: €{update.TotalPrice}
            """;

                await _blobStorageService.UpdateContractAsyc(fileName, updatedContent);
                await _emailService.SendEmailAsync(update.Email, "Bevestiging huurcontract aangepast", $"Beste {update.GuestName},\n\nUw huurcontract is aangepast.\n\nBevestiging: {confirmationCode}");

                return Ok($"Contract '{fileName}' werd succesvol bijgewerkt.");
                
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            

        }

        [Authorize(Roles = "admin")]
        [HttpPost("approve/{id}")]
        public async Task<IActionResult> ApproveBooking(string id)
        {
            var bookings = await _cosmosDbService.GetBookingsAsync();
            var booking = bookings.FirstOrDefault(b => b.Id == id);

            if (booking == null)
                return NotFound("Boeking niet gevonden.");

            if (booking.IsApproved)
                return BadRequest("Boeking is al goedgekeurd.");

            booking.IsApproved = true;
            await _cosmosDbService.UpdateBookingAsync(booking);

            // Contract ophalen
            var fileName = $"{booking.Id}.txt";
            var stream = await _blobStorageService.DownloadContractAsync(fileName);
            using var mem = new MemoryStream();
            await stream.CopyToAsync(mem);
            var data = mem.ToArray();

            // Mail sturen
            await _emailService.SendEmailWithAttachmentAsync(
                booking.Email,
                "Je boeking is goedgekeurd",
                $"Beste {booking.Guestname},\n\nJe boeking is goedgekeurd.\nBevestiging: {booking.Id}",
                fileName,
                data
            );

            return Ok("Boeking goedgekeurd en e-mail verzonden.");
        }

    }
}