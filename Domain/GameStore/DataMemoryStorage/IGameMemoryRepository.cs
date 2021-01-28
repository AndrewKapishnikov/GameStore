using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore
{
    public interface IGameMemoryRepository
    {
        GameMemoryStorage[] GetAllByNameOrPublisher(string nameOrPublisher);
        GameMemoryStorage[] GetAllByCategory(string category);
        GameMemoryStorage[] GetGamesByIds(IEnumerable<int> gamesId);
        GameMemoryStorage GetGameById(int id);
        GameMemoryStorage[] GetLastSixGameByDataAdding();
    }
}
