using GameStore.DataEF;
using GameStore.MemoryStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace GameStore.Data.EF
{
    public static class InfrastructureEFExtensions
    {
        public static IServiceCollection AddEntityFrameworkRepositories(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<GameStoreDbContext>(
                options =>
                {
                    options.UseSqlServer(connectionString);
                },
                ServiceLifetime.Transient
            );

            services.AddScoped<Dictionary<Type, GameStoreDbContext>>();
            services.AddSingleton<ContextDBFactory>();
            services.AddSingleton<IGameRepository, GameRepository>();
            services.AddSingleton<IOrderRepository, OrderRepository>();
            return services;
        }

    }
}
