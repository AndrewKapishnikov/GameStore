using GameStore.Web.App;
using GameStore.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public IActionResult Index()
        {
            var games = gameService.GetGamesByDescedingOrder().ToList();
            return View("Index", games);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
