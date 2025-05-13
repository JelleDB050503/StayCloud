using Azure.Storage.Blobs;
using BookingService.Models;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace BookingService.Services
{
    public class FileProcessor : IFileProcessor
    {
        private readonly IBlobStorageService _blobStorageService;

        public FileProcessor(IBlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        public async Task GenerateAndUploadContractAsync(BookingResponse booking)
        {
            // Bouw huurovereenkomst als tekst
            string agreementText = $"Bevestiging: {booking.ConfirmationCode}\n" +
                                   $"Naam: {booking.Guestname}\n" +
                                   $"Email: {booking.Email}\n" +
                                   $"Accommodatie: {booking.AccommodationType}\n" +
                                   $"Verblijfstype: {booking.StayType}\n" +
                                   $"Aantal nachten: {booking.TotalNights}\n" +
                                   $"Prijs: €{booking.TotalPrice}";

            // Upload naar Blob Storage
            string fileName = $"{booking.ConfirmationCode}.txt";
            await _blobStorageService.UploadContractAsync(fileName, agreementText);
        }
       
    }
}