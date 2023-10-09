using GameStore.Contractors;
using GameStore.Contractors.Interfaces;
using GameStore.DataEF;
using GameStore.Web.App;
using GameStore.Web.App.Interfaces;
using GameStore.Web.App.Models;
using GameStore.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.Web.Controllers
{
    public class OrderController: Controller
    {
        //private readonly OrderMemoryService orderService;
        private readonly AbstractOrderService orderService;
        private readonly UserManager<User> userManager;
        private readonly AbstractEmailService emailService;
        private readonly IEnumerable<IDeliveryService> deliveryServices;
        private readonly IEnumerable<IPaymentService> paymentServices;
        private readonly IEnumerable<IExternalWebService> webExternalService;

        public OrderController(AbstractOrderService orderService,
                               UserManager<User> userManager,
                               AbstractEmailService emailService,
                               IEnumerable<IDeliveryService> deliveryServices,
                               IEnumerable<IPaymentService> paymentServices,
                               IEnumerable<IExternalWebService> webExternalService)
                              
        {
            this.orderService = orderService;
            this.userManager = userManager;
            this.emailService = emailService;
            this.deliveryServices = deliveryServices;
            this.paymentServices = paymentServices;
            this.webExternalService = webExternalService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var (hasValue, model) = await orderService.TryGetModelAsync();
            if (hasValue) return View(model);
            return View("CartEmpty");
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(int gameId, string returnUrl, int count)
        {
            if (count != -1)
                count = 1;
            await orderService.AddGameAsync(gameId, count);

            if (!string.IsNullOrEmpty(returnUrl) && (Url?.IsLocalUrl(returnUrl) ?? true))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int gameId, string returnUrl)
        {
            var model = await orderService.RemoveGameAsync(gameId);
            if (!string.IsNullOrEmpty(returnUrl) && (Url?.IsLocalUrl(returnUrl) ?? true))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
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
            if(User?.Identity.IsAuthenticated ?? false)
            {
                var order = await orderService.GetOrderAsync();
                if(orderId == order.Id)
                {
                    var user = await userManager.FindByNameAsync(User.Identity.Name);
                    await orderService.SetUserForOrderAsync(user, orderId);
                    var deliveryChoice = deliveryServices.ToDictionary(service => service.Name, service => service.Title);
                    ViewBag.OrderId = orderId;
                    return View("DeliveryChoice", deliveryChoice);
                }
                return View("CartEmpty");
            }
            else
            {
                ViewBag.orderUrl = $"{Request?.Path.ToString().ToLower()}?orderId={orderId}";
                return View("../Account/Register", new RegisterViewModel());
            }
           
        }

    
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> StartDelivery(string service, int orderId)
        {
            var order = await orderService.GetOrderAsync(); 
            if( order.Id == orderId)
            {
                var deliveryService = deliveryServices.Single(p => p.Name == service);
                var dataSteps = deliveryService.FirstStep(order);
                if (deliveryService is CourierDeliveryService)
                {
                   var delivery = deliveryService.GetDelivery(dataSteps);
                    var orderModel = await orderService.SetDeliveryAsync(delivery);
                    ViewBag.OrderId = order.Id;
                    var paymentChoice = paymentServices.ToDictionary(service => service.Name, service => service.Title);
                    return View("PaymentChoice", paymentChoice);
                }
                var webService = webExternalService.SingleOrDefault(s => s.Name == service);
                if (webService == null)
                    return View("NextDelivery", dataSteps);

                var returnUri = GetReturnUri(nameof(NextDeliveryStep), webService.Name);
                var redirectUri = await webService.GetServiceUriAsync(dataSteps.Parameters, returnUri);
                return Redirect(redirectUri.ToString());
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
                return View("NextDelivery", dataSteps);

            var delivery = deliveryService.GetDelivery(dataSteps);
            await orderService.SetDeliveryAsync(delivery);

            var order = await orderService.GetOrderAsync();
            ViewBag.OrderId = order.Id;
            if (deliveryService is PostamateDeliveryService)
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
            var order = await orderService.GetOrderAsync();
            if(order.Id == orderId)
            {
                var paymentService = paymentServices.Single(choice => choice.Name == service);
                var dataStepsPayment = paymentService.FirstStep(order);
                if (paymentService is CashPaymentService)
                {
                    var finishModel = await SetPaymentAndSendEmail(paymentService, dataStepsPayment);
                    return View("FinishOrder", finishModel);
                }
                if (paymentService is PayPalPaymentService)
                {
                    var orderModel = await orderService.GetOrderDetailAsync(orderId);
                    ViewBag.payPalConfig = HttpContext.RequestServices.GetService(typeof(PayPalConfig)) as PayPalConfig;
                    ViewBag.returnUrl = GetReturnUri(nameof(SuccessPayPal), paymentService.Name, orderId);
                    return View("ServicePayPal", orderModel);
                }
                var webService = webExternalService.SingleOrDefault(s => s.Name == service);
                if (webService == null)
                    return View("NextPaymentChoice", dataStepsPayment);

                var returnUri = GetReturnUri(nameof(NextPaymentStep), webService.Name);
                var redirectUri = await webService.GetServiceUriAsync(dataStepsPayment.Parameters, returnUri);
                return Redirect(redirectUri.ToString());

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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SuccessPayPal(string tx, string service, int orderId)
        {
            var order = await orderService.GetOrderAsync();
            var payPalConfig = HttpContext.RequestServices.GetService(typeof(PayPalConfig)) as PayPalConfig;
            var result = PDTHolder.Success(tx, payPalConfig);
            if (result != null && orderId == order.Id)
            {
                var values = result.GetPayPalPaymentParameters(orderId);
                var paymentService = paymentServices.Single(choice => choice.Name == service);
                var dataSteps = paymentService.NextStep(1, values);
                var finishModel = await SetPaymentAndSendEmail(paymentService, dataSteps);
                ViewBag.transactionPayPal = values;
                return View("FinishOrder", finishModel);
            }
            return View("NotSuccessPayPal");
        }

        private async Task<OrderModel> SetPaymentAndSendEmail(IPaymentService paymentService, DataSteps data)
        {
            var payment = paymentService.GetPayment(data);
            var finishModel = await orderService.SetPaymentAsync(payment);
            await emailService.SendOrderEmailAsync(finishModel);
            return finishModel;
        }

        private Uri GetReturnUri(string action, string serviceName)
        {
            var query = QueryString.Create("service", serviceName);
            return CreateReturnUri(action, query);
        }

        private Uri GetReturnUri(string action, string serviceName, int orderId)
        {
            var query = QueryString.Create("service", serviceName);
            query += QueryString.Create("orderId", orderId.ToString());
            return CreateReturnUri(action, query);
        }

        private Uri CreateReturnUri(string action, QueryString query)
        {
             
            var builder = new UriBuilder(Request.Scheme, Request.Host.Host)
            {
                Path = Url.Action(action),
                Query = query.ToString(),
            };
          
            if (Request.Host.Port != null)
                    builder.Port = Request.Host.Port.Value;
           
            return builder.Uri;
        }

    }
}
