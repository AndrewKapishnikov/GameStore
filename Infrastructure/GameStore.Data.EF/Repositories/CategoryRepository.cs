using GameStore.EntityInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.Data.EF.Repositories
{
    public class CategoryRepository : ICategoryRepositoryAsync
    {
        private readonly ContextDBFactory dbContextFactory;
        public CategoryRepository(ContextDBFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }
        private GameStoreDbContext GetDbContextForCategoryRepository()
        {
            return dbContextFactory.Create(typeof(CategoryRepository));
        }
        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            var db = GetDbContextForCategoryRepository();
            var categoryDto = await db.Categories.SingleAsync(p => p.Id == id);
            return Category.Mapper.Map(categoryDto);
        }

        public async Task<Category> GetCategoryByNameAsync(string name)
        {
            var db = GetDbContextForCategoryRepository();
            var categoryDto = await db.Categories.SingleAsync(p => p.Name == name);
            return Category.Mapper.Map(categoryDto);
        }

        public async Task<Category[]> GetAllCategoriesAsync()
        {
            var db = GetDbContextForCategoryRepository();
            var categoriesDto = await db.Categories.ToArrayAsync();
            return categoriesDto.Select(Category.Mapper.Map).ToArray();
        }

        public async Task AddCategoryAsync(Category category)
        {
            var db = GetDbContextForCategoryRepository();
            await db.Categories.AddAsync(Category.Mapper.Map(category));
            await db.SaveChangesAsync();
        }

        public async Task RemoveCategoryAsync(Category category)
        {
            var db = GetDbContextForCategoryRepository();
            var categoryDto = Category.Mapper.Map(category);
            db.Categories.Remove(categoryDto);
            await db.SaveChangesAsync();
        }

    }
}
