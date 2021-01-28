using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.MemoryStorage
{
    public static class InfrastructureMemoryStorageExtensions
    {
        public static IServiceCollection AddMemoryStorageRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IGameMemoryRepository, GameMemoryRepository>();
            services.AddSingleton<IOrderMemoryRepository, OrderMemoryRepository>();
            return services;
        }
    }
}
