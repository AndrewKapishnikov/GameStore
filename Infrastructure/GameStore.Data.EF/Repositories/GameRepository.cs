using GameStore.Data.EF.Repositories;
using GameStore.DataEF;
using GameStore.EntityInterfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.Data.EF
{
    public class GameRepository : BaseGameRepository, IGetGamesRepositoryAsync, IGetGamesRepository
    {
        public GameRepository(ContextDBFactory dbContextFactory) : base(dbContextFactory) { }
       
        public Game[] GetAllByCategory(string categoryUrlSlug)
        {
            var db = GetDbContextForGameRepository();
            var gamesDto = db.Games.Include(p => p.Category).Where(p => p.Category.UrlSlug == categoryUrlSlug && p.OnSale);
            return gamesDto.Select(Game.Mapper.Map).ToArray();
        }
        public async Task<Game[]> GetAllByCategoryAsync(string categoryUrlSlug)
        {
            var db = GetDbContextForGameRepository();
            var gamesDto = await db.Games.Include(p => p.Category)
                                   .Where(p => p.Category.UrlSlug == categoryUrlSlug && p.OnSale)
                                   .ToArrayAsync();
            return gamesDto.Select(Game.Mapper.Map).ToArray();
        }


        public Game[] GetAllByNameOrPublisher(string nameOrPublisher)
        {
            string strSearch = $"\"{nameOrPublisher.Trim()}\"";
            var db = GetDbContextForGameRepository();
            var sqlParameter = new SqlParameter("@nameOrPublisher", strSearch);
            var gamesDto = db.Games.FromSqlRaw("SELECT * FROM Games WHERE CONTAINS((Name, Publisher), @nameOrPublisher)",
                                                  sqlParameter).Where(p => p.OnSale).Include(p => p.Category);
            return gamesDto.Select(Game.Mapper.Map).ToArray();
        }
        public async Task<Game[]> GetAllByNameOrPublisherAsync(string nameOrPublisher)
        {
            string strSearch = $"\"{nameOrPublisher.Trim()}\"";
            var db = GetDbContextForGameRepository();
            var sqlParameter = new SqlParameter("@nameOrPublisher", strSearch);
            var gamesDto = await db.Games.FromSqlRaw("SELECT * FROM Games WHERE CONTAINS((Name, Publisher), @nameOrPublisher)", sqlParameter)
                                         .Where(p => p.OnSale)
                                         .Include(p => p.Category)
                                         .ToArrayAsync();
            return gamesDto.Select(Game.Mapper.Map).ToArray();
        }


        public Game GetGameById(int id)
        {
            var db = GetDbContextForGameRepository();
            var gameDto = db.Games.Include(p => p.Category).Single(p => p.Id == id);

            return Game.Mapper.Map(gameDto);
        }
        public async Task<Game> GetGameByIdAsync(int id, bool withCategory)
        {
            var db = GetDbContextForGameRepository();
            GameDTO gameDto;
            if(withCategory)
            {
                gameDto = await db.Games.Include(p => p.Category).SingleAsync(p => p.Id == id);
            }
            else
            {
                gameDto = await db.Games.SingleAsync(p => p.Id == id);
            }
       
            return Game.Mapper.Map(gameDto);
        }



        public Game[] GetGamesByIds(IEnumerable<int> gamesId)
        {
            var db = GetDbContextForGameRepository();
            var gamesDto = db.Games.Include(p => p.Category).Join(gamesId, gameDto => gameDto.Id, id => id, (gamesDto, id) => gamesDto);
            return gamesDto.Select(Game.Mapper.Map).ToArray();
        }
        public async Task<Game[]> GetGamesByIdsAsync(IEnumerable<int> gamesId)
        {
            var db = GetDbContextForGameRepository();
            var gamesDto = await db.Games.Include(p => p.Category)
                                   .Join(gamesId, gameDto => gameDto.Id, id => id, (gamesDto, id) => gamesDto)
                                   .ToArrayAsync();

            return gamesDto.Select(Game.Mapper.Map).ToArray();
        }



        public Game[] GetLastEightGameByDataAdding()
        {
            var db = GetDbContextForGameRepository();
            var gamesDto = db.Games.Where(p => p.OnSale).Include(p => p.Category).OrderByDescending(p => p.DateOfAdding).Take(8);
            return gamesDto.Select(Game.Mapper.Map).ToArray();
        }
        public async Task<Game[]> GetLastEightGameByDataAddingAsync()
        {
            var db = GetDbContextForGameRepository();
            var gamesDto = await db.Games.Where(p => p.OnSale)
                                         .Include(p => p.Category)
                                         .OrderByDescending(p => p.DateOfAdding)
                                         .Take(8)
                                         .ToArrayAsync();
            return gamesDto.Select(Game.Mapper.Map).ToArray();
        }


        public async Task<Game[]> GetAllGamesNotOnSaleAsync()
        {
            var db = GetDbContextForGameRepository();
            var gamesDto = await db.Games.Include(p => p.Category)
                                   .Where(p => !p.OnSale)
                                   .ToArrayAsync();
            return gamesDto.Select(Game.Mapper.Map).ToArray();
        }

        public async Task<Game[]> GetGamesForAdminPanel(int pageNo, int pageSize, string sortColumn, bool sortByAscending)
        {
            var db = GetDbContextForGameRepository();
            GameDTO[] games = null;
            switch (sortColumn)
            {
                case nameof(Game.Name):
                    if (sortByAscending)
                    {
                        games = await db.Games.OrderBy(p => p.Name)
                                        .Include(p => p.Category)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .ToArrayAsync();
                    }
                    else
                    {
                        games = await db.Games.OrderByDescending(p => p.Name)
                                        .Include(p => p.Category)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .ToArrayAsync();
                    }
                    break;
                case nameof(Game.GameDescription.Publisher):
                    if (sortByAscending)
                    {
                        games = await db.Games.OrderBy(p => p.Publisher)
                                        .Include(p => p.Category)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .ToArrayAsync();
                    }
                    else
                    {
                        games = await db.Games.OrderByDescending(p => p.Publisher)
                                        .Include(p => p.Category)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .ToArrayAsync();
                    }
                    break;
                case nameof(Game.Price):
                    if (sortByAscending)
                    {
                        games = await db.Games.OrderBy(p => p.Price)
                                        .Include(p => p.Category)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .ToArrayAsync();
                    }
                    else
                    {
                        games = await db.Games.OrderByDescending(p => p.Price)
                                        .Include(p => p.Category)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .ToArrayAsync();
                    }
                    break;
                case nameof(Game.DateOfAdding):
                    if (sortByAscending)
                    {
                        games = await db.Games.OrderBy(p => p.DateOfAdding)
                                        .Include(p => p.Category)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .ToArrayAsync();
                    }
                    else
                    {
                        games = await db.Games.OrderByDescending(p => p.DateOfAdding)
                                        .Include(p => p.Category)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .ToArrayAsync();
                    }
                    break;

                default:
                    games = await db.Games.OrderBy(p => p.Name)
                                    .Include(p => p.Category)
                                    .Skip(pageNo * pageSize)
                                    .Take(pageSize)
                                    .ToArrayAsync();
                    break;
            }

            return games.Select(Game.Mapper.Map).ToArray();
        }

    }
}
