using Bogus;
using FluentAssertions;
using GameStore.DataEF;
using NUnit.Framework;
using System;
using System.Linq;
using static GameStore.Game;


namespace GameStore.UnitTests.Domain.Entities
{
    [TestFixture]
    public class GameTests: BaseTest
    {
        [Test]
        public void Game_With_Valid_Arguments_Is_Created()
        {
            //Arrange
            var name = Faker.Random.String2(20, russianLettersAndNumbers);
            var price = Faker.Random.Decimal(0, 100000m);
            var imageData = Faker.Random.Bytes(500);
            var dateOfAdding = Faker.Date.Between(new DateTime(2021, 1, 01), new DateTime(2100, 1, 01));
            var categoryId = Faker.Random.Int(1, int.MaxValue);
            var gameDescription = GameDescriptionFakeData.Valid.Generate();

            Func<GameDTO> createGameDTO = () => DTOFactory.Create(
                name, price, imageData, dateOfAdding, true, categoryId, gameDescription);
            
            
            createGameDTO.Should().NotThrow();

            //Act
            var gameDto = createGameDTO();
            gameDto.OrderItems = collectionItemDto;
            var game = Mapper.Map(gameDto);
            var gameList = game.GameOrders.ToList();

            //Assert
            Assert.Multiple(() =>
            {
                game.Name.Should().Be(name);
                game.OnSale.Should().Be(true);
                game.ImageData.Should().BeSameAs(imageData);
                game.Price.Should().Be(price);
                game.DateOfAdding.Should().Be(dateOfAdding);
                game.GameOrders.Count.Should().Be(collectionItemDto.Count);
                gameList[0].Count.Should().Be(collectionItemDto[0].Count);
                gameList[1].Count.Should().Be(collectionItemDto[1].Count);
            });
        }

        [Test]
        public void Game_With_Invalid_Arguments_Throw_Exception()
        {
            //Arrange
            var name = Faker.Random.String(20);
            var price = Faker.Random.Decimal(0, 100000m);
            var imageData = Faker.Random.Bytes(500);
            var dateOfAdding = Faker.Date.Between(new DateTime(2021, 1, 01), new DateTime(2100, 1, 01));
            var categoryId = Faker.Random.Int(1, int.MaxValue);
            var gameDescription = GameDescriptionFakeData.Valid.Generate();

            var dateOfAddingLessThenMinimumItem = Faker.Date.Between(new DateTime(1000, 1, 01), new DateTime(2021, 1, 01));
            var dateOfAddingGreaterThenMaximumItem = Faker.Date.Between(new DateTime(2100, 1, 01), new DateTime(9999, 1, 01));
            var priceLessThenZero = Faker.Random.Decimal(Decimal.MinValue, Decimal.Zero);
            var priceGreateThenMaximumItem = Faker.Random.Decimal(100001m, Decimal.MaxValue);

            // Act
            Func<GameDTO> createGameDtoWithNullName = () => DTOFactory.Create(
                 null, price, imageData, dateOfAdding, true, categoryId, gameDescription);
            Func<GameDTO> createGameDtoWithPriceLessThenZero = () => DTOFactory.Create(
                 name, priceLessThenZero, imageData, dateOfAdding, true, categoryId, gameDescription);
            Func<GameDTO> createGameDtoWithPriceGreateThenMaximumItem = () => DTOFactory.Create(
                 name, priceGreateThenMaximumItem, imageData, dateOfAdding, true, categoryId, gameDescription);
            Func<GameDTO> createGameDtoWithNullImageData = () => DTOFactory.Create(
                 name, price, null, dateOfAdding, true, categoryId, gameDescription);
            Func<GameDTO> createGameDtoWithNullGameDescription = () => DTOFactory.Create(
                 name, price, imageData, dateOfAdding, true, categoryId, null);
            Func<GameDTO> createGameDtoWithDateOfAddingLessThenMinimumItem = () => DTOFactory.Create(
                 name, price, imageData, dateOfAddingLessThenMinimumItem, true, categoryId, gameDescription);
            Func<GameDTO> createGameDtoWithDateOfAddingGreateThenMaximumItem = () => DTOFactory.Create(
                 name, price, imageData, dateOfAddingGreaterThenMaximumItem, true, categoryId, gameDescription);

            //Assert
            Assert.Multiple(() =>
            {
                createGameDtoWithNullName.Should().Throw<ArgumentException>();
                createGameDtoWithPriceLessThenZero.Should().Throw<ArgumentOutOfRangeException>();
                createGameDtoWithPriceGreateThenMaximumItem.Should().Throw<ArgumentOutOfRangeException>();
                createGameDtoWithNullImageData.Should().Throw<ArgumentException>();
                createGameDtoWithNullGameDescription.Should().Throw<ArgumentException>();
                createGameDtoWithDateOfAddingLessThenMinimumItem.Should().Throw<ArgumentOutOfRangeException>();
                createGameDtoWithDateOfAddingGreateThenMaximumItem.Should().Throw<ArgumentOutOfRangeException>();
                
            });

        }


    }
}
