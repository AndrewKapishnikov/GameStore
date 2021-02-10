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
        Task<Game> GetGameByIdAsync(int id);

        Game[] GetLastSixGameByDataAdding();
        Task<Game[]> GetLastSixGameByDataAddingAsync();

        Task<Game[]> GetAllGamesNotOnSaleAsync();
    }
}
