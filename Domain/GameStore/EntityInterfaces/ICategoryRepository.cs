using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.EntityInterfaces
{
    public interface ICategoryRepository
    {
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> GetCategoryByNameAsync(string name);
        Task<Category[]> GetAllCategoriesAsync();
    }
}
