using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace GameStore.Data.EF
{
    public class ContextDBFactory
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public ContextDBFactory(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public GameStoreDbContext Create(Type repositoryType)
        {
            var services = httpContextAccessor.HttpContext.RequestServices;

            var dbContexts = services.GetService<Dictionary<Type, GameStoreDbContext>>();

            if (!dbContexts.ContainsKey(repositoryType))
                dbContexts[repositoryType] = services.GetService<GameStoreDbContext>();

            return dbContexts[repositoryType];
        }
    }
}
