﻿using GameStore.EntityInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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

        public async Task AddCategory(Category category)
        {
            var db = dbContextFactory.Create(typeof(CategoryRepository));
            await db.Categories.AddAsync(Category.Mapper.Map(category));
            await db.SaveChangesAsync();
        }

        public async Task RemoveCategory(Category category)
        {
            var db = dbContextFactory.Create(typeof(CategoryRepository));
            var categoryDto = Category.Mapper.Map(category);
            db.Categories.Remove(categoryDto);
            await db.SaveChangesAsync();
        }

    }
}
