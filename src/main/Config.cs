using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ei8.IdP.Constants;

namespace ei8.IdP
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email() 
                // TODO:,
                // new IdentityResource("country", new [] { "country" })
            };


        public static IEnumerable<ApiResource> Apis =>
           new ApiResource[]
           {
                new ApiResource("avatarapi",
                    "ei8 Avatar")
                {
                    Scopes = new[] { "avatarapi" },
                    ApiSecrets = new[] { new Secret(Environment.GetEnvironmentVariable(EnvironmentVariableKeys.ApisAvatarSecret).ToSha256())}
                }
               // TODO: ,
               // new [] { "country" })
           };


        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "d23",
                    ClientName = "ei8 d#",
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 120,
                    RequireConsent = true,
                    RequirePkce = true,
                    AllowedGrantTypes = GrantTypes.Code,
                    ClientSecrets = { new Secret( Environment.GetEnvironmentVariable(EnvironmentVariableKeys.ClientsD23Secret).Sha256()) },
                    RedirectUris = { Environment.GetEnvironmentVariable(EnvironmentVariableKeys.ClientsD23) + Constants.Paths.Login },
                    PostLogoutRedirectUris = { Environment.GetEnvironmentVariable(EnvironmentVariableKeys.ClientsD23) + Constants.Paths.Logout },
                    AllowedScopes = { IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile, IdentityServerConstants.StandardScopes.Email, "avatarapi" },
                    // TODO: Should ideally be TokenUsage.OneTimeOnly
                    RefreshTokenUsage = TokenUsage.ReUse
                }
            };

        public static IEnumerable<ApiScope> ApiScopes =>
             new List<ApiScope>
             {
                 new ApiScope(name: "avatarapi", displayName: "Access Avatar API endpoints.")
             };
    }
}
