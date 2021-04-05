using GameStore.Web.App;
using GameStore.Web.App.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IGetGamesService gameService;

        public HomeController(ILogger<HomeController> logger, IGetGamesService gameService)
        {
            this.logger = logger;
            this.gameService = gameService;
        }

        public async Task<ActionResult<List<GameModel>>> Index()
        {
            var games = await gameService.GetGamesByDescedingOrderAsync();
            return View("Index", games.ToList());
        }


    }
}
