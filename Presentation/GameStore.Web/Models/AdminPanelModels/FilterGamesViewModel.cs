using GameStore.Web.App.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Web.Models.AdminPanelModels
{
    public class PagePair
    {
        public int Number { get; set; }
        public string NumberName { get; set; }
    }
    public class FilterGamesViewModel
    {
        private List<PagePair> pagePair = new List<PagePair>()
        {
            new PagePair() { Number = 5, NumberName = "5"},
            new PagePair() { Number = 10, NumberName = "10"},
            new PagePair() { Number = 20, NumberName = "20"},
            new PagePair() { Number = 50, NumberName = "50"},
            new PagePair() { Number = 100, NumberName = "100"}
        };
        public FilterGamesViewModel(List<CategoryModel> categories, int? categoryId, int pageCount, string nameGame)
        {
            categories.Insert(0, new CategoryModel { Name = "Все", CategoryId = 0, CategoryUrlSlug = "all" });
            Categories = new SelectList(categories, "CategoryId", "Name", categoryId);
            SelectedCategory = categoryId;
            SelectedGameName = nameGame;
            CountPages = new SelectList(pagePair, "Number", "NumberName", pageCount);
            SelectedPageSize = pageCount;

        }
        public SelectList CountPages { get; private set; }
        public int SelectedPageSize { get; private set; }
        public SelectList Categories { get; private set; } 
        public int? SelectedCategory { get; private set; }   
        public string SelectedGameName { get; private set; }  
    }
}
