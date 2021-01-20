using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore
{
    public class GameService
    {
        private readonly IGameRepository bookRepository;

        public GameService(IGameRepository bookRepository)
        {
            this.bookRepository = bookRepository;
        }

        public Game[] GetGamesByQuery(string query)
        {
              return bookRepository.GetAllByNameOrPublisher(query);
        }

        public Game[] GetGamesByCategory(string category)
        {
            return bookRepository.GetAllByCategory(category);
        }
    }
}
