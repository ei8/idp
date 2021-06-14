using ei8.IdP.Models;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ei8.IdP.Adapted
{
    internal class UserProfileService : IProfileService
    {
        protected UserManager<ApplicationUser> _userManager;

        // https://stackoverflow.com/questions/44761058/how-to-add-custom-claims-to-access-token-in-identityserver4

        public UserProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            //>Processing
            var user = await _userManager.GetUserAsync(context.Subject);
            IList<Claim> claims = new Claim[0];
            if (user != null)
                claims = await _userManager.GetClaimsAsync(user);
            else if (context.Subject.Claims?.Count() > 0)
                claims = context.Subject.Claims.ToArray();

            context.IssuedClaims.AddRange(claims);

            if (!context.IssuedClaims.Any(c => c.Type == JwtClaimTypes.Name))
            {
                var name = string.Empty;
                // get given_name which is used as an internal claim
                var gn = context.IssuedClaims.FirstOrDefault(c => c.Type == JwtClaimTypes.GivenName);
                if (gn != null)
                    name += gn.Value;
                var fn = context.IssuedClaims.FirstOrDefault(c => c.Type == JwtClaimTypes.FamilyName);
                if (fn != null)
                {
                    if (!string.IsNullOrEmpty(name))
                        name += " ";

                    name += fn.Value;
                }

                context.IssuedClaims.Add(new Claim(JwtClaimTypes.Name, name));
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            //>Processing
            var user = await _userManager.GetUserAsync(context.Subject);

            context.IsActive = true; // TODO: (user != null) && user.IsActive;
        }
    }
}
