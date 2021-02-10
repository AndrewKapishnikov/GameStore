using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Data.EF
{
    public class GameRepository : IGameRepository
    {
        private readonly ContextDBFactory dbContextFactory;
        public GameRepository(ContextDBFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public Game[] GetAllByCategory(string categoryUrlSlug)
        {
            var db = dbContextFactory.Create(typeof(GameRepository));
            var gamesDto = db.Games.Include(p => p.Category).Where(p => p.Category.UrlSlug == categoryUrlSlug && p.OnSale);
            return gamesDto.Select(Game.Mapper.Map).ToArray();
        }
        public async Task<Game[]> GetAllByCategoryAsync(string categoryUrlSlug)
        {
            var db = dbContextFactory.Create(typeof(GameRepository));
            var gamesDto = await db.Games.Include(p => p.Category)
                                   .Where(p => p.Category.UrlSlug == categoryUrlSlug && p.OnSale)
                                   .ToArrayAsync();
            return gamesDto.Select(Game.Mapper.Map).ToArray();
        }


        public Game[] GetAllByNameOrPublisher(string nameOrPublisher)
        {
            string strSearch = $"\"{nameOrPublisher.Trim()}\"";
            var db = dbContextFactory.Create(typeof(GameRepository));
            var sqlParameter = new SqlParameter("@nameOrPublisher", strSearch);
            var gamesDto = db.Games.FromSqlRaw("SELECT * FROM Games WHERE CONTAINS((Name, Publisher), @nameOrPublisher)",
                                                  sqlParameter).Where(p => p.OnSale).Include(p => p.Category);
            return gamesDto.Select(Game.Mapper.Map).ToArray();
        }
        public async Task<Game[]> GetAllByNameOrPublisherAsync(string nameOrPublisher)
        {
            string strSearch = $"\"{nameOrPublisher.Trim()}\"";
            var db = dbContextFactory.Create(typeof(GameRepository));
            var sqlParameter = new SqlParameter("@nameOrPublisher", strSearch);
            var gamesDto = await db.Games.FromSqlRaw("SELECT * FROM Games WHERE CONTAINS((Name, Publisher), @nameOrPublisher)", sqlParameter)
                                         .Where(p => p.OnSale)
                                         .Include(p => p.Category)
                                         .ToArrayAsync();
            return gamesDto.Select(Game.Mapper.Map).ToArray();
        }



        public Game GetGameById(int id)
        {
            var db = dbContextFactory.Create(typeof(GameRepository));
            var gameDto = db.Games.Include(p => p.Category).Single(p => p.Id == id); 
            return Game.Mapper.Map(gameDto);
        }
        public async Task<Game> GetGameByIdAsync(int id)
        {
            var db = dbContextFactory.Create(typeof(GameRepository));
            var gameDto = await db.Games.Include(p => p.Category).SingleAsync(p => p.Id == id);
            return Game.Mapper.Map(gameDto);
        }



        public Game[] GetGamesByIds(IEnumerable<int> gamesId)
        {
            var db = dbContextFactory.Create(typeof(GameRepository));
            var gamesDto = db.Games.Include(p => p.Category).Join(gamesId, gameDto => gameDto.Id, id => id, (gamesDto, id) => gamesDto);
            return gamesDto.Select(Game.Mapper.Map).ToArray();
        }
        public async Task<Game[]> GetGamesByIdsAsync(IEnumerable<int> gamesId)
        {
            var db = dbContextFactory.Create(typeof(GameRepository));
            var gamesDto = await db.Games.Include(p => p.Category)
                                   .Join(gamesId, gameDto => gameDto.Id, id => id, (gamesDto, id) => gamesDto)
                                   .ToArrayAsync();

            return gamesDto.Select(Game.Mapper.Map).ToArray();
        }



        public Game[] GetLastSixGameByDataAdding()
        {
            var db = dbContextFactory.Create(typeof(GameRepository));
            var gamesDto = db.Games.Where(p => p.OnSale).Include(p => p.Category).OrderByDescending(p => p.DateOfAdding).Take(6);
            return gamesDto.Select(Game.Mapper.Map).ToArray();
        }
        public async Task<Game[]> GetLastSixGameByDataAddingAsync()
        {
            var db = dbContextFactory.Create(typeof(GameRepository));
            var gamesDto = await db.Games.Where(p => p.OnSale)
                                         .Include(p => p.Category)
                                         .OrderByDescending(p => p.DateOfAdding)
                                         .Take(6)
                                         .ToArrayAsync();
            return gamesDto.Select(Game.Mapper.Map).ToArray();
        }


        public async Task<Game[]> GetAllGamesNotOnSaleAsync()
        {
            var db = dbContextFactory.Create(typeof(GameRepository));
            var gamesDto = await db.Games.Include(p => p.Category)
                                   .Where(p => !p.OnSale)
                                   .ToArrayAsync();
            return gamesDto.Select(Game.Mapper.Map).ToArray();
        }

    }
}
