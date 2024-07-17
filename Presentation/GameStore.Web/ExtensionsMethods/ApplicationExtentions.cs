using GameStore.Contractors.Interfaces;
using GameStore.Contractors;
using GameStore.Web.App.Interfaces;
using GameStore.Web.App.Models;
using GameStore.Web.App;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameStore.Web.ExtensionsMethods
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(service => configuration.GetSection("PayPal").Get<PayPalConfig>());
            services.AddScoped(service => configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>());
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

            return services;
        }
    }
}
