using System.Collections.Generic;

namespace GameStore.EntityInterfaces
{
    public interface IGetGamesRepository
    {
        Game[] GetAllByNameOrPublisher(string nameOrPublisher);
        Game[] GetAllByCategory(string category);
        Game[] GetGamesByIds(IEnumerable<int> gamesId);
        Game GetGameById(int id);
        Game[] GetLastEightGameByDataAdding();
    
    }
}
