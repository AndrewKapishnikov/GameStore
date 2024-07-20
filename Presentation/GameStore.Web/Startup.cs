using GameStore.MemoryStorage;
using GameStore.Data.EF;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using GameStore.Web.ExtensionsMethods;
using WebMarkupMin.AspNetCore5;

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
            services.AddAspNetCoreIdentity();
            services.AddCookie();
            services.AddAuthorizationService();

            services.AddApplicationServices(Configuration);

            services.AddWebMarkupMin(options =>
            {
               options.AllowMinificationInDevelopmentEnvironment = true;
               options.AllowCompressionInDevelopmentEnvironment = true;
            }).AddHtmlMinification()
              .AddHttpCompression();
          
            services.AddControllersWithViews();
        }

      
        public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
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
           
            app.UseRequestLocalizationOptions();
            app.UseStaticOptionsFiles();

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
