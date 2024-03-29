﻿using GameStore.Data.EF.Repositories;
using GameStore.EntityInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;


namespace GameStore.Data.EF
{
    public static class EntityFrameworkExtensions
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
            services.AddSingleton<IGetGamesRepositoryAsync, GameRepository>();
            services.AddSingleton<IOrderRepositoryAsync, OrderRepository>();
            services.AddSingleton<ICategoryRepositoryAsync, CategoryRepository>();

            return services;
        }

    }
}
