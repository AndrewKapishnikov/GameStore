using GameStore.EntityInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Data.EF.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ContextDBFactory dbContextFactory;
        public CategoryRepository(ContextDBFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            var db = dbContextFactory.Create(typeof(CategoryRepository));
            var categoryDto = await db.Categories.SingleAsync(p => p.Id == id);
            return Category.Mapper.Map(categoryDto);
        }

        public async Task<Category> GetCategoryByNameAsync(string name)
        {
            var db = dbContextFactory.Create(typeof(CategoryRepository));
            var categoryDto = await db.Categories.SingleAsync(p => p.Name == name);
            return Category.Mapper.Map(categoryDto);
        }

        public async Task<Category[]> GetAllCategoriesAsync()
        {
            var db = dbContextFactory.Create(typeof(CategoryRepository));
            var categoriesDto = await db.Categories.ToArrayAsync();
            return categoriesDto.Select(Category.Mapper.Map).ToArray();
        }



    }
}
