using System.Threading.Tasks;

namespace GameStore.EntityInterfaces
{
    public interface ICategoryRepository
    {
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> GetCategoryByNameAsync(string name);
        Task<Category[]> GetAllCategoriesAsync();
        Task AddCategory(Category category);
        Task RemoveCategory(Category category);
    }
}
