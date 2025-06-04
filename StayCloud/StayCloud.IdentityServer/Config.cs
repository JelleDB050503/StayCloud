using Duende.IdentityServer.Models;

namespace StayCloud.IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope("staycloud.price.api", "StayCloud PriceService API"),
            new ApiScope("staycloud.booking.api", "StayCloud BookingService API"),
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // m2m client credentials flow client
            new Client
            {
                ClientId = "postman-client",
                ClientName = "Client Credentials Client",

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("superPostmanSecret".Sha256())  },

                AllowedScopes = { "staycloud.price.api", "staycloud.booking.api" }
            },
            new Client
          {
            ClientId = "bookingservice-client",
            ClientName = "Bookingservice",
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets = { new Secret("superBookingSecret".Sha256()) },
            AllowedScopes = { "staycloud.price.api" }
          },
            // interactive client using code flow + pkce
                    new Client
                    {
                        ClientId = "interactive",
                        ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },
                            
                        AllowedGrantTypes = GrantTypes.Code,

                        RedirectUris = { "https://localhost:44300/signin-oidc" },
                        FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

                AllowOfflineAccess = true,
                AllowedScopes = { "openid", "profile", "scope2" }
            },
        };
}
