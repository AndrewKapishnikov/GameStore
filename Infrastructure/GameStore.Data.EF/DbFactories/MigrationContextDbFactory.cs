using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;


namespace GameStore.Data.EF
{
    public class MigrationContextDbFactory : IDesignTimeDbContextFactory<GameStoreDbContext>
    {
        GameStoreDbContext IDesignTimeDbContextFactory<GameStoreDbContext>.CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../../Presentation/GameStore.Web"))
            .AddJsonFile("appsettings.json", optional: true).Build();

            var optionsBuilder = new DbContextOptionsBuilder<GameStoreDbContext>();
            optionsBuilder.UseSqlServer<GameStoreDbContext>(config.GetConnectionString("GameStore"));
            
            return new GameStoreDbContext(optionsBuilder.Options);
        }
    }
}
