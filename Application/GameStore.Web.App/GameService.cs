using GameStore.Web.App.Interfaces;
using GameStore.Web.App.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.Web.App
{
    public class GameService: IGetGamesService, IChangeGameService
    {
        private readonly IGetGamesRepositoryAsync gameRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        protected ISession Session => httpContextAccessor.HttpContext.Session;

        public GameService(IGetGamesRepositoryAsync gameRepository,
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

        public async Task<(IReadOnlyCollection<GameModel>, int)> GetAllGamesByCategoryAsync(string categoryUrlSlug, int pageNumber, int pageSize)
        {
            var gameArray = await gameRepository.GetAllByCategoryAsync(categoryUrlSlug);
            var count = gameArray.Count();
            var games = gameArray.Select(Map);
            var gamesList = games.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return (gamesList, count);
        }

        public async Task<IReadOnlyCollection<GameModel>> GetGamesByDescedingOrderAsync()
        {
            var game = await gameRepository.GetLastEightGameByDataAddingAsync();

            return game.Select(Map).ToArray();
        }

        public async Task<(IReadOnlyCollection<GameModel>, int)> GetGamesForAdminByPageAsync(int pageNo, int pageSize, SortGameStates sortGame)
        {
            var (sortColumn, sortByAscending) = ParseSortGameState(sortGame);
            var games = await gameRepository.GetGamesForAdminPanel(pageNo - 1, pageSize, sortColumn, sortByAscending);
            var gameModel = games.Select(Map).ToArray();
            var countGames = await gameRepository.TotalItemsAsync();
            return (gameModel, countGames);
        }

        public async Task<(IReadOnlyCollection<GameModel>, int)> GetGamesForAdminByCategoryAndNameAsync(
                                                                 int pageNo, int pageSize, SortGameStates sortGame, string gameName, int categoryId)
        {
            var games = gameRepository.GetAllGames();

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
                case SortGameStates.NameDesc:
                    games = games.OrderByDescending(p => p.Name);
                    break;
                case SortGameStates.PublisherAsc:
                    games = games.OrderBy(p => p.Publisher);
                    break;
                case SortGameStates.PublisherDesc:
                    games = games.OrderByDescending(p => p.Publisher);
                    break;
                case SortGameStates.DateOfAddingAsc:
                    games = games.OrderBy(p => p.DateOfAdding);
                    break;
                case SortGameStates.DateOfAddingDesc:
                    games = games.OrderByDescending(p => p.DateOfAdding);
                    break;
                case SortGameStates.PriceAsc:
                    games = games.OrderBy(p => p.Price);
                    break;
                case SortGameStates.PriceDesc:
                    games = games.OrderByDescending(p => p.Price);
                    break;
                default:
                    games = games.OrderBy(p => p.Name);
                    break;
            }

            var count = games.Count();
            var gamesFinal = games.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();
            var gamesModel = gamesFinal.Select(Game.Mapper.Map).Select(Map).ToArray();

            return await Task.FromResult((gamesModel, count));
          
        }

        private (string, bool) ParseSortGameState(SortGameStates sortGame)
        {
            string sortColumn = null;
            bool sortByAscending = true;
            switch (sortGame)
            {
                case SortGameStates.NameAsc:
                    sortColumn = nameof(Game.Name); sortByAscending = true;
                    break;
                case SortGameStates.NameDesc:
                    sortColumn = nameof(Game.Name); sortByAscending = false;
                    break;
                case SortGameStates.PublisherAsc:
                    sortColumn = nameof(Game.GameDescription.Publisher); sortByAscending = true;
                    break;
                case SortGameStates.PublisherDesc:
                    sortColumn = nameof(Game.GameDescription.Publisher); sortByAscending = false;
                    break;
                case SortGameStates.PriceAsc:
                    sortColumn = nameof(Game.Price); sortByAscending = true;
                    break;
                case SortGameStates.PriceDesc:
                    sortColumn = nameof(Game.Price); sortByAscending = false;
                    break;
                case SortGameStates.DateOfAddingAsc:
                    sortColumn = nameof(Game.DateOfAdding); sortByAscending = true;
                    break;
                case SortGameStates.DateOfAddingDesc:
                    sortColumn = nameof(Game.Price); sortByAscending = false;
                    break;
            }

            return (sortColumn, sortByAscending); 
        }

        public async Task AddNewGame(GameModel gameNew)
        {
            var game = CreateGame(gameNew);
            await gameRepository.AddGameAsync(game);
                
        }

        public async Task UpdateGame(GameModel gameNew)
        {
            var game = CreateGame(gameNew);
            game.Id = gameNew.GameId;
            await gameRepository.UpdateGameAsync(game);
            Session.RemoveCart();
        }

        public async Task RemoveGameByGameId(int gameId)
        {
            var game = await gameRepository.GetGameByIdAsync(gameId, false);
            await gameRepository.RemoveGameAsync(game);
            Session.RemoveCart();
        }

        public static GameModel Map(Game game)
        {
            return new GameModel
            {
                GameId = game.Id,
                Category = game.Category?.Name,
                CategoryId = game.Category?.Id,
                Name = game.Name,
                Price = game.Price,
                ImageData = game.ImageData,
                DateOfAdding = game.DateOfAdding,
                OnSale = game.OnSale,
                Publisher = game.GameDescription.Publisher,
                ShortDescription = game.GameDescription.ShortDescription,
                Description = game.GameDescription.Description,
                ReleaseDate = game.GameDescription.ReleaseDate,
            };
        }
        public static Game CreateGame(GameModel gameModel)
        {
            
            if (gameModel.DateOfAdding == DateTime.MinValue) gameModel.DateOfAdding = DateTime.UtcNow;
            var gameDescription = GameDescription.Create(gameModel.Publisher, gameModel.ShortDescription,
                                                         gameModel.Description, gameModel.ReleaseDate);
            return Game.Mapper.Map(Game.DTOFactory.Create(
                                        gameModel.Name,
                                        gameModel.Price,
                                        gameModel.ImageData,
                                        gameModel.DateOfAdding,
                                        gameModel.OnSale,
                                        gameModel.CategoryId ?? 1,
                                        gameDescription));
        }
    }
}
