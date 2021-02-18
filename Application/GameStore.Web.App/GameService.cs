using GameStore.DataEF;
using GameStore.Web.App.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
        private readonly IHttpContextAccessor httpContextAccessor;
        protected ISession Session => httpContextAccessor.HttpContext.Session;

        public GameService(IGameRepository gameRepository,
                           IHttpContextAccessor httpContextAccessor)
        {
            this.gameRepository = gameRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<GameModel> GetGameByIdAsync(int id)
        {
            var game = await gameRepository.GetGameByIdAsync(id, true);

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

        public async Task<IReadOnlyCollection<GameModel>> GetAllGamesByCategoryAsync(string categoryUrlSlug)
        {
            var game = await gameRepository.GetAllByCategoryAsync(categoryUrlSlug);

            return game.Select(Map).ToArray();
        }

        public async Task<IReadOnlyCollection<GameModel>> GetGamesByDescedingOrderAsync()
        {
            var game = await gameRepository.GetLastEightGameByDataAddingAsync();

            return game.Select(Map).ToArray();
        }

        public async Task<(IReadOnlyCollection<GameModel>, int)> GetGamesForAdminByPageAsync(int pageNo, int pageSize, SortGameState sortGame)
        {
            string sortColumn = null;
            bool sortByAscending = true;
            (sortColumn, sortByAscending) = ParseSortGameState(sortGame);
            var games = await gameRepository.GetGamesForAdminPanel(pageNo - 1, pageSize, sortColumn, sortByAscending);
            var gameModel = games.Select(Map).ToArray();
            var countGames = await gameRepository.TotalItems();

            return (gameModel, countGames);
        }

        public async Task<(IReadOnlyCollection<GameModel>, int)> GetGamesForAdminByCategoryAndNameAsync(int pageNo, int pageSize, SortGameState sortGame, string gameName, int categoryId)
        {
            IQueryable<GameDTO> games = gameRepository.GetAllGames();

            if (categoryId != 0)
            {
                games = games.Where(p => p.Category.Id == categoryId);
            }
            if (!String.IsNullOrEmpty(gameName))
            {
                games = games.Where(p => p.Name.Contains(gameName));
            }

            switch (sortGame)
            {
                case SortGameState.NameDesc:
                    games = games.OrderByDescending(p => p.Name);
                    break;
                case SortGameState.PublisherAsc:
                    games = games.OrderBy(p => p.Publisher);
                    break;
                case SortGameState.PublisherDesc:
                    games = games.OrderByDescending(p => p.Publisher);
                    break;
                case SortGameState.DateOfAddingAsc:
                    games = games.OrderBy(p => p.DateOfAdding);
                    break;
                case SortGameState.DateOfAddingDesc:
                    games = games.OrderByDescending(p => p.DateOfAdding);
                    break;
                case SortGameState.PriceAsc:
                    games = games.OrderBy(p => p.Price);
                    break;
                case SortGameState.PriceDesc:
                    games = games.OrderByDescending(p => p.Price);
                    break;
                default:
                    games = games.OrderBy(p => p.Name);
                    break;
            }

            var count = await games.CountAsync();
            var gamesFinal = await games.Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
            var gamesModel = gamesFinal.Select(Game.Mapper.Map).Select(Map).ToArray();

            return (gamesModel, count);

        }

        private (string, bool) ParseSortGameState(SortGameState sortGame)
        {
            string sortColumn = null;
            bool sortByAscending = true;
            switch (sortGame)
            {
                case SortGameState.NameAsc:
                    sortColumn = nameof(Game.Name); sortByAscending = true;
                    break;
                case SortGameState.NameDesc:
                    sortColumn = nameof(Game.Name); sortByAscending = false;
                    break;
                case SortGameState.PublisherAsc:
                    sortColumn = nameof(Game.Publisher); sortByAscending = true;
                    break;
                case SortGameState.PublisherDesc:
                    sortColumn = nameof(Game.Publisher); sortByAscending = false;
                    break;
                case SortGameState.PriceAsc:
                    sortColumn = nameof(Game.Price); sortByAscending = true;
                    break;
                case SortGameState.PriceDesc:
                    sortColumn = nameof(Game.Price); sortByAscending = false;
                    break;
                case SortGameState.DateOfAddingAsc:
                    sortColumn = nameof(Game.DateOfAdding); sortByAscending = true;
                    break;
                case SortGameState.DateOfAddingDesc:
                    sortColumn = nameof(Game.Price); sortByAscending = false;
                    break;
            }

            return (sortColumn, sortByAscending); 
        }

        public async Task AddNewGame(GameModel gameNew)
        {
            var game = CreateGameDTO(gameNew);
            await gameRepository.AddGame(Game.Mapper.Map(game));
                
        }

        public async Task UpdateGame(GameModel gameNew)
        {
            var game = CreateGameDTO(gameNew);
            game.Id = gameNew.GameId;
            await gameRepository.UpdateGame(Game.Mapper.Map(game));
            Session.RemoveCart();
        }

        public async Task RemoveGame(int gameId)
        {
            var game = await gameRepository.GetGameByIdAsync(gameId, false);
            await gameRepository.RemoveGame(game);
            Session.RemoveCart();
        }

        private GameModel Map(Game game)
        {
            return new GameModel
            {
                GameId = game.Id,
                Publisher = game.Publisher,
                Category = game.Category.Name,
                CategoryId = game.Category.Id,
                Name = game.Name,
                ShortDescription = game.ShortDescription,
                Description = game.Description,
                Price = game.Price,
                ImageData = game.ImageData,
                ReleaseDate = game.ReleaseDate,
                DateOfAdding = game.DateOfAdding,
                OnSale = game.OnSale
            };
        }
        private GameDTO CreateGameDTO(GameModel gameModel)
        {
            
            if (gameModel.DateOfAdding == DateTime.MinValue) gameModel.DateOfAdding = DateTime.UtcNow;
            return Game.DTOFactory.Create(gameModel.Name,
                                          gameModel.Publisher,
                                          gameModel.ShortDescription,
                                          gameModel.Description,
                                          gameModel.Price,
                                          gameModel.ImageData,
                                          gameModel.ReleaseDate,
                                          gameModel.DateOfAdding,
                                          gameModel.OnSale,
                                          gameModel.CategoryId ?? 1);
        }
    }
}
