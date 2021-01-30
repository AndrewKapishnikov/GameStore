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
        //private readonly OrderMemoryService orderService;
        private readonly OrderService orderService;
   
        public OrderController(OrderService orderService)
        {

            this.orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var (hasValue, model) = await orderService.TryGetModelAsync();
            if (hasValue) return View(model);
            return View("CartEmpty");
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(int gameId, string returnUrl, int count = 1)
        {
            await orderService.AddGameAsync(gameId, count);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int gameId)
        {
            //TODO
            //Exception if remove item when session ended
            var model = await orderService.RemoveGameAsync(gameId);

            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateItem(int gameId, int count)
        {
            var model = await orderService.UpdateGameAsync(gameId, count);

            return View("Index", model);
        }

    }
}
