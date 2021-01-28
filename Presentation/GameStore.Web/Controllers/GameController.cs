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

        public GameController(GameService gameService)
        {
            this.gameService = gameService;

        }

        public IActionResult Index(int id)
        {
            //  TODO  Exception if remove item when session ended
            var model = gameService.GetGameById(id);

            return View(model);
        }
    }
}
