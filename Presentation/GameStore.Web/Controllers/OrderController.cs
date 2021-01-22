using GameStore.Web.App;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Web.Controllers
{
    public class OrderController: Controller
    {
        private readonly OrderService orderService;
   
        public OrderController(OrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (orderService.TryGetModel(out OrderModel model))
                return View(model);

            return View("CartEmpty");
        }

        [HttpPost]
        public IActionResult AddItem(int gameId, string returnUrl, int count = 1)
        {
            orderService.AddGame(gameId, count);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult RemoveItem(int gameId)
        {
            var model = orderService.RemoveGame(gameId);

            return View("Index", model);
        }

        [HttpPost]
        public IActionResult UpdateItem(int gameId, int count)
        {
            var model = orderService.UpdateGame(gameId, count);

            return View("Index", model);
        }

    }
}
