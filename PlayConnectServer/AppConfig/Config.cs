using Duende.IdentityServer.Models;

namespace PlayConnectServer.AppConfig
{
    public static class Config
    {
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("api1", "My API")
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "client",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        // "K7gNU3sdo+OL0wNhqoVWhr3g6s1xYv72ol/pe/Unols="
                        new Secret("secret".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = { "api1" },
                    ClientClaimsPrefix = "",
                    Claims = new List<ClientClaim>
                    {
                        new ClientClaim("PlayWebApp.TenantId","d40bb4cc-c099-46f3-bdc2-caca4aff8a0a"),
                        new ClientClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier","ccac9e00-c572-410f-b548-05ac19dd65aa"),
                    }
                }
            };
    }
}