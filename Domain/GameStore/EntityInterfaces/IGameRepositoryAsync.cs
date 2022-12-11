using GameStore.DataEF;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.EntityInterfaces
{
    public interface IGameRepositoryAsync
    {
        Task<int> TotalItemsAsync();
        IQueryable<GameDTO> GetAllGames();
        Task AddGameAsync(Game game);
        Task RemoveGameAsync(Game game);
        Task UpdateGameAsync(Game game);
    }
}
