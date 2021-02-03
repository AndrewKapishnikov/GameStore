using GameStore.Contractors;
using GameStore.DataEF;
using GameStore.Web.App;
using GameStore.Web.Models;
using Microsoft.AspNetCore.Identity;
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
        //If I delete an order from the database, it is necessary to delete an entry in the cart, if such an order exists there //TODO this

        //private readonly OrderMemoryService orderService;
        private readonly OrderService orderService;
        private readonly UserManager<User> userManager;
        private readonly IEnumerable<IDeliveryService> deliveryServices;

        public OrderController(OrderService orderService,
                               UserManager<User> userManager,
                               IEnumerable<IDeliveryService> deliveryServices)
        {
            this.orderService = orderService;
            this.userManager = userManager;
            this.deliveryServices = deliveryServices;
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


        [HttpPost]
        public async Task<IActionResult> MakeOrder(int orderId)
        {
            if(User.Identity.IsAuthenticated)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                await orderService.SetUserForOrderAsync(user, orderId);
                var deliveryMethods = deliveryServices.ToDictionary(service => service.Name,
                                                                    service => service.Title);
                ViewBag.OrderId = orderId;
                return View("DeliveryChoice", deliveryMethods);
            }
            else
            {
                ViewBag.orderUrl = $"{Request.Path.ToString().ToLower()}?orderId={orderId}";
                return View("../Account/Register", new RegisterViewModel());
            }

           
        }


        [HttpPost]
        public async Task<IActionResult> StartDelivery(string serviceName)
        {
            await Task.Yield();
            return View("Home");
        }


    }
}
