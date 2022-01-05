using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using ei8.IdP.Adapted;
using ei8.IdP.Data;
using ei8.IdP.Models;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.AspNetIdentity;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using static ei8.IdP.Constants;

namespace ei8.IdP
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Environment.GetEnvironmentVariable(EnvironmentVariableKeys.ConnectionStringsDefault)));

            services.AddIdentityCustom<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                // options.EmitStaticAudienceClaim = true;
            });

            // in-memory, code config
            builder.AddInMemoryIdentityResources(Config.Ids)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryApiResources(this.Configuration.GetSection("IdentityServer:ApiResources"))
                .AddInMemoryClients(this.Configuration.GetSection("IdentityServer:Clients"))
                // Necessary for claims retrieval
                .AddAspNetIdentityCustom<ApplicationUser>();

            #region Signing Credential
            // not recommended for production - you need to store your key material somewhere secure
            //if (_env.IsDevelopment())
            //{
            builder.AddDeveloperSigningCredential();
            //}
            //else
            //{
            //var certificate = new X509Certificate2("/https/aspnetapp.pfx", "eB245ebK28ubsQJR");
            //builder.AddSigningCredential(certificate);
            //}
            #endregion

            services.AddAuthentication()
                .AddGoogle("Google", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.ClientId = Environment.GetEnvironmentVariable(EnvironmentVariableKeys.GoogleClientId);
                    options.ClientSecret = Environment.GetEnvironmentVariable(EnvironmentVariableKeys.GoogleClientSecret);
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
