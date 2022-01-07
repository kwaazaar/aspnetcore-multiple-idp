// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System;
using System.Collections.Generic;

namespace idp
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            { 
                new IdentityResources.OpenId()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("api1", "My API1", userClaims: new string[] { "user_id", "nonexisting" }),
                new ApiScope("api2", "My API2")
            };

        public static IEnumerable<Client> Clients =>
            new Client[] 
            {
                new Client
                        {
                            ClientId = "client",

                            AllowOfflineAccess = true, // Sends refresh token (unless a specific scope was specified)
                            UpdateAccessTokenClaimsOnRefresh = true,

                            // no interactive user, use the clientid/secret for authentication
                            AllowedGrantTypes = GrantTypes.ClientCredentials,

                            // Claims only works for client_credentials, unless: AlwaysSendClientClaims = true,
                            // Claims will be prefixed with "client_", change with ClientClaimsPrefix
                            Claims = new List<ClientClaim> { new ClientClaim ("always_included", "1"), },

                            // secret for authentication
                            ClientSecrets =
                            {
                                new Secret("secret".Sha256())
                            },

                            // scopes that client has access to
                            AllowedScopes = { "api1", "api2" }
                        },
                new Client
                        {
                            ClientId = "fancy_client",

                            AllowOfflineAccess = true, // Sends refresh token (unless a specific scope was specified)
                            UpdateAccessTokenClaimsOnRefresh = true,

                            // no interactive user, use the clientid/secret for authentication
                            AllowedGrantTypes = new List<string> { "fancy" },
                    
                            RequireClientSecret = false, // Make optional (secret is ignored when specified): only for public/untrusted clients -> usually other types of credentials are provided in the token request as well

                            // secret for authentication
                            ClientSecrets =
                            {
                                new Secret("secret".Sha256())
                            },

                            // scopes that client has access to
                            AllowedScopes = { "api1", "api2" }
                        },
            };
    }
}