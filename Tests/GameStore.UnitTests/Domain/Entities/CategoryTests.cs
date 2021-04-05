using Bogus;
using FluentAssertions;
using GameStore.DataEF;
using NUnit.Framework;
using System;
using System.Linq;
using static GameStore.Category;

namespace GameStore.UnitTests.Domain.Entities
{
    [TestFixture]
    public class CategoryTests : BaseTest
    {
        [Test]
        public void Category_With_Valid_Arguments_Is_Created()
        {
            //Arrang
            var name = Faker.Random.String2(3, 40, russianLettersAndNumbers);
            var urlSlug = Faker.Random.String2(3, 30, russianLettersAndNumbers);

            Func<CategoryDTO> createCategoryDTO = () => DtoFactory.Create(name, urlSlug);

            //Act
            createCategoryDTO.Should().NotThrow();
            var categoryDto = createCategoryDTO();
            categoryDto.Games = collectionGameDto;
            var category = Mapper.Map(categoryDto);
            var gameList = category.CategoryGames.ToList();

            //Assert
            Assert.Multiple(() =>
            {
                category.Name.Should().Be(name);
                category.UrlSlug.Should().Be(urlSlug);
                category.CategoryGames.Count.Should().Be(collectionGameDto.Count);
                gameList[0].Price.Should().Be(collectionGameDto[0].Price);
                gameList[1].Price.Should().Be(collectionGameDto[1].Price);
            });
        }


        [Test]
        public void Category_With_InValid_Arguments_Throw_Exception()
        {
            var name = Faker.Random.String2(3, 40, russianLettersAndNumbers);
            var urlSlug = Faker.Random.String2(3, 30, russianLettersAndNumbers);

            //name.Length < 3 || name.Length > 40
            var nameLessThenMinItem = Faker.Random.String2(1, 2, russianLettersAndNumbers);
            var nameGreaterThenMaxItem = Faker.Random.String2(41, 1000, russianLettersAndNumbers);

            //urlSlug.Length < 3 || urlSlug.Length > 30
            var urlSlugLessThenMinItem = Faker.Random.String2(1, 2, russianLettersAndNumbers);
            var urlSlugGreaterThenMaxItem = Faker.Random.String2(31, 1000, russianLettersAndNumbers);

            Func<CategoryDTO> createCategoryDtoWithNullName = () => DtoFactory.Create(null, urlSlug);
            Func<CategoryDTO> createCategoryDtoWithNameLessThenMin = () => DtoFactory.Create(nameLessThenMinItem, urlSlug);
            Func<CategoryDTO> createCategoryDtoWithNameGreaterThenMax = () => DtoFactory.Create(nameGreaterThenMaxItem, urlSlug);
            Func<CategoryDTO> createCategoryDtoWithNullUrlSlug = () => DtoFactory.Create(name, null);
            Func<CategoryDTO> createCategoryDtoWithUrlSlugLessThenMin = () => DtoFactory.Create(name, urlSlugLessThenMinItem);
            Func<CategoryDTO> createCategoryDtoWithUrlSlugGreaterThenMax = () => DtoFactory.Create(name, urlSlugGreaterThenMaxItem);

            Assert.Multiple(() =>
            {
                createCategoryDtoWithNullName.Should().Throw<ArgumentException>();
                createCategoryDtoWithNameLessThenMin.Should().Throw<ArgumentOutOfRangeException>();
                createCategoryDtoWithNameGreaterThenMax.Should().Throw<ArgumentOutOfRangeException>();
                createCategoryDtoWithNullUrlSlug.Should().Throw<ArgumentException>();
                createCategoryDtoWithUrlSlugLessThenMin.Should().Throw<ArgumentOutOfRangeException>();
                createCategoryDtoWithUrlSlugGreaterThenMax.Should().Throw<ArgumentOutOfRangeException>();
            });

        }


    }
}
