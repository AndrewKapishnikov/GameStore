using GameStore.EntityInterfaces;
using GameStore.Web.App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.Web.App.Interfaces
{
    public abstract class AbstractCategoryService
    {
        protected ICategoryRepositoryAsync categoryRepository;

        public abstract Task<CategoryModel> GetByIdAsync(int id);
        public abstract Task<CategoryModel> GetByNameAsync(string name);
        public abstract Task<IReadOnlyCollection<CategoryModel>> GetAllAsync();
        public abstract Task AddNewCategory(CategoryModel categoryModel);
        public abstract Task DeleteCategory(int categoryId);
       
        public static CategoryModel Map(Category category)
        {
            return new CategoryModel
            {
                CategoryId = category.Id,
                Name = category.Name,
                CategoryUrlSlug = category.UrlSlug
            };
        }
        public static Category CreateCategory(CategoryModel category)
        {
            return Category.Mapper.Map(Category.DtoFactory.Create(category.Name, category.CategoryUrlSlug));
        }

    }
}
