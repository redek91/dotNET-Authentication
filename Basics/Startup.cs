using Basics.AuthorizationRequirements;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Basics
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // The order does not count here

            // Define the authentication scheme that will be used by the middleware authentication
            // Middleware
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, config =>
                {
                    config.Cookie.Name = "Grandmas.Cookie";
                    config.LoginPath = "/Home/Authenticate";
                });

            // Authorization handler
            services.AddAuthorization(config =>
            {
                //var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                //var defaultAuthPolicy = defaultAuthBuilder
                //.RequireAuthenticatedUser()
                ////.RequireClaim(ClaimTypes.DateOfBirth)
                //.Build();

                //// DefaultPolicy would be only authenticated required
                //config.DefaultPolicy = defaultAuthPolicy;

                // Custom implementation
                config.AddPolicy("Claim.DoB", policyBuilder =>
                {
                    policyBuilder.AddRequirements(new CustomRequireClaim(ClaimTypes.DateOfBirth));
                });

                // Roles are just legacy => now Claims
                config.AddPolicy("Admin", policyBuilder => policyBuilder.RequireClaim(ClaimTypes.Role, "Admin"));
            });

            services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Middlewares in execution order!

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // The order is important here!!! First identify the route, then check authorization,
            // then ping the endpoint

            app.UseRouting();

            // Add Authentication middleware (who are you?)
            app.UseAuthentication();

            // Add the Authorization middleware for authentication (are you allowed?)
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // Maps the controllers by naming conventions
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
