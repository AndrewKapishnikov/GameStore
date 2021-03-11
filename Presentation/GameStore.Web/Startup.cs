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

namespace GameStore.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                            //.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../../Infrastructure/GameStore.MemoryStorage"))
                            //.AddJsonFile("dataimages.json")
                           .AddEnvironmentVariables()
                           .AddConfiguration(configuration);
        
          
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //TestMode GameStore.MemoryStorage in Infrastructure section
            //services.Configure<GameImageData>(options => Configuration.GetSection("ImagesData").Bind(options));
             
            services.AddHttpContextAccessor();
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            services.AddMemoryStorageRepositories();
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
            services.AddScoped<EmailService>();

            services.AddSingleton<GameMemoryService>();
            services.AddSingleton<OrderMemoryService>();

            services.AddSingleton<GameService>();
            services.AddSingleton<OrderService>();
            services.AddSingleton<CategoryService>();
  
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
                //app.UseExceptionHandler("/Error");
                //app.UseStatusCodePagesWithReExecute("/Error/{0}");
                app.UseBrowserLink();
            }
            else
            {
                app.UseHsts();
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");

            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
           
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();
              
           //When use this middleware BrowserLink don't work. Uncomment in Production mode
            app.UseWebMarkupMin();

            //For testing purposes
            //app.Use(async (context, next) =>
            //{
            //    var endPoint = context.GetEndpoint();
            //    var routes = context.Request.RouteValues;
            //    await next.Invoke();
            //});

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
