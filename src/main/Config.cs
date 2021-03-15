using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDP
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource("country", new [] { "country" })
            };


        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource("bethanyspieshophrapi",
                    "Bethany's Pie Shop HR API",
                    new [] { "country" })
            };


        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "bethanyspieshophr",
                    ClientName = "Bethany's Pie Shop HRM",
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 120,
                    RequireConsent = false,
                    RequirePkce = true,
                    AllowedGrantTypes = GrantTypes.Code,
                    ClientSecrets = {
                        new Secret("108B7B4F-BEFC-4DD2-82E1-7F025F0F75D0".Sha256()) },
                    RedirectUris = { "https://localhost:44301/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:44301/signout-oidc" },
                    AllowedScopes = { "openid", "profile", "email", "bethanyspieshophrapi" }
                }
            };
    }
}
