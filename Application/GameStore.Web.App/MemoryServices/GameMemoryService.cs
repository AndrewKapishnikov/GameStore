﻿using GameStore.Web.App;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStore
{
    public class GameMemoryService
    {
        private readonly IGameMemoryRepository gameRepository;

        public GameMemoryService(IGameMemoryRepository gameRepository)
        {
            this.gameRepository = gameRepository;
        }

        public GameModel GetGameById(int id)
        {
            var game = gameRepository.GetGameById(id);

            return Map(game);
        }

        public IReadOnlyCollection<GameModel> GetAllGamesByNameOrPublisher(string query)
        {
            var game = gameRepository.GetAllByNameOrPublisher(query);

            return game.Select(Map).ToArray();
        }

        public IReadOnlyCollection<GameModel> GetAllGamesByCategory(string query)
        {
            var game = gameRepository.GetAllByCategory(query);

            return game.Select(Map).ToArray();
        }

        public IReadOnlyCollection<GameModel> GetGamesByDescedingOrder()
        {
            var game = gameRepository.GetLastSixGameByDataAdding();

            return game.Select(Map).ToArray();
        }

        private GameModel Map(GameMemoryEntity game)
        {
            return new GameModel
            {
                GameId = game.Id,
                Publisher = game.Publisher,
                Category = game.Category,
                Name = game.Name,
                ShortDescription = game.ShortDescription,
                Description = game.Description,
                Price = game.Price,
                ImageData = game.ImageData,
                ReleaseDate = DateTime.Now,       
                DateOfAdding = game.DateOfAdding
            };
        }
    }
}
