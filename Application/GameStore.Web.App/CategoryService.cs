using GameStore.DataEF;
using GameStore.EntityInterfaces;
using GameStore.Web.App.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.Web.App
{
    public class CategoryService
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public async Task<CategoryModel> GetByIdAsync(int id)
        {
            var category = await categoryRepository.GetCategoryByIdAsync(id);
            return Map(category);
        }

        public async Task<CategoryModel> GetByNameAsync(string name)
        {
            var category = await categoryRepository.GetCategoryByNameAsync(name);
            return Map(category);
        }

        public async Task<IReadOnlyCollection<CategoryModel>> GetAllAsync()
        {
            var categories = await categoryRepository.GetAllCategoriesAsync();
            return categories.Select(Map).ToArray();
        }

        public async Task CreateCategory(CategoryModel categoryModel)
        {
            var categoryDto = CreateCategoryDTO(categoryModel);
            await categoryRepository.AddCategory(Category.Mapper.Map(categoryDto));
        }

        public async Task DeleteCategory(int categoryId)
        {
            var category = await categoryRepository.GetCategoryByIdAsync(categoryId);
            await categoryRepository.RemoveCategory(category);
        }

        private CategoryModel Map(Category category)
        {
            return new CategoryModel
            {
                CategoryId = category.Id,
                Name = category.Name,
                CategoryUrlSlug = category.UrlSlug
            };
        }
        private CategoryDTO CreateCategoryDTO(CategoryModel category)
        {
            return Category.DtoFactory.Create(category.Name, category.CategoryUrlSlug);
        }
    }
}
