using BookingService.Models;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BookingService.Services
{
    public class CosmosDbService : ICosmosDbService
    {
        private readonly Container _container;

        //Constructor: container ophalen DI
        public CosmosDbService(CosmosClient dbClient, string databaseName, string containerName)
        {
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        // Voeg nieuwe boeking toe 
        public async Task AddBookingAsync(BookingResponse booking)
        {
            await _container.CreateItemAsync(booking, new PartitionKey(booking.ConfirmationCode));
        }

        // Verwijdert boeking obv confirmationcode
        public async Task DeleteBookingAsync(string confirmationCode)
        {
            await _container.DeleteItemAsync<BookingResponse>(confirmationCode, new PartitionKey(confirmationCode));
        }

        // Alle boekingen ophalen 
        public async Task<IEnumerable<BookingResponse>> GetBookingsAsync()
        {
            //Query haalt alle boekingen op
            var query = _container.GetItemQueryIterator<BookingResponse>(new QueryDefinition("SELECT * FROM c"));
            var results = new List<BookingResponse>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        // Boeking zoeken op naam
        public async Task<IEnumerable<BookingResponse>> GetBookingsByNameAsync(string name)
        {
            var query = _container.GetItemQueryIterator<BookingResponse>(new QueryDefinition("SELECT * FROM c WHERE c.Guestname = @name").WithParameter("@name", name));

            var results = new List<BookingResponse>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        // Bestaande boeking bijwerken 
        public async Task UpdateBookingAsync(BookingResponse booking)
        {
            await _container.UpsertItemAsync(booking, new PartitionKey(booking.ConfirmationCode));
        }
    }
}