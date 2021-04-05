using GameStore.Web.App;
using GameStore.Web.App.Interfaces;
using NSubstitute;
using NUnit.Framework;
using System.Linq;
using GameStore.Web.Controllers;
using FluentAssertions;
using GameStore.DataEF;
using GameStore.Contractors;
using System.Collections.Generic;
using GameStore.Contractors.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace GameStore.UnitTests.Presentation
{
    [TestFixture]
    public class OrderControllerTests: BaseTest
    {
        [Test]
        public void Index_WhenCartIsNotEmpty_Returns_OrderModel()
        {
            //Arrange
            var order = Order.Mapper.Map(fakeOrderDto);
            var orderModel = OrderService.Map(order);
            var orderService = Substitute.For<AbstractOrderService>();
                orderService.TryGetModelAsync().Returns((true, orderModel));
            var userManager = FaketUserManager<User>();
            var emailService = Substitute.For<AbstractEmailService>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var paymentServices = Substitute.For<IEnumerable<IPaymentService>>();
            var webExternalServices = Substitute.For<IEnumerable<IExternalWebService>>();
            var orderController = new OrderController(orderService, userManager, emailService,
                                  deliveryServices, paymentServices, webExternalServices);
            //// Act
            var task = orderController.Index();

            //Assert
            var viewResult = task.Result as ViewResult;
            var model = viewResult.Model as OrderModel;
            Assert.Multiple(() =>
            {
                model.UserName.Should().Be(orderModel.UserName);
                viewResult.ViewName.Should().NotBe("CartEmpty");
            });
          
        }

        [Test]
        public void Index_WhenCartEmpty_Returns_ViewCartEmpty()
        {
         
            var order = Order.Mapper.Map(fakeOrderDto);
            var orderModel = OrderService.Map(order);
            var orderService = Substitute.For<AbstractOrderService>();
                orderService.TryGetModelAsync().Returns((false, orderModel));
            var userManager = FaketUserManager<User>();
            var emailService = Substitute.For<AbstractEmailService>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var paymentServices = Substitute.For<IEnumerable<IPaymentService>>();
            var webExternalServices = Substitute.For<IEnumerable<IExternalWebService>>();
            var orderController = new OrderController(orderService, userManager, emailService,
                                  deliveryServices, paymentServices, webExternalServices);
           
            var task = orderController.Index();
            var viewResult = task.Result as ViewResult;

            viewResult.ViewName.Should().Be("CartEmpty");
        }

        [Test]
        public void AddItem_WhenReturnUrlIsNotNullOrEmpty_And_ReturnUrlIsLocal_ReturtnsRedirectUrl()
        {
            int gameId = 1, count = 7;
            string returnUrl = "~/";
            var orderService = Substitute.For<AbstractOrderService>();
                orderService.AddGameAsync(gameId, count).Returns(new OrderModel());
            var userManager = FaketUserManager<User>();
            var emailService = Substitute.For<AbstractEmailService>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var paymentServices = Substitute.For<IEnumerable<IPaymentService>>();
            var webExternalServices = Substitute.For<IEnumerable<IExternalWebService>>();
            var orderController = new OrderController(orderService, userManager, emailService,
                                  deliveryServices, paymentServices, webExternalServices);
            // Act
            var task = orderController.AddItem(gameId, returnUrl, count);
            var redirectResult = task.Result as RedirectResult;

            Assert.Multiple(() =>
            {
                orderService.Received(1).AddGameAsync(gameId, 1);
                redirectResult.Url.Should().Be(returnUrl);
            });
        }

        [Test]
        public void AddItem_WhenReturnUrlIsNullOrEmpty_ReturtnsRedirectToActionIndex()
        {
            int gameId = 1, count = 7;
            string returnUrl = string.Empty;
            var orderService = Substitute.For<AbstractOrderService>();
                orderService.AddGameAsync(gameId, count).Returns(new OrderModel());
            var userManager = FaketUserManager<User>();
            var emailService = Substitute.For<AbstractEmailService>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var paymentServices = Substitute.For<IEnumerable<IPaymentService>>();
            var webExternalServices = Substitute.For<IEnumerable<IExternalWebService>>();
            var orderController = new OrderController(orderService, userManager, emailService,
                                  deliveryServices, paymentServices, webExternalServices);
            // Act
            var task = orderController.AddItem(gameId, returnUrl, count);
            var redirectToActionResult = task.Result as RedirectToActionResult;

            Assert.Multiple(() =>
            {
                orderService.Received(1).AddGameAsync(gameId, 1);
                redirectToActionResult.ActionName.Should().Be("Index");
            });
        }

        [Test]
        public void RemoveItem_WhenReturnUrlIsNotNullOrEmpty_And_ReturnUrlIsLocal_ReturtnsRedirectUrl()
        {
            int gameId = 1;
            string returnUrl = "~/";
            var orderService = Substitute.For<AbstractOrderService>();
                orderService.RemoveGameAsync(gameId).Returns(new OrderModel());
            var userManager = FaketUserManager<User>();
            var emailService = Substitute.For<AbstractEmailService>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var paymentServices = Substitute.For<IEnumerable<IPaymentService>>();
            var webExternalServices = Substitute.For<IEnumerable<IExternalWebService>>();
            var orderController = new OrderController(orderService, userManager, emailService,
                                  deliveryServices, paymentServices, webExternalServices);
            // Act
            var task = orderController.RemoveItem(gameId, returnUrl);
            var redirectResult = task.Result as RedirectResult;

            Assert.Multiple(() =>
            {
                orderService.Received(1).RemoveGameAsync(gameId);
                redirectResult.Url.Should().Be(returnUrl);
            });
        }


        [Test]
        public void RemoveItem_WhenReturnUrlIsNullOrEmpty_ReturtnsRedirectUrl()
        {
            int gameId = 1;
            string returnUrl = string.Empty;
            var orderService = Substitute.For<AbstractOrderService>();
                orderService.RemoveGameAsync(gameId).Returns(new OrderModel());
            var userManager = FaketUserManager<User>();
            var emailService = Substitute.For<AbstractEmailService>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var paymentServices = Substitute.For<IEnumerable<IPaymentService>>();
            var webExternalServices = Substitute.For<IEnumerable<IExternalWebService>>();
            var orderController = new OrderController(orderService, userManager, emailService,
                                  deliveryServices, paymentServices, webExternalServices);
            // Act
            var task = orderController.RemoveItem(gameId, returnUrl);
            var redirectToActionResult = task.Result as RedirectToActionResult;

            Assert.Multiple(() =>
            {
                orderService.Received(1).RemoveGameAsync(gameId);
                redirectToActionResult.ActionName.Should().Be("Index");
            });
        }

        [Test]
        public void UpdateItem_Pass_GameId_Count_ReturnsViewIndex()
        {
            int count = 5, gameId = 1;
            var order = Order.Mapper.Map(fakeOrderDto);
            var orderModel = OrderService.Map(order);
            var orderService = Substitute.For<AbstractOrderService>();
                orderService.UpdateGameAsync(gameId, count).Returns(orderModel);
            var userManager = FaketUserManager<User>();
            var emailService = Substitute.For<AbstractEmailService>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var paymentServices = Substitute.For<IEnumerable<IPaymentService>>();
            var webExternalServices = Substitute.For<IEnumerable<IExternalWebService>>();
            var orderController = new OrderController(orderService, userManager, emailService,
                                  deliveryServices, paymentServices, webExternalServices);
            //Act
            var task = orderController.UpdateItem(gameId, 5); 
            var viewResult = task.Result as ViewResult;
            var model = viewResult.Model as OrderModel;

            //Assert
            Assert.Multiple(() =>
            {
                model.UserName.Should().Be(orderModel.UserName);
                viewResult.ViewName.Should().Be("Index");
            });
        }

        [Test]
        public void MakeOrder_WhenUserIsAuthenticated_And_EquelOrderId_ReturnsViewDeliveryChoice()
        {
            string userName = Faker.Person.UserName; 
            var user = new User() { UserName = userName };
            var claims = new List<Claim>() { new Claim(ClaimTypes.Name, userName) };
            var identity = new ClaimsIdentity(claims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var order = Order.Mapper.Map(fakeOrderDto);
            var userManager = FaketUserManager<User>();
                userManager.FindByNameAsync(userName).Returns(user);
            var orderService = Substitute.For<AbstractOrderService>();
                orderService.GetOrderAsync().Returns(order);
                orderService.SetUserForOrderAsync(user, order.Id).Returns(Task.CompletedTask);
            var paymentServices = Substitute.For<IEnumerable<IPaymentService>>();
            var webExternalServices = Substitute.For<IEnumerable<IExternalWebService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var postamatDelivery = new PostamateDeliveryService();
            var enumerable = new PostamateDeliveryService[] { postamatDelivery };
            var orderController = new OrderController(orderService, userManager, emailService,
                                 enumerable, paymentServices, webExternalServices);
            var context = new ControllerContext { HttpContext = new DefaultHttpContext { User = claimsPrincipal }};
            orderController.ControllerContext = context;

            //Act
            var task = orderController.MakeOrder(order.Id);
            var viewResult = task.Result as ViewResult;
            var model = viewResult.Model as Dictionary<string, string>;
            var orderIdViewBag = (int)orderController.ViewBag.OrderId;

            //Assert
            Assert.Multiple(() =>
            {
                model.Keys.First().Should().Be(postamatDelivery.Name);
                viewResult.ViewName.Should().Be("DeliveryChoice");
                userManager.Received(1).FindByNameAsync(userName);
                orderService.Received(1).GetOrderAsync();
                orderService.SetUserForOrderAsync(user, order.Id);
                orderIdViewBag.Should().Be(order.Id);
            });
        }



        [Test]
        public void MakeOrder_WhenUserIsAuthenticated_And_NotEquelOrderId_ReturnsCartEmptyView()
        {
            int orderId = 2;
            string userName = Faker.Person.UserName;
            var claims = new List<Claim>() { new Claim(ClaimTypes.Name, userName) };
            var identity = new ClaimsIdentity(claims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var order = Order.Mapper.Map(fakeOrderDto);
            var userManager = FaketUserManager<User>();
            var orderService = Substitute.For<AbstractOrderService>();
                orderService.GetOrderAsync().Returns(order);
            var paymentServices = Substitute.For<IEnumerable<IPaymentService>>();
            var webExternalServices = Substitute.For<IEnumerable<IExternalWebService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var orderController = new OrderController(orderService, userManager, emailService,
                                 deliveryServices, paymentServices, webExternalServices);
            var context = new ControllerContext { HttpContext = new DefaultHttpContext { User = claimsPrincipal }};
            orderController.ControllerContext = context;

            //Act
            var task = orderController.MakeOrder(orderId);
            var viewResult = task.Result as ViewResult;
             
            //Assert
            viewResult.ViewName.Should().Be("CartEmpty");
        }


        [Test]
        public void MakeOrder_WhenUserNotAuthenticated_ReturnsViewNameAccountRegister()
        {
            int orderId = 1;
            var userManager = FaketUserManager<User>();
            var orderService = Substitute.For<AbstractOrderService>();
            var paymentServices = Substitute.For<IEnumerable<IPaymentService>>();
            var webExternalServices = Substitute.For<IEnumerable<IExternalWebService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var orderController = new OrderController(orderService, userManager, emailService,
                                 deliveryServices, paymentServices, webExternalServices);
            //Act
            var task = orderController.MakeOrder(orderId);
            var viewResult = task.Result as ViewResult;

            //Assert
            viewResult.ViewName.Should().Be("../Account/Register");
        }

        [Test]
        public void StartDelivery_WhenDeliveryServiceIsCourierDeliveryService_ReturnsViewPaymentChoice()
        {
            var order = Order.Mapper.Map(fakeOrderDto);
            var courierDelivery = new CourierDeliveryService();
            var deliveryServices = new CourierDeliveryService[] {courierDelivery };
            var userManager = FaketUserManager<User>();
            var orderService = Substitute.For<AbstractOrderService>();
                orderService.GetOrderAsync().Returns(order);
                orderService.SetDeliveryAsync(Arg.Any<Delivery>()).Returns(OrderService.Map(order));
            var cashPayment = new CashPaymentService();
            var paymentServices = new CashPaymentService[] { cashPayment };
            var webExternalServices = Substitute.For<IEnumerable<IExternalWebService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderController = new OrderController(orderService, userManager, emailService,
                                 deliveryServices, paymentServices, webExternalServices);
            //Act
            var task = orderController.StartDelivery(courierDelivery.Name, order.Id);
            var viewResult = task.Result as ViewResult;
            var model = viewResult.Model as Dictionary<string, string>;

            //Assert
            Assert.Multiple(() =>
            {
                viewResult.ViewName.Should().Be("PaymentChoice");
                orderService.Received(1).GetOrderAsync();
                model.First().Key.Should().Be(cashPayment.Name);
            });
        }



        [Test]
        public void StartDelivery_WhenDeliveryServiceIsNotCourierDeliveryService_ReturnsViewNextDelivery()
        {
            var order = Order.Mapper.Map(fakeOrderDto);
            var postamatDelivery = new PostamateDeliveryService();
            var deliveryServices = new PostamateDeliveryService[] { postamatDelivery };
            var userManager = FaketUserManager<User>();
            var orderService = Substitute.For<AbstractOrderService>();
                orderService.GetOrderAsync().Returns(order);
            var paymentServices = Substitute.For<IEnumerable<IPaymentService>>();
            var webExternalServices = Substitute.For<IEnumerable<IExternalWebService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderController = new OrderController(orderService, userManager, emailService,
                                 deliveryServices, paymentServices, webExternalServices);
            //Act
            var task = orderController.StartDelivery(postamatDelivery.Name, order.Id);
            var viewResult = task.Result as ViewResult;
            var model = viewResult.Model as Dictionary<string, string>;

            //Assert
            Assert.Multiple(() =>
            {
                viewResult.ViewName.Should().Be("NextDelivery");
                orderService.Received(1).GetOrderAsync();
            });
        }

        [Test]
        public void StartDelivery_WhenOrdersNotMatch_ReturnsViewCartEmpty()
        {
            var order = Order.Mapper.Map(fakeOrderDto);
            var orderService = Substitute.For<AbstractOrderService>();
                orderService.GetOrderAsync().Returns(order);
            var userManager = FaketUserManager<User>();
            var paymentServices = Substitute.For<IEnumerable<IPaymentService>>();
            var webExternalServices = Substitute.For<IEnumerable<IExternalWebService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
             var orderController = new OrderController(orderService, userManager, emailService,
                                   deliveryServices, paymentServices, webExternalServices);
            //Act
            var task = orderController.StartDelivery("Postamet", 2);
            var viewResult = task.Result as ViewResult;
            //Assert
            viewResult.ViewName.Should().Be("CartEmpty");
        }

        [Test]
        public void NextDeliveryStep_WhenDataStepsIsNotFinal_ReturnsViewNextDelivery()
        {
            var postamatDelivery = new PostamateDeliveryService();
            var deliveryServices = new PostamateDeliveryService[] { postamatDelivery };
            var userManager = FaketUserManager<User>();
            var orderService = Substitute.For<AbstractOrderService>();
            var paymentServices = Substitute.For<IEnumerable<IPaymentService>>();
            var webExternalServices = Substitute.For<IEnumerable<IExternalWebService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderController = new OrderController(orderService, userManager, emailService,
                                 deliveryServices, paymentServices, webExternalServices);
            //Act
            var task = orderController.NextDeliveryStep(postamatDelivery.Name, 1, new Dictionary<string, string>() { {"city", "1" } });
            var viewResult = task.Result as ViewResult;

            //Assert
             viewResult.ViewName.Should().Be("NextDelivery");
        }


        [Test]
        public void NextDeliveryStep_WhenDataStepsIsFinal_And_DeliveryServiceIsPostamateDelivery_ReturnsViewModelPaymentsExceptCash()
        {
            //Arrange
            var order = Order.Mapper.Map(fakeOrderDto);
            var postamatDelivery = new PostamateDeliveryService();
            var deliveryServices = new PostamateDeliveryService[] { postamatDelivery };
            var userManager = FaketUserManager<User>();
            var orderService = Substitute.For<AbstractOrderService>();
                orderService.GetOrderAsync().Returns(order);
            var paypalPayment = new PayPalPaymentService();
            var paymentServices = new PayPalPaymentService[] { paypalPayment };
            var webExternalServices = Substitute.For<IEnumerable<IExternalWebService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderController = new OrderController(orderService, userManager, emailService,
                                 deliveryServices, paymentServices, webExternalServices);
            //Act
            var task = orderController.NextDeliveryStep(postamatDelivery.Name, 2, new Dictionary<string, string>() {{"city", "1"}, {"postamate", "1"}});
            var viewResult = task.Result as ViewResult;
            var model = viewResult.Model as Dictionary<string, string>;

            //Assert
            Assert.Multiple(() =>
            {
                viewResult.ViewName.Should().Be("PaymentChoice");
                orderService.Received(1).GetOrderAsync();
                model.First().Key.Should().Be(paypalPayment.Name);
                model.First().Value.Should().Be(paypalPayment.Title);
            });
        }

        [Test]
        public void StartPayment_WhenPaymentServiceIsWebExternalService_RedirectToExternalServiceUri()
        {
            var order = Order.Mapper.Map(fakeOrderDto);
            var userManager = FaketUserManager<User>();
            var fakeRequest = Substitute.For<HttpRequest>();
                fakeRequest.Scheme.Returns("http");
                fakeRequest.Host.Returns(new HostString("localhost", 12278));
            var fakeContext = Substitute.For<HttpContext>();
                fakeContext.Request.Returns(fakeRequest);
            var orderService = Substitute.For<AbstractOrderService>();
                orderService.GetOrderAsync().Returns(order);
            var accessor = Substitute.For<IHttpContextAccessor>();
                accessor.HttpContext.Request.Returns(fakeRequest);
            var webService = new EmulateKassaPaymentService(accessor);
            var paymentServices = new EmulateKassaPaymentService[] { webService };
            var emailService = Substitute.For<AbstractEmailService>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var orderController = new OrderController(orderService, userManager, emailService,
                                   deliveryServices, paymentServices, paymentServices);
           var urlHelper = Substitute.For<IUrlHelper>();
                urlHelper.Action("test").Returns("testController");
            orderController.ControllerContext = new ControllerContext { HttpContext = fakeContext};
            orderController.Url = urlHelper;
            
            //Act
            var task = orderController.StartPayment(webService.Name, order.Id);
            var viewResult = task.Result as RedirectResult;

            //Assert
            Assert.Multiple(() =>
            {
                viewResult.Url.Contains("http://localhost:12278").Should().Be(true);
                viewResult.Url.Contains(webService.Name).Should().Be(true);
            });
        }

        [Test]
        public void StartPayment_WhenPaymentServiceIsCashPaymentService_ReturnsViewFinishOrder()
        {
            var order = Order.Mapper.Map(fakeOrderDto);
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var userManager = FaketUserManager<User>();
            var orderService = Substitute.For<AbstractOrderService>();
                orderService.GetOrderAsync().Returns(order);
            var cashPayment = new CashPaymentService();
            var paymentServices = new CashPaymentService[] { cashPayment };
            var webExternalServices = Substitute.For<IEnumerable<IExternalWebService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderController = new OrderController(orderService, userManager, emailService,
                                 deliveryServices, paymentServices, webExternalServices);
            //Act
            var task = orderController.StartPayment(cashPayment.Name, order.Id);
            var viewResult = task.Result as ViewResult;
            //Assert
            viewResult.ViewName.Should().Be("FinishOrder");
        }


        [Test]
        public void StartPayment_WhenOrdersNotMatch_ReturnsViewCartEmpty()
        {
            var order = Order.Mapper.Map(fakeOrderDto);
            var orderService = Substitute.For<AbstractOrderService>();
                orderService.GetOrderAsync().Returns(order);
            var userManager = FaketUserManager<User>();
            var paymentServices = Substitute.For<IEnumerable<IPaymentService>>();
            var webExternalServices = Substitute.For<IEnumerable<IExternalWebService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var orderController = new OrderController(orderService, userManager, emailService,
                                  deliveryServices, paymentServices, webExternalServices);
            //Act
            var task = orderController.StartPayment("Cash", 2);
            var viewResult = task.Result as ViewResult;
            //Assert
            viewResult.ViewName.Should().Be("CartEmpty");
        }

        [Test]
        public void NextPaymentStep_WhenDataStepIsFinal_ReturnsViewFinishOrder()
        {
            var order = Order.Mapper.Map(fakeOrderDto);
            var orderModel = OrderService.Map(order);
            var paypalPayment = new PayPalPaymentService();
            var paymentServices = new PayPalPaymentService[] { paypalPayment };
            var userManager = FaketUserManager<User>();
            var orderService = Substitute.For<AbstractOrderService>();
                 orderService.SetPaymentAsync(Arg.Any<Payment>()).Returns(orderModel);
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var webExternalServices = Substitute.For<IEnumerable<IExternalWebService>>();
            var emailService = Substitute.For<AbstractEmailService>();
                emailService.SendOrderEmailAsync(orderModel).Returns(Task.CompletedTask);
            var orderController = new OrderController(orderService, userManager, emailService,
                                 deliveryServices, paymentServices, webExternalServices);
            //Act
            var task = orderController.NextPaymentStep(paypalPayment.Name, 1, new Dictionary<string, string>());
            var viewResult = task.Result as ViewResult;
          
            //Assert
            viewResult.ViewName.Should().Be("FinishOrder");
       }

    }
}
