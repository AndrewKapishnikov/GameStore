using GameStore.Web.App;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
         
            //  TODO  Exception if remove item when session ended
            var model = await gameService.GetGameByIdAsync(id);

            return View(model);
        }

        [Route("{controller}/{action}/{category}")]
        public async Task<IActionResult> CategoryGames(string category)
        {
            var games = await gameService.GetAllGamesByCategoryAsync(category.Trim());

            return View(games);
        }



    }
}
