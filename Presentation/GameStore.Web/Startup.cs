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
using System.IO;
using GameStore.DataEF;
using Microsoft.AspNetCore.Identity;

namespace GameStore.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                           .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../../Infrastructure/GameStore.MemoryStorage"))
                           .AddJsonFile("dataimages.json")
                           .AddConfiguration(configuration);
          
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<GameImageData>(options => Configuration.GetSection("ImagesData").Bind(options));

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
                //options.SignIn.RequireConfirmedEmail = true;
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            }).AddEntityFrameworkStores<GameStoreDbContext>();

                       
            services.AddSingleton<GameMemoryService>();
            services.AddSingleton<OrderMemoryService>();

            services.AddSingleton<GameService>();
            services.AddSingleton<OrderService>();

            services.AddControllersWithViews();
        }

      
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
           
            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllerRoute(
                //    name: "search",
                //    pattern: "search/{query}",
                //    defaults: new { controller = "Search", action = "SearchByName" }
    
                //);
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

            });
        }
    }
}
