using FluentAssertions;
using GameStore.Web.App;
using GameStore.Web.App.Models;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;


namespace GameStore.UnitTests.Application
{
    [TestFixture]
    public class GameServiceTests: BaseTest
    {
        private readonly HttpContextAccessor contextAccessor = new HttpContextAccessor();

        [Test]
        public void GetGameByIdAsyncTest_Pass_GameId()
        {
            //Arrange
            var game = Game.Mapper.Map(collectionGameDto.First());
            var gameRepository = Substitute.For<IGetGamesRepositoryAsync>();
                gameRepository.GetGameByIdAsync(game.Id, true)
                              .Returns(game);
            var gameService = new GameService(gameRepository, contextAccessor);
            // Act

            var taskGameModel = gameService.GetGameByIdAsync(game.Id);

            // Assert
            Assert.Multiple(() =>
            {
                var gameModel = taskGameModel.Result;
                gameModel.GameId.Should().Be(game.Id);
                gameModel.Price.Should().Be(game.Price);
                gameModel.Name.Should().Be(game.Name);
                gameRepository.Received(1).GetGameByIdAsync(game.Id, true);
            });
        }

        [Test]
        public void GetAllGamesByNameOrPublisherAsyn_Pass_ValidQuery()
        {
            var games = collectionGameDto.Select(Game.Mapper.Map).ToArray<Game>();
            var query = Faker.Random.String2(3, 30);
            var gameRepository = Substitute.For<IGetGamesRepositoryAsync>();
                gameRepository.GetAllByNameOrPublisherAsync(query)
                              .Returns(games);
            var gameService = new GameService(gameRepository, contextAccessor);

            //Act
            var taskCollectionGameModel = gameService.GetAllGamesByNameOrPublisherAsync(query);

            Assert.Multiple(() =>
            {
                var gameModel = taskCollectionGameModel.Result;
                gameModel.Count.Should().Be(games.Length);
                gameRepository.Received(1).GetAllByNameOrPublisherAsync(query);
            });

        }

        [Test]
        public void GetAllGamesByNameOrPublisherAsyn_Pass_EmptyQuery()
        {
            var games = collectionGameDto.Select(Game.Mapper.Map).ToArray<Game>();
            var query = "";
            var gameRepository = Substitute.For<IGetGamesRepositoryAsync>();
            gameRepository.GetAllByNameOrPublisherAsync(query)
                          .Returns(games);
            var gameService = new GameService(gameRepository, contextAccessor);

            //Act
            var taskCollectionGameModel = gameService.GetAllGamesByNameOrPublisherAsync(query);

            Assert.Multiple(() =>
            {
                var gameModel = taskCollectionGameModel.Result;
                gameModel.Count.Should().Be(0);
                gameRepository.DidNotReceive().GetAllByNameOrPublisherAsync(query);
            });
        }

        [Test]
        public void GetAllGamesByCategoryAsync_Pass_CategoryUrlSlug_PageNumber_PageSize()
        {
            var games = collectionGameDto.Select(Game.Mapper.Map).ToArray<Game>();
            var pageNumber = 1;
            var pageSize = 2;
            var categoryUrlSlug = Faker.Random.String2(3, 30);
            var gameRepository = Substitute.For<IGetGamesRepositoryAsync>();
                gameRepository.GetAllByCategoryAsync(categoryUrlSlug)
                              .Returns(games);
            var gameService = new GameService(gameRepository, contextAccessor);

            var taskCollectionGameModel = gameService.GetAllGamesByCategoryAsync(categoryUrlSlug, pageNumber, pageSize);

            Assert.Multiple(() =>
            {
                var gameModel = taskCollectionGameModel.Result;
                gameModel.Item1.Count.Should().Be(pageSize);
                gameModel.Item2.Should().Be(games.Length);
                gameRepository.Received(1).GetAllByCategoryAsync(categoryUrlSlug);
            });
        }

        [Test]
        public void GetGamesByDescedingOrderAsyncTest()
        {
            var games = collectionGameDto.Select(Game.Mapper.Map).ToArray<Game>();
            var gameRepository = Substitute.For<IGetGamesRepositoryAsync>();
                gameRepository.GetLastEightGameByDataAddingAsync()
                              .Returns(games);
            var gameService = new GameService(gameRepository, contextAccessor);

            var taskCollectionGameModel = gameService.GetGamesByDescedingOrderAsync();

            Assert.Multiple(() =>
            {
                var gameModel = taskCollectionGameModel.Result;
                gameModel.Count.Should().Be(games.Length);
                gameRepository.Received(1).GetLastEightGameByDataAddingAsync();
            });
        }

        [Test]
        public void GetGamesForAdminByPageAsync_Pass_PageNumber_PageSize_SortGame()
        {
            //Arrange
            var games = collectionGameDto.Select(Game.Mapper.Map).ToArray<Game>();
            var pageNumber = 1;
            var pageSize = 2;
            var takeCountGames = 2;
            var sortColumn = SortGameStates.NameAsc;
            var sortColumnStr = nameof(Game.Name);
            var gameRepository = Substitute.For<IGetGamesRepositoryAsync>();
                gameRepository.GetGamesForAdminPanel(pageNumber - 1, pageSize, sortColumnStr, true)
                              .Returns(games.Take(takeCountGames).ToArray());
                gameRepository.TotalItemsAsync().Returns(games.Length);
            var gameService = new GameService(gameRepository, contextAccessor);

            //Act
            var taskCollectionGameModel = gameService.GetGamesForAdminByPageAsync(pageNumber, pageSize, sortColumn);

            //Assert
            Assert.Multiple(() =>
            {
                var gameModel = taskCollectionGameModel.Result;
                gameModel.Item1.Count.Should().Be(takeCountGames);
                gameModel.Item2.Should().Be(games.Length);
                gameRepository.Received(1).GetGamesForAdminPanel(pageNumber - 1, pageSize, sortColumnStr, true);
                gameRepository.Received(1).TotalItemsAsync();
            });
        }

        [Test]
        [TestCase(1, 2, SortGameStates.NameAsc, 0)]
        public void GetGamesForAdminByCategoryAndNameAsync_WithPassValue_GameName(
                          int pageNo, int pageSize, SortGameStates sortGame, int categoryId)
        {
            var iQuerableGamesCollection = collectionGameDto.AsQueryable();
            var gameName = iQuerableGamesCollection.First().Name;
            var gameRepository = Substitute.For<IGetGamesRepositoryAsync>();
                gameRepository.GetAllGames().Returns(iQuerableGamesCollection);
            var gameService = new GameService(gameRepository, contextAccessor);

            // Act
            var taskCollectionGameModel = gameService.GetGamesForAdminByCategoryAndNameAsync(
                                          pageNo, pageSize, sortGame, gameName, categoryId);
            //Assert
            Assert.Multiple(() =>
            {
                var gameModel = taskCollectionGameModel.Result;
                gameModel.Item1.Count.Should().Be(1);
                gameModel.Item1.First().Name.Should().Be(gameName);
                gameModel.Item2.Should().Be(1);
                gameRepository.Received(1).GetAllGames();
            });
        }

        [Test]
        [TestCase(1, 2, SortGameStates.NameAsc, 0)]
        public void GetGamesForAdminByCategoryAndNameAsync_WithPass_Empty_GameName(
                         int pageNo, int pageSize, SortGameStates sortGame, int categoryId)
        {
            var iQuerableGamesCollection = collectionGameDto.AsQueryable();
            var gameName = "";
            var gameRepository = Substitute.For<IGetGamesRepositoryAsync>();
            gameRepository.GetAllGames().Returns(iQuerableGamesCollection);
            var gameService = new GameService(gameRepository, contextAccessor);

            // Act
            var taskCollectionGameModel = gameService.GetGamesForAdminByCategoryAndNameAsync(
                                          pageNo, pageSize, sortGame, gameName, categoryId);
            //Assert
            Assert.Multiple(() =>
            {
                var gameModel = taskCollectionGameModel.Result;
                gameModel.Item1.Count.Should().Be(2);
                gameModel.Item2.Should().Be(iQuerableGamesCollection.Count());
                gameRepository.Received(1).GetAllGames();
            });
        }

        [Test]
        public void AddNewGame_Pass_GameModel()
        {
            var gameModel = CreateFakeGameModel();
            var game = GameService.CreateGame(gameModel);
            var gameRepository = Substitute.For<IGetGamesRepositoryAsync>();
                gameRepository.AddGameAsync(game);
            var gameService = new GameService(gameRepository, contextAccessor);

            var task = gameService.AddNewGame(gameModel);

            gameRepository.Received(1).AddGameAsync(game);
            Assert.IsTrue(task.IsCompletedSuccessfully);
        }


        [Test]
        public void UpdateGame_Pass_GameModel()
        {
            var gameModel = CreateFakeGameModel();
            var game = GameService.CreateGame(gameModel);
            var gameRepository = Substitute.For<IGetGamesRepositoryAsync>();
                gameRepository.UpdateGameAsync(game);
            var contextAccess = Substitute.For<IHttpContextAccessor>();
                contextAccess.HttpContext.Session.RemoveCart();
            var gameService = new GameService(gameRepository, contextAccess);

            var task = gameService.UpdateGame(gameModel);

            gameRepository.Received(1).UpdateGameAsync(game);
            Assert.IsTrue(task.IsCompletedSuccessfully);
        }


        [Test]
        public void RemoveGameByGameId_Pass_GameId()
        {
            var gameModel = CreateFakeGameModel();
            var game = GameService.CreateGame(gameModel);
                game.Id = gameModel.GameId;
            var gameRepository = Substitute.For<IGetGamesRepositoryAsync>();
                gameRepository.RemoveGameAsync(game);
                gameRepository.GetGameByIdAsync(game.Id, false).Returns(game);
            var contextAccess = Substitute.For<IHttpContextAccessor>();
                contextAccess.HttpContext.Session.RemoveCart();
            var gameService = new GameService(gameRepository, contextAccess);

            var task = gameService.RemoveGameByGameId(gameModel.GameId);

            //Assert
            Assert.Multiple(() =>
            {
                gameRepository.Received(1).GetGameByIdAsync(game.Id, false);
                Assert.IsTrue(task.IsCompletedSuccessfully);
            });
        }


        private GameModel CreateFakeGameModel()
        {
            return new GameModel()
            {
                GameId = 1,
                DateOfAdding = Faker.Date.Between(new DateTime(2021, 1, 01), new DateTime(2100, 1, 01)),
                Publisher = Faker.Company.CompanyName(),
                ShortDescription = Faker.Random.String2(20, 25, russianLettersAndNumbers),
                Description = Faker.Random.String2(20, 10000, russianLettersAndNumbers),
                ReleaseDate = Faker.Date.Between(new DateTime(1980, 1, 01), new DateTime(2099, 12, 31)),
                Price = Faker.Random.Decimal(0, 100000m),
                Name = Faker.Random.String2(5, 30, russianLettersAndNumbers),
                ImageData = Faker.Random.Bytes(400),
                OnSale = true
            };
        }

    }
}
