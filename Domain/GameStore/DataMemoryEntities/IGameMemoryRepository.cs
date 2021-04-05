using System.Collections.Generic;


namespace GameStore
{
    public interface IGameMemoryRepository
    {
        GameMemoryEntity[] GetAllByNameOrPublisher(string nameOrPublisher);
        GameMemoryEntity[] GetAllByCategory(string category);
        GameMemoryEntity[] GetGamesByIds(IEnumerable<int> gamesId);
        GameMemoryEntity GetGameById(int id);
        GameMemoryEntity[] GetLastSixGameByDataAdding();
    }
}
