using GameStore.Data.EF;
using GameStore.DataEF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;


namespace GameStore.Web.ExtensionsMethods
{
    static class AspNetIdentityExtensions
    {
        public static IServiceCollection AddAspNetCoreIdentity(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.Lockout.MaxFailedAccessAttempts = 7;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.AllowedForNewUsers = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = true;

            }).AddEntityFrameworkStores<GameStoreDbContext>()
              .AddDefaultTokenProviders()
              .AddErrorDescriber<CustomIdentityErrorDescriber>();

            return services;
        }
        public static IServiceCollection AddCookie(this IServiceCollection services)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                //options.Cookie.SecurePolicy = CookieSecurePolicy.None; //comment with https
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
                options.LoginPath = "/account/register";
                options.AccessDeniedPath = "/account/accessdenied";

            });
            return services;
        }

        public static IServiceCollection AddAuthorizationService(this IServiceCollection services)
        {
            return services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));
                options.AddPolicy("ModeratorAndAdminRolePolicy", policy => policy.RequireRole("Moderator", "Admin"));
            });
        }
    }
}
