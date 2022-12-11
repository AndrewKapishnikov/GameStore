using System.Threading.Tasks;

namespace GameStore.EntityInterfaces
{
    public interface ICategoryRepositoryAsync
    {
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> GetCategoryByNameAsync(string name);
        Task<Category[]> GetAllCategoriesAsync();
        Task AddCategoryAsync(Category category);
        Task RemoveCategoryAsync(Category category);
    }
}
