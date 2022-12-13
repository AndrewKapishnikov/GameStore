using GameStore.MemoryStorage;
using GameStore.Web.App;
using GameStore.Data.EF;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using GameStore.DataEF;
using Microsoft.AspNetCore.Identity;
using GameStore.Web.ExtensionsMethods;
using GameStore.Contractors;
using GameStore.Contractors.Interfaces;
using WebMarkupMin.AspNetCore5;
using GameStore.Web.App.Models;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using GameStore.Web.App.Interfaces;

namespace GameStore.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                           //.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../../Infrastructure/GameStore.MemoryStorage"))
                           //.AddJsonFile("dataimages.json")   //TestMode MemoryStorage
                           .AddEnvironmentVariables()
                           .AddConfiguration(configuration);
          
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //TestMode MemoryStorage GameStore.MemoryStorage in Infrastructure section
            //services.Configure<GameImageData>(options => Configuration.GetSection("ImagesData").Bind(options));

            services.AddHttpContextAccessor();
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            //TestMode MemoryStorage
            //services.AddMemoryStorageRepositories();

            services.AddEntityFrameworkRepositories(Configuration.GetConnectionString("GameStore"));
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
            
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));
                options.AddPolicy("ModeratorAndAdminRolePolicy", policy => policy.RequireRole("Moderator", "Admin"));
            });
         
            services.AddScoped(service => Configuration.GetSection("PayPal").Get<PayPalConfig>());
            services.AddScoped(service => Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>());
            services.AddScoped<AbstractEmailService, EmailService>();

            //TestMode MemoryStorage
            //services.AddSingleton<GameMemoryService>();
            //services.AddSingleton<OrderMemoryService>();

            services.AddScoped<IChangeGameService, GameService>();
            services.AddScoped<IGetGamesService, GameService>();
            services.AddScoped<AbstractCategoryService, CategoryService>();
            services.AddScoped<AbstractOrderService, OrderService>();
  
            services.AddSingleton<IDeliveryService, PostamateDeliveryService>();
            services.AddSingleton<IDeliveryService, CourierDeliveryService>();
            services.AddSingleton<IPaymentService, CashPaymentService>();
            services.AddSingleton<IPaymentService, PayPalPaymentService>();
            services.AddSingleton<IPaymentService, EmulateKassaPaymentService>();
            services.AddSingleton<IExternalWebService, EmulateKassaPaymentService>();
         
            services.AddWebMarkupMin(options =>
            {
                options.AllowMinificationInDevelopmentEnvironment = true;
                options.AllowCompressionInDevelopmentEnvironment = true;
            }).AddHtmlMinification()
              .AddHttpCompression();
          
            services.AddControllersWithViews();
        }

      
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();         
                //app.UseExceptionHandler("/Error");
                //app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }
            else
            {
                app.UseHsts();
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                app.UseWebMarkupMin();
            }
            
            app.UseHttpsRedirection();
          
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();
             
            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("en"),
                new CultureInfo("ru-RU"),
                new CultureInfo("ru")
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("ru-RU"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = s =>
                {
                    if (s.Context.Request.Path.StartsWithSegments(new PathString("/js")) &&
                       !s.Context.User.IsInRole("Admin"))
                    {
                        s.Context.Response.StatusCode = 404;
                        s.Context.Response.Redirect("/error/404");
                    }
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                   name: "areas",
                   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }


    }
}
