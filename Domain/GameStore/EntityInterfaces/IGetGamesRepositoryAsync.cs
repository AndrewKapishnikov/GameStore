using GameStore.EntityInterfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore
{
    public interface IGetGamesRepositoryAsync: IGameRepositoryAsync
    {
        Task<Game[]> GetAllByNameOrPublisherAsync(string nameOrPublisher);
        Task<Game[]> GetAllByCategoryAsync(string category);
        Task<Game[]> GetGamesByIdsAsync(IEnumerable<int> gamesId);
        Task<Game> GetGameByIdAsync(int id, bool withCategory);
        Task<Game[]> GetLastEightGameByDataAddingAsync();
        Task<Game[]> GetAllGamesNotOnSaleAsync();
        Task<Game[]> GetGamesForAdminPanel(int pageNo, int pageSize, string sortColumn, bool sortByAscending);

    }
}

