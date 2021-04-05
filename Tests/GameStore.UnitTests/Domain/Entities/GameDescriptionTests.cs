using System;
using NUnit.Framework;
using Bogus;
using FluentAssertions;


namespace GameStore.UnitTests.Domain.Entities
{
    [TestFixture]
    public class GameDescriptionTests: BaseTest
    {

        [Test]
        public void GameDescription_With_Valid_Arguments_Is_Created()
        {
            // Arrange
            Func<GameDescription> createGameDescription = () => GameDescription.Create(
                publisher,
                shortDescription,
                description,
                releaseDate);

            //Act
            var gameDescription = createGameDescription();
            // Assert
            createGameDescription.Should().NotThrow();

            Assert.Multiple(() =>
            {
                gameDescription.Publisher.Should().Be(publisher);
                gameDescription.ShortDescription.Should().Be(shortDescription);
                gameDescription.Description.Should().Be(description);
                gameDescription.ReleaseDate.Should().Be(releaseDate);
            });
        }

        [Test]
        public void GameDescription_With_Invalid_Arguments_Throw_Exceptions()
        {
            // Arrange
            var minFailedshortDescription = Faker.Random.String2(10, russianLettersAndNumbers);
            var maxFailedshortDescription = Faker.Random.String2(200, russianLettersAndNumbers);
            var releaseDateLessThenMinimum = Faker.Date.Between(new DateTime(1000, 1, 01), new DateTime(1979, 12, 31));
            var releaseDateGreaterThenMaximum = Faker.Date.Between(new DateTime(2100, 1, 01), new DateTime(5000, 1, 01));

            // Act
            Action createWithNullPublisher = () => GameDescription.Create(
                null,
                shortDescription,
                description,
                releaseDate);
            Action createWithWhiteSpacePublisher = () => GameDescription.Create(
               "",
               shortDescription,
               description,
               releaseDate);
            Action createWithNullShortDescription = () => GameDescription.Create(
                publisher,
                null,
                description,
                releaseDate);
            Action createWithMinFailedShortDescription = () => GameDescription.Create(
                publisher,
                minFailedshortDescription,
                description,
                releaseDate);
            Action createWithMaxFailedShortDescription = () => GameDescription.Create(
                publisher,
                maxFailedshortDescription,
                description,
                releaseDate);
            Action createWithNullDescription = () => GameDescription.Create(
                publisher,
                shortDescription,
                null,
                releaseDate);
            Action createWithDateIsLessThenMinimumPoint = () => GameDescription.Create(
                publisher,
                shortDescription,
                description,
                releaseDateLessThenMinimum);
            Action createWithDateIsGreaterThenMaximumPoint = () => GameDescription.Create(
                publisher,
                shortDescription,
                description,
                releaseDateGreaterThenMaximum);

            // Assert
            Assert.Multiple(() =>
            {
                createWithNullPublisher.Should().Throw<ArgumentException>();
                createWithWhiteSpacePublisher.Should().Throw<ArgumentException>();
                createWithNullShortDescription.Should().Throw<ArgumentException>();
                createWithMinFailedShortDescription.Should().Throw<ArgumentOutOfRangeException>();
                createWithMaxFailedShortDescription.Should().Throw<ArgumentOutOfRangeException>();
                createWithNullDescription.Should().Throw<ArgumentException>();
                createWithDateIsLessThenMinimumPoint.Should().Throw<ArgumentOutOfRangeException>();
                createWithDateIsGreaterThenMaximumPoint.Should().Throw<ArgumentOutOfRangeException>();
            });
        }




    }
}
