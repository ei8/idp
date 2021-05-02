// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ei8.IdP.Adapted
{
    /// <summary>
    /// Extension methods to add ASP.NET Identity support to IdentityServer.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        internal static IdentityBuilder AddIdentityCustom<TUser, TRole>(
            this IServiceCollection services)
            where TUser : class
            where TRole : class
        {
            services.TryAddScoped<IUserConfirmation<TUser>, DefaultUserConfirmation<TUser>>();

            services.TryAddScoped<IUserValidator<TUser>, UserValidator<TUser>>();
            services.TryAddScoped<IPasswordValidator<TUser>, PasswordValidator<TUser>>();
            services.TryAddScoped<IPasswordHasher<TUser>, PasswordHasher<TUser>>();
            services.TryAddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();
            services.TryAddScoped<IRoleValidator<TRole>, RoleValidator<TRole>>();
            // No interface for the error describer so we can add errors without rev'ing the interface
            services.TryAddScoped<IdentityErrorDescriber>();
            services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<TUser>>();
            services.TryAddScoped<ITwoFactorSecurityStampValidator, TwoFactorSecurityStampValidator<TUser>>();
            services.TryAddScoped<IUserClaimsPrincipalFactory<TUser>, UserClaimsPrincipalFactory<TUser, TRole>>();
            services.TryAddScoped<UserManager<TUser>>();
            services.TryAddScoped<SignInManager<TUser>>();
            services.TryAddScoped<RoleManager<TRole>>();

            return new IdentityBuilder(typeof(TUser), typeof(TRole), services);
        }

        /// <summary>
        /// Configures IdentityServer to use the ASP.NET Identity implementations 
        /// of IUserClaimsPrincipalFactory, IResourceOwnerPasswordValidator, and IProfileService.
        /// Also configures some of ASP.NET Identity's options for use with IdentityServer (such as claim types to use
        /// and authentication cookie settings).
        /// </summary>
        /// <typeparam name="TUser">The type of the user.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        // from https://github.com/IdentityServer/IdentityServer4/blob/main/src/AspNetIdentity/src/IdentityServerBuilderExtensions.cs#L35
        internal static IIdentityServerBuilder AddAspNetIdentityCustom<TUser>(this IIdentityServerBuilder builder)
           where TUser : class            
        {
            // TODO: builder.Services.AddTransientDecorator<IUserClaimsPrincipalFactory<TUser>, UserClaimsFactory<TUser>>();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserIdClaimType = JwtClaimTypes.Subject;
                options.ClaimsIdentity.UserNameClaimType = JwtClaimTypes.Name;
                options.ClaimsIdentity.RoleClaimType = JwtClaimTypes.Role;
            });

            //builder.Services.Configure<SecurityStampValidatorOptions>(opts =>
            //{
            //    opts.OnRefreshingPrincipal = SecurityStampValidatorCallback.UpdatePrincipal;
            //});

            //builder.Services.ConfigureApplicationCookie(options =>
            //{
            //    options.Cookie.IsEssential = true;
            //    // we need to disable to allow iframe for authorize requests
            //    options.Cookie.SameSite = SameSiteMode.None;
            //});

            //builder.Services.ConfigureExternalCookie(options =>
            //{
            //    options.Cookie.IsEssential = true;
            //    // https://github.com/IdentityServer/IdentityServer4/issues/2595
            //    options.Cookie.SameSite = SameSiteMode.None;
            //});

            //builder.Services.Configure<CookieAuthenticationOptions>(IdentityConstants.TwoFactorRememberMeScheme, options =>
            //{
            //    options.Cookie.IsEssential = true;
            //});

            //builder.Services.Configure<CookieAuthenticationOptions>(IdentityConstants.TwoFactorUserIdScheme, options =>
            //{
            //    options.Cookie.IsEssential = true;
            //});

            //builder.Services.AddAuthentication(options =>
            //{
            //    if (options.DefaultAuthenticateScheme == null &&
            //        options.DefaultScheme == IdentityServerConstants.DefaultCookieAuthenticationScheme)
            //    {
            //        options.DefaultScheme = IdentityConstants.ApplicationScheme;
            //    }
            //});

            builder.AddResourceOwnerValidator<ResourceOwnerPasswordValidator<TUser>>();
            builder.AddProfileService<UserProfileService>();

            return builder;
        }
    }
}
