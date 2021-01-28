using GameStore.Web.App;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Web.Controllers
{
    public class SearchController:Controller
    {
        private readonly GameService gameService;

        public SearchController(GameService gameService)
        {
            this.gameService = gameService;
        }

        public IActionResult Index(string query)
        {
        
            var games = gameService.GetAllGamesByNameOrPublisher(query);

            return View(games);
        }
    }
}
