using GameStore.Web.App;
using GameStore.Web.App.Interfaces;
using GameStore.Web.Models;
using GameStore.Web.Models.AdminPanelModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.Web.Controllers
{
    public class GameController: Controller
    {
        private readonly IGetGamesService getGamesService;
      
        public GameController(IGetGamesService getGamesService)
        {
            this.getGamesService = getGamesService;
        }

        public async Task<ActionResult<IReadOnlyCollection<GameModel>>> SearchGame(string query)
        {
            var games = await getGamesService.GetAllGamesByNameOrPublisherAsync(query);
            
            return View(games);
        }

        public async Task<ActionResult<GameModel>> Index(int id)
        {
            var model = await getGamesService.GetGameByIdAsync(id);

            return View(model);
        }

        [Route("{controller}/{action}/{category}/{page?}")]
        public async Task<ActionResult<CategoryGamesViewModel>> CategoryGames(string category, int? page)
        {
            int pageNumber = (page ?? 1);
            if (pageNumber > 100) pageNumber = 100;
            if (pageNumber < 1) pageNumber = 1;
            int pageSize = 7;

            var (games, count) = await getGamesService.GetAllGamesByCategoryAsync(category.Trim(), pageNumber, pageSize);
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
