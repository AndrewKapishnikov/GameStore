using FluentAssertions;
using GameStore.EntityInterfaces;
using GameStore.Web.App;
using GameStore.Web.App.Models;
using NSubstitute;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.UnitTests.Application
{
    [TestFixture]
    public class CategoryServiceTests: BaseTest
    {
        [Test]
        public void GetByIdAsync_Pass_CategoryId()
        {
            //Arrange
            var category = Category.Mapper.Map(categoryDto.First());
            var categoryRepository = Substitute.For<ICategoryRepository>();
                categoryRepository.GetCategoryByIdAsync(category.Id).Returns(category);
            var categoryService = new CategoryService(categoryRepository);

            //Act
            var taskCategoryModel = categoryService.GetByIdAsync(category.Id);

            //Assert
            Assert.Multiple(() =>
            {
                var categoryModel = taskCategoryModel.Result;
                categoryModel.Name.Should().Be(category.Name);
                categoryModel.CategoryId.Should().Be(category.Id);
                categoryRepository.Received(1).GetCategoryByIdAsync(category.Id);
            });
        }

        [Test]
        public void GetByNameAsync_Pass_CategoryName()
        {
            var category = Category.Mapper.Map(categoryDto.First());
            var categoryRepository = Substitute.For<ICategoryRepository>();
                categoryRepository.GetCategoryByNameAsync(category.Name).Returns(category);
            var categoryService = new CategoryService(categoryRepository);

            //Act
            var taskCategoryModel = categoryService.GetByNameAsync(category.Name);

            //Assert
            Assert.Multiple(() =>
            {
                var categoryModel = taskCategoryModel.Result;
                categoryModel.Name.Should().Be(category.Name);
                categoryModel.CategoryId.Should().Be(category.Id);
                categoryModel.CategoryUrlSlug.Should().Be(category.UrlSlug);
                categoryRepository.Received(1).GetCategoryByNameAsync(category.Name);
            });
        }

        [Test]
        public void GetAllAsyncTest()
        {
            var categories = categoryDto.Select(Category.Mapper.Map).ToArray();
            var categoryRepository = Substitute.For<ICategoryRepository>();
                categoryRepository.GetAllCategoriesAsync().Returns(categories);
            var categoryService = new CategoryService(categoryRepository);

            var taskCategoryModel = categoryService.GetAllAsync();

            Assert.Multiple(() =>
            {
                var categoryModelCollect = taskCategoryModel.Result.ToArray();
                categoryModelCollect.Length.Should().Be(categories.Length);
                categoryModelCollect[0].Name.Should().Be(categories[0].Name);
                categoryRepository.Received(1).GetAllCategoriesAsync();
            });
        }

        [Test]
        public void AddNewCategory_Pass_CategoryModel()
        {
            var categoryModel = CreateFakeCategoryModel();
            var category = CategoryService.CreateCategory(categoryModel);
            var categoryRepository = Substitute.For<ICategoryRepository>();
                categoryRepository.AddCategory(category);
            var categoryService = new CategoryService(categoryRepository);

            var task = categoryService.AddNewCategory(categoryModel);

            Assert.Multiple(() =>
            {
               categoryRepository.Received(1).AddCategory(category);
               Assert.IsTrue(task.IsCompletedSuccessfully);
            });
        }

        [Test]
        public void DeleteCategory_Pass_CategoryId()
        {
            var categoryModel = CreateFakeCategoryModel();
            var category = CategoryService.CreateCategory(categoryModel);
            var categoryRepository = Substitute.For<ICategoryRepository>();
                categoryRepository.GetCategoryByIdAsync(categoryModel.CategoryId).Returns(category);
                categoryRepository.RemoveCategory(category).Returns(Task.CompletedTask);
            var categoryService = new CategoryService(categoryRepository);

            var task = categoryService.DeleteCategory(categoryModel.CategoryId);

            Assert.Multiple(() =>
            {
                categoryRepository.Received(1).GetCategoryByIdAsync(categoryModel.CategoryId);
                categoryRepository.Received(1).RemoveCategory(category);
                Assert.IsTrue(task.IsCompletedSuccessfully);
            });
        }


        private CategoryModel CreateFakeCategoryModel()
        {
            return new CategoryModel()
            {
                CategoryId = 1,
                Name = Faker.Random.String2(3, 40, russianLettersAndNumbers),
                CategoryUrlSlug = Faker.Random.String2(3, 30, russianLettersAndNumbers)
            };
        }

    }
}
