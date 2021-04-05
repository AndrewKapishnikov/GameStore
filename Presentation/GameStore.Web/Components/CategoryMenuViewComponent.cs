using GameStore.Web.App;
using GameStore.Web.App.Interfaces;
using GameStore.Web.App.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.Web.Components
{
    public class CategoryMenuViewComponent: ViewComponent
    {
        private readonly AbstractCategoryService category;
        public CategoryMenuViewComponent(AbstractCategoryService categoryService)
        {
            category = categoryService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            IReadOnlyCollection<CategoryModel> categories = await category.GetAllAsync();
            return View(categories);
        }
    }
}
