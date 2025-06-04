            // Voor token ophalen en discovery
using System.Net.Http;                  // Voor HttpClient
using System.Net.Http.Json;             // Voor PostAsJsonAsync
using Microsoft.Extensions.Configuration;
using BookingService.Models;
using Duende.IdentityModel.Client;  // Voor IConfiguration


public class PriceServiceClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;

    public PriceServiceClient(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
    }

    public async Task<PriceResponse?> CalculatePriceAsync(object priceRequest)
    {
        var httpClient = _httpClientFactory.CreateClient();

        var disco = await httpClient.GetDiscoveryDocumentAsync("https://staycloud-identity-jdb.azurewebsites.net");
        if (disco.IsError)
            throw new Exception(disco.Error);

        var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = "bookingservice-client",
            ClientSecret = "superBookingSecret",
            Scope = "staycloud.price.api"
        });

        if (tokenResponse.IsError)
            throw new Exception(tokenResponse.Error);

        httpClient.SetBearerToken(tokenResponse.AccessToken);

        var priceServiceUrl = _config["PriceService:BaseUrl"];
        var response = await httpClient.PostAsJsonAsync($"{priceServiceUrl}/api/price/calculate", priceRequest);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<PriceResponse>();
    }
}
