using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Web.App
{
    public class GameService
    {
        private readonly IGameRepository gameRepository;

        public GameService(IGameRepository gameRepository)
        {
            this.gameRepository = gameRepository;
        }

        public async Task<GameModel> GetGameByIdAsync(int id)
        {
            var game = await gameRepository.GetGameByIdAsync(id);

            return Map(game);
        }
        public async Task<IReadOnlyCollection<GameModel>> GetAllGamesByNameOrPublisherAsync(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var game = await gameRepository.GetAllByNameOrPublisherAsync(query);
                return game.Select(Map).ToArray();
            }
            return new List<GameModel>();
        }

        public async Task<IReadOnlyCollection<GameModel>> GetAllGamesByCategoryAsync(string query)
        {
            var game = await gameRepository.GetAllByCategoryAsync(query);

            return game.Select(Map).ToArray();
        }

        public async Task<IReadOnlyCollection<GameModel>> GetGamesByDescedingOrderAsync()
        {
            var game = await gameRepository.GetLastSixGameByDataAddingAsync();

            return game.Select(Map).ToArray();
        }

        private GameModel Map(Game game)
        {
            return new GameModel
            {
                GameId = game.Id,
                Publisher = game.Publisher,
                Category = game.Category.Name,
                Name = game.Name,
                ShortDescription = game.ShortDescription,
                Description = game.Description,
                Price = game.Price,
                ImageData = game.ImageData,
                ReleaseDate = game.ReleaseDate,
                DateOfAdding = game.DateOfAdding
            };
        }
    }
}
