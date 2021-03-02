using GameStore.Web.App;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly GameService gameService;

        public HomeController(ILogger<HomeController> logger, GameService gameService)
        {
            this.logger = logger;
            this.gameService = gameService;
        }

        public async Task<IActionResult> Index()
        {
            var games = await gameService.GetGamesByDescedingOrderAsync();
            return View("Index", games.ToList());
        }


    }
}
