using GameStore.Contractors;
using GameStore.DataEF;
using GameStore.Web.App;
using GameStore.Web.Models;
using Microsoft.AspNetCore.Authorization;
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
        private readonly EmailService emailService;
        private readonly IEnumerable<IDeliveryService> deliveryServices;
        private readonly IEnumerable<IPaymentService> paymentServices;

        public OrderController(OrderService orderService,
                               UserManager<User> userManager,
                                EmailService emailService,
                               IEnumerable<IDeliveryService> deliveryServices,
                               IEnumerable<IPaymentService> paymentServices)
                              
        {
            this.orderService = orderService;
            this.userManager = userManager;
            this.emailService = emailService;
            this.deliveryServices = deliveryServices;
            this.paymentServices = paymentServices;
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
                var order = await orderService.GetOrderAsync();
                if(orderId == order.Id)
                {
                    await orderService.SetUserForOrderAsync(user, orderId);
                    var deliveryChoice = deliveryServices.ToDictionary(service => service.Name, service => service.Title);
                    ViewBag.OrderId = orderId;
                    return View("DeliveryChoice", deliveryChoice);
                }
                return View("CartEmpty");
            }
            else
            {
                ViewBag.orderUrl = $"{Request.Path.ToString().ToLower()}?orderId={orderId}";
                return View("../Account/Register", new RegisterViewModel());
            }
           
        }

    
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> StartDelivery(string service, int orderId)
        {
            var deliveryService = deliveryServices.Single(p => p.Name == service);
            var order = await orderService.GetOrderAsync(); 
            if( order.Id == orderId)
            {
                if(deliveryService.Name == "Courier")
                {
                    var data = deliveryService.FirstStep(order);
                    var delivery = deliveryService.GetDelivery(data);
                    await orderService.SetDeliveryAsync(delivery);
                    ViewBag.OrderId = order.Id;
                    var paymentChoice = paymentServices.ToDictionary(service => service.Name, service => service.Title);
                    return View("PaymentChoice", paymentChoice);
                }
                var dataSteps = deliveryService.FirstStep(order);
                return View("NextDeliveryChoice", dataSteps);
            }
            return View("CartEmpty");
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> NextDeliveryStep(string service, int step, Dictionary<string, string> values)
        {
            var deliveryService = deliveryServices.Single(p => p.Name == service);
            var dataSteps = deliveryService.NextStep(step, values);
            if (!dataSteps.IsFinal)
                return View("NextDeliveryChoice", dataSteps);

            var delivery = deliveryService.GetDelivery(dataSteps);
            await orderService.SetDeliveryAsync(delivery);

            var order = await orderService.GetOrderAsync();
            ViewBag.OrderId = order.Id;
            if (deliveryService.Name == "Postamate")
            {
                var payment = paymentServices.Where(p => p.Name != "Cash").ToDictionary(service => service.Name, service => service.Title);
                return View("PaymentChoice", payment);
            }
            else
            {
                var paymentChoice = paymentServices.ToDictionary(service => service.Name, service => service.Title);
                return View("PaymentChoice", paymentChoice);
            }
        }
        


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> StartPayment(string service, int orderId)
        {
            var paymentService = paymentServices.Single(choice => choice.Name == service);
            var order = await orderService.GetOrderAsync();
            if(order.Id == orderId)
            {
                var dataStepsPayment = paymentService.FirstStep(order);
                if (paymentService.Name == "Cash")
                {
                    var finishModel = await SetPaymentAndSendEmail(paymentService, dataStepsPayment);
                    return View("FinishOrder", finishModel);
                }
                return View("NextPaymentChoice", dataStepsPayment);
            }
            return View("CartEmpty");
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> NextPaymentStep(string service, int step, Dictionary<string, string> values)
        {
            var paymentService = paymentServices.Single(choice => choice.Name == service);

            var dataSteps = paymentService.NextStep(step, values);
            if (!dataSteps.IsFinal)
                return View("NextPaymentChoice", dataSteps);

            var finishModel = await SetPaymentAndSendEmail(paymentService, dataSteps);

            return View("FinishOrder", finishModel);
            
        }

        private async Task<OrderModel> SetPaymentAndSendEmail(IPaymentService paymentService, DataSteps data)
        {
            var payment = paymentService.GetPayment(data);
            var finishModel = await orderService.SetPaymentAsync(payment);
            await emailService.SendOrderEmailAsync(finishModel);
            return finishModel;
        }



    }
}
