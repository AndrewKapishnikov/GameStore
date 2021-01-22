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
        Game[] GetAllByCategory(string category);
        Game[] GetGamesByIds(IEnumerable<int> gamesId);
        Game GetGameById(int id);
    }
}
