using GameStore.EntityInterfaces;
using GameStore.Web.App.Interfaces;
using GameStore.Web.App.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.Web.App
{
    public class CategoryService: AbstractCategoryService
    {
        public CategoryService(ICategoryRepository categoryRepository)
        {
           this.categoryRepository = categoryRepository;
        }

        public override async Task<CategoryModel> GetByIdAsync(int id)
        {
            var category = await categoryRepository.GetCategoryByIdAsync(id);
            return Map(category);
        }

        public override async Task<CategoryModel> GetByNameAsync(string name)
        {
            var category = await categoryRepository.GetCategoryByNameAsync(name);
            return Map(category);
        }

        public override async Task<IReadOnlyCollection<CategoryModel>> GetAllAsync()
        {
            var categories = await categoryRepository.GetAllCategoriesAsync();
            return categories.Select(Map).ToArray();
        }

        public override async Task AddNewCategory(CategoryModel categoryModel)
        {
            var category = CreateCategory(categoryModel);
            await categoryRepository.AddCategory(category);
        }

        public override async Task DeleteCategory(int categoryId)
        {
            var category = await categoryRepository.GetCategoryByIdAsync(categoryId);
            await categoryRepository.RemoveCategory(category);
        }

      
    }
}
