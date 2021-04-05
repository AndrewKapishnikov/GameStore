using System.Threading.Tasks;

namespace GameStore.Web.App.Interfaces
{
    public interface IChangeGameService
    {
        Task AddNewGame(GameModel gameNew);
        Task UpdateGame(GameModel gameNew);
        Task RemoveGameByGameId(int gameId);
    }
}
