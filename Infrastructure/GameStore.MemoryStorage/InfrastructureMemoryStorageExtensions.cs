using Microsoft.Extensions.DependencyInjection;


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
