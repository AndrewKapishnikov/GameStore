using GameStore.Web.App;
using GameStore.Web.App.Interfaces;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System.Linq;
using GameStore.Web.Controllers;
using FluentAssertions;

namespace GameStore.UnitTests.Presentation
{
    [TestFixture]
    public class HomeControllerTests: BaseTest
    {
        [Test]
        public void IndexTest_Returns_CollectionGameModels()
        {
            //Arrange
            var games = collectionGameDto.Select(Game.Mapper.Map).ToList();
            var gamesModel = games.Select(GameService.Map).ToList();
            var gameService = Substitute.For<IGetGamesService>();
                //!!! Return values cannot be configured for non-virtual/non-abstract members.
                gameService.GetGamesByDescedingOrderAsync().Returns(gamesModel);
            var logger = Substitute.For<ILogger<HomeController>>();
            var controller = new HomeController(logger, gameService);
                      
            // Act
            var task = controller.Index();

            //Assert
            var indexViewModel = task.GetFromTaskViewResult();
            Assert.Multiple(() =>
            {
                gameService.Received(1).GetGamesByDescedingOrderAsync();
                indexViewModel.Count.Should().Be(games.Count);
            });
        }

    }
}
