using GameStore.Web.App;
using GameStore.Web.Models;
using GameStore.Web.Models.AdminPanelModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.Web.Controllers
{
    public class GameController: Controller
    {
        private readonly GameService gameService;
        private readonly CategoryService categoryService;

        public GameController(GameService gameService, CategoryService categoryService)
        {
            this.gameService = gameService;
            this.categoryService = categoryService;
        }

        public async Task<IActionResult> SearchGame(string query)
        {
            var games = await gameService.GetAllGamesByNameOrPublisherAsync(query);
            
            return View(games);
        }

        public async Task<IActionResult> Index(int id)
        {
            var model = await gameService.GetGameByIdAsync(id);

            return View(model);
        }

        [Route("{controller}/{action}/{category}/{page?}")]
        public async Task<IActionResult> CategoryGames(string category, int? page)
        {
            int pageNumber = (page ?? 1);
            if (pageNumber > 100) pageNumber = 100;
            if (pageNumber < 1) pageNumber = 1;
            int pageSize = 7;

            var (games, count) = await gameService.GetAllGamesByCategoryAsync(category.Trim(), pageNumber, pageSize);
            var viewModel = new CategoryGamesViewModel
            {
                PageViewModel = new PaginationViewModel(count, pageNumber, pageSize),
                Games = games,
                Category = category
            };

            return View(viewModel);
        }



    }
}
