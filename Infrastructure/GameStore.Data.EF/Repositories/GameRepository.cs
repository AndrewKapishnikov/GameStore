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
        public Game[] GetAllByCategory(string category)
        {
            var db = dbContextFactory.Create(typeof(GameRepository));
            var gamesDto = db.Games.Include(p => p.Category).Where(p => p.Category.Name == category);

            return gamesDto.Select(Game.Mapper.Map).ToArray();
        }

        public Game[] GetAllByNameOrPublisher(string nameOrPublisher)
        {
            string strSearch = $"\"{nameOrPublisher.Trim()}\"";
            var db = dbContextFactory.Create(typeof(GameRepository));
            var sqlParameter = new SqlParameter("@nameOrPublisher", strSearch);
            var gamesDto = db.Games.FromSqlRaw("SELECT * FROM Games WHERE CONTAINS((Name, Publisher), @nameOrPublisher)",
                                                  sqlParameter).Include(p => p.Category);
            return gamesDto.Select(Game.Mapper.Map).ToArray();
        }

        public Game GetGameById(int id)
        {
            var db = dbContextFactory.Create(typeof(GameRepository));
            var gameDto = db.Games.Include(p => p.Category).Single(p => p.Id == id); 
            return Game.Mapper.Map(gameDto);

        }

        public Game[] GetGamesByIds(IEnumerable<int> gamesId)
        {
            var db = dbContextFactory.Create(typeof(GameRepository));
            var gamesDto = db.Games.Include(p => p.Category).Join(gamesId, gameDto => gameDto.Id, id => id, (gamesDto, id) => gamesDto);
            return gamesDto.Select(Game.Mapper.Map).ToArray();
           
        }

        public Game[] GetLastSixGameByDataAdding()
        {
            var db = dbContextFactory.Create(typeof(GameRepository));
            var gamesDto = db.Games.Include(p => p.Category).OrderByDescending(p => p.DateOfAdding).Take(6);
            return gamesDto.Select(Game.Mapper.Map).ToArray();
        }
    }
}
