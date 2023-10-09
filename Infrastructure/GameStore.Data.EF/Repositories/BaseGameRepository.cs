using GameStore.DataEF;
using GameStore.EntityInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.Data.EF.Repositories
{
    public class BaseGameRepository: IGameRepositoryAsync
    {
        private readonly ContextDBFactory dbContextFactory;
        public BaseGameRepository(ContextDBFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        protected GameStoreDbContext GetDbContextForGameRepository()
        {
            return dbContextFactory.Create(typeof(GameRepository));
        }
        //protected GameStoreDbContext GetDbContextForGameRepository<T>()
        //{
        //    return dbContextFactory.Create(typeof(T));
        //}

        public IQueryable<GameDTO> GetAllGames()
        {
            var db = GetDbContextForGameRepository();
            IQueryable<GameDTO> games = db.Games.Include(p => p.Category);
            return games;
        }

        public async Task<int> TotalItemsAsync()
        {
            var db = GetDbContextForGameRepository();
            return await db.Games.CountAsync<GameDTO>();
        }

        public async Task AddGameAsync(Game game)
        {
            var db = GetDbContextForGameRepository();
            var gameDto = Game.Mapper.Map(game);
            await db.Games.AddAsync(gameDto);
            await db.SaveChangesAsync();
        }

        public async Task RemoveGameAsync(Game game)
        {
            var db = GetDbContextForGameRepository();
            var gameDto = Game.Mapper.Map(game);
            db.Games.Remove(gameDto);
            await db.SaveChangesAsync();
        }

        public async Task UpdateGameAsync(Game game)
        {
            var db = GetDbContextForGameRepository();
            var gameDto = Game.Mapper.Map(game);
            db.Entry(gameDto).State = EntityState.Modified;
            await db.SaveChangesAsync();

        }
    }
}
