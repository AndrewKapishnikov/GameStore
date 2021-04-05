using GameStore.Web.App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.Web.App.Interfaces
{
    public interface IGetGamesService
    {
        Task<GameModel> GetGameByIdAsync(int id);
        Task<IReadOnlyCollection<GameModel>> GetAllGamesByNameOrPublisherAsync(string query);
        Task<(IReadOnlyCollection<GameModel>, int)> GetAllGamesByCategoryAsync(string categoryUrlSlug, int pageNumber, int pageSize);
        Task<IReadOnlyCollection<GameModel>> GetGamesByDescedingOrderAsync();
        Task<(IReadOnlyCollection<GameModel>, int)> GetGamesForAdminByPageAsync(int pageNo, int pageSize, SortGameStates sortGame);
        Task<(IReadOnlyCollection<GameModel>, int)> GetGamesForAdminByCategoryAndNameAsync(int pageNo, int pageSize,
                                                    SortGameStates sortGame, string gameName, int categoryId);
    }
}
