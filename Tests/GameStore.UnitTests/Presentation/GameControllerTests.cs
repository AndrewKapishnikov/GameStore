using FluentAssertions;
using GameStore.Web.App;
using GameStore.Web.App.Interfaces;
using GameStore.Web.Controllers;
using GameStore.Web.Models;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;


namespace GameStore.UnitTests.Presentation
{
    [TestFixture]
    public class GameControllerTests: BaseTest
    {
        [Test]
        public void SearchGame_ByNameOrPublisher_Returns_CollectionGameModels()
        {
            string searchName = "SearchName";
            int countReturn = 2;
            var games = collectionGameDto.Select(Game.Mapper.Map).ToList();
            var gamesModel = games.Select(GameService.Map).Take(countReturn).ToList();
            var getGamesService = Substitute.For<IGetGamesService>();
                getGamesService.GetAllGamesByNameOrPublisherAsync(searchName).Returns(gamesModel);
            var controller = new GameController(getGamesService);

            // Act
            var task = controller.SearchGame(searchName);

            //Assert
            IReadOnlyCollection<GameModel> viewModel = task.GetFromTaskViewResult();
            Assert.Multiple(() =>
            {
                getGamesService.Received(1).GetAllGamesByNameOrPublisherAsync(searchName);
                viewModel.Count.Should().Be(countReturn);
            });
        }

        [Test]
        public void Index_Returns_GameModels()
        {
            int id = 1;
            var games = collectionGameDto.Select(Game.Mapper.Map).ToList();
            var gameModel = GameService.Map(games.First());
            var getGamesService = Substitute.For<IGetGamesService>();
                getGamesService.GetGameByIdAsync(id).Returns(gameModel);
            var controller = new GameController(getGamesService);

            // Act
            var task = controller.Index(id);

            GameModel viewModel = task.GetFromTaskViewResult();
            Assert.Multiple(() =>
            {
                getGamesService.Received(1).GetGameByIdAsync(id);
                viewModel.Name.Should().Be(gameModel.Name);
            });
        }

        [Test]
        public void CategoryGames_Returns_CategoryGamesViewModel()
        {
            string fakeCategoryUrl = "SomeCategory";
            int page = 200; int pageSize = 7;
            var games = collectionGameDto.Select(Game.Mapper.Map).ToList();
            var gamesModel = games.Select(GameService.Map).ToList();
            var getGamesService = Substitute.For<IGetGamesService>();
                getGamesService.GetAllGamesByCategoryAsync(fakeCategoryUrl, 100,pageSize).Returns((gamesModel, gamesModel.Count));
            var controller = new GameController(getGamesService);

            // Act
            var task = controller.CategoryGames(fakeCategoryUrl, page);

            CategoryGamesViewModel viewModel = task.GetFromTaskViewResult();
            Assert.Multiple(() =>
            {
                getGamesService.Received(1).GetAllGamesByCategoryAsync(fakeCategoryUrl, 100, pageSize);
                viewModel.Games.Count.Should().Be(gamesModel.Count);
            });

        }

    }
}
