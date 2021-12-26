// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace PlayConnectServer.AppConfig
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources
        {
            get
            {
                var prof = new IdentityResources.Profile();
                var oid = new IdentityResources.OpenId();
                prof.UserClaims.Add("PlayWebApp.TenantId");
                return new IdentityResource[] { oid, prof };
            }
        }


        //
        // used to verify the audience
        // if you are getting invalid_audience error, then add your audience name here
        //
        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]{
                new ApiResource("play.services.logistics:API", "Logsitics API")
                {
                    Scopes = { "catalog.fullaccess", "catalog.readonlyaccess" }
                }
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("scope1"),
                new ApiScope("scope2"),
                new ApiScope("logistics.fullaccess"),
                new ApiScope("logistics.readonlyaccess")
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "m2m.client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "scope1" }
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
                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "mvc",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    // where to redirect to after login
                    RedirectUris = { "https://localhost:7096/signin-oidc" },
                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:7096/signout-callback-oidc" },
                    FrontChannelLogoutUri = "https://localhost:7096/signout-oidc",
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                },
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