using GameStore.DataEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore
{
    public interface IGameRepository
    {
        Game[] GetAllByNameOrPublisher(string nameOrPublisher);
        Task<Game[]> GetAllByNameOrPublisherAsync(string nameOrPublisher);

        Game[] GetAllByCategory(string category);
        Task<Game[]> GetAllByCategoryAsync(string category);

        Game[] GetGamesByIds(IEnumerable<int> gamesId);
        Task<Game[]> GetGamesByIdsAsync(IEnumerable<int> gamesId);

        Game GetGameById(int id);
        Task<Game> GetGameByIdAsync(int id, bool withCategory);

        Game[] GetLastEightGameByDataAdding();
        Task<Game[]> GetLastEightGameByDataAddingAsync();

        Task<Game[]> GetAllGamesNotOnSaleAsync();
        
        Task<Game[]> GetGamesForAdminPanel(int pageNo, int pageSize, string sortColumn, bool sortByAscending);
        Task<int> TotalItems();

        IQueryable<GameDTO> GetAllGames();
        Task AddGame(Game game);
        Task RemoveGame(Game game);
        Task UpdateGame(Game game);

    }
}
