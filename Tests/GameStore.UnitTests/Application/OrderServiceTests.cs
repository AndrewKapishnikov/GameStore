using NUnit.Framework;
using NSubstitute;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using GameStore.Web.App;
using GameStore.DataEF;
using System.IO;
using GameStore.Web.App.Models;

namespace GameStore.UnitTests.Application
{
    [TestFixture]
    public class OrderServiceTests: BaseTest
    {
        private readonly HttpContextAccessor contextAccessor = new HttpContextAccessor();

        [Test]
        public void TryGetModelAsync_When_OrderSession_Is_Empty_Returns_False()
        {
            //Arrange
            var fakeCart = new Cart(1, 2, 4);
            var orderRepository = Substitute.For<IOrderRepositoryAsync>();
            var gameRepository = Substitute.For<IGetGamesRepositoryAsync>();
            var contextAccessor = Substitute.For<IHttpContextAccessor>();
                contextAccessor.HttpContext.Session.TryGetCart(out fakeCart).Returns(false);
            var orderService = new OrderService(gameRepository, orderRepository, contextAccessor);

            //Act
            Task<(bool, OrderModel)> task =  orderService.TryGetModelAsync();
            var tuple = task.Result;

            //Assert
            Assert.Multiple(() =>
            {
                tuple.Item1.Should().BeFalse();
                tuple.Item2.Should().BeNull();
                contextAccessor.HttpContext.Session.Received(1).TryGetCart(out fakeCart);
            });
        }


        [Test]
        public void TryGetModelAsync_When_OrderSessionContainCart_Returns_TrueAndOrderModel()
        {
            byte[] bytes;
            var order = Order.Mapper.Map(fakeOrderDto);
            Cart fakeCart = new Cart(order.Id, 2, 4);
            var orderRepository = Substitute.For<IOrderRepositoryAsync>();
                orderRepository.GetByIdAsync(order.Id).Returns(order);
            var gameRepository = Substitute.For<IGetGamesRepositoryAsync>();
            var contextAccessor = Substitute.For<IHttpContextAccessor>();
                contextAccessor.HttpContext.Session.TryGetValue("SomeKey", out bytes).ReturnsForAnyArgs(x =>
            {
                x[1] = CreateByteMassivByCartContent(fakeCart);
                return true;
            });
            var orderService = new OrderService(gameRepository, orderRepository, contextAccessor);

            //Act
            Task<(bool, OrderModel)> task = orderService.TryGetModelAsync();
            var tuple = task.Result;
            var orderModel = tuple.Item2;

            Assert.Multiple(() =>
            {
                tuple.Item1.Should().BeTrue();
                orderModel.OrderDateAndTime.Should().Be(order.DateOfOrder);
                orderModel.DeliveryName.Should().Be(order.Delivery.NameDelivery);
                contextAccessor.HttpContext.Session.Received(1).TryGetCart(out fakeCart);
                orderRepository.Received(1).GetByIdAsync(order.Id);
            });
        }

        [Test]
        public void AddGameAsync_WhenGameNotExistsInOrder_AddNewOrderItemWithGameInOrder()  
        {
            byte[] bytes; int count = 1;
            var order = Order.Mapper.Map(fakeOrderDto);
            int totalCount = order.TotalCount;
            Cart fakeCart = new Cart(order.Id, order.TotalCount, order.TotalPrice);
            var gameDto = collectionGameDto.First();
            var game = Game.Mapper.Map(gameDto);
            var orderRepository = Substitute.For<IOrderRepositoryAsync>();
                orderRepository.GetByIdAsync(order.Id).Returns(order);
                orderRepository.UpdateAsync(order).Returns(Task.CompletedTask);
            var gameRepository = Substitute.For<IGetGamesRepositoryAsync>();
                gameRepository.GetGameByIdAsync(game.Id, false).Returns(game);
            var contextAccessor = Substitute.For<IHttpContextAccessor>();
                contextAccessor.HttpContext.Session.TryGetValue("SomeKey", out bytes).ReturnsForAnyArgs(x =>
            {
                x[1] = CreateByteMassivByCartContent(fakeCart);
                return true;
            });
            var orderService = new OrderService(gameRepository, orderRepository, contextAccessor);

            //Act
            Task<OrderModel> task = orderService.AddGameAsync(game.Id, count);
            OrderModel orderModel = task.Result;

            //Assert
            Assert.Multiple(() =>
            {
                orderModel.Id.Should().Be(order.Id);
                orderModel.OrderItems.Length.Should().Be(1);
                orderModel.TotalCount.Should().Be(totalCount + count);
                orderRepository.Received(1).GetByIdAsync(order.Id);
                orderRepository.Received(1).UpdateAsync(order);
                gameRepository.Received(1).GetGameByIdAsync(game.Id, false);
            });
        }


        [Test]
        public void AddGameAsync_WhenGameExistsInOrder_ChangeCountOrderItemGameByOne()
        {
            byte[] bytes;
            int count = 5;
            var game = Game.Mapper.Map(collectionGameDto.First());
            var order = Order.Mapper.Map(fakeOrderDto);
            order.Items.Add(game, count);
            
            var orderRepository = Substitute.For<IOrderRepositoryAsync>();
                orderRepository.CreateAsync().Returns(order);
                orderRepository.UpdateAsync(order).Returns(Task.CompletedTask);
            var gameRepository = Substitute.For<IGetGamesRepositoryAsync>();
                gameRepository.GetGameByIdAsync(game.Id, false).Returns(game);
            var contextAccessor = Substitute.For<IHttpContextAccessor>();
                contextAccessor.HttpContext.Session.TryGetValue("SomeKey", out bytes).ReturnsForAnyArgs(x =>
                {
                  x[1] = null;
                  return false;
                });
            var orderService = new OrderService(gameRepository, orderRepository, contextAccessor);

            //Act
            Task<OrderModel> task = orderService.AddGameAsync(game.Id, 1);
            OrderModel orderModel = task.Result;

            //Assert
            Assert.Multiple(() =>
            {
                orderModel.Id.Should().Be(order.Id);
                orderModel.OrderItems.First().Count.Should().Be(count + 1);
                orderModel.TotalCount.Should().Be(count + 1);
                orderRepository.Received(1).CreateAsync();
                orderRepository.Received(1).UpdateAsync(order);
                gameRepository.GetGameByIdAsync(game.Id, false);
            });

        }

        [Test]
        public void RemoveGameAsync_Pass_GameId()
        {
            byte[] bytes;
            int count = 1;
            var game = Game.Mapper.Map(collectionGameDto.First());
            var order = Order.Mapper.Map(fakeOrderDto);
            order.Items.Add(game, count);
            int totalCount = order.TotalCount;
            Cart fakeCart = new Cart(order.Id, order.TotalCount, order.TotalPrice);

            var orderRepository = Substitute.For<IOrderRepositoryAsync>();
                orderRepository.GetByIdAsync(order.Id).Returns(order);
                orderRepository.UpdateAsync(order).Returns(Task.CompletedTask);
                orderRepository.RemoveAsync(order).Returns(Task.CompletedTask);
            var gameRepository = Substitute.For<IGetGamesRepositoryAsync>();
            var contextAccessor = Substitute.For<IHttpContextAccessor>();
                contextAccessor.HttpContext.Session.TryGetValue("SomeKey", out bytes).ReturnsForAnyArgs(x =>
            {
                x[1] = CreateByteMassivByCartContent(fakeCart);
                return true;
            });
            var orderService = new OrderService(gameRepository, orderRepository, contextAccessor);

            //Act
            Task<OrderModel> task =  orderService.RemoveGameAsync(game.Id);
            OrderModel orderModel = task.Result;

            //Assert
            Assert.Multiple(() =>
            {
                orderModel.Id.Should().Be(order.Id);
                orderModel.OrderItems.Length.Should().Be(0);
                orderModel.TotalCount.Should().Be(0);
                orderRepository.Received(1).UpdateAsync(order);
                orderRepository.Received(1).RemoveAsync(order);
            });
        }

        [Test]
        public void GetOrderAsync_WhenCartIsEmpty_ThrowInvalidOperationException()
        {
            byte[] bytes;
            var orderRepository = Substitute.For<IOrderRepositoryAsync>();
            var gameRepository = Substitute.For<IGetGamesRepositoryAsync>();
            var contextAccessor = Substitute.For<IHttpContextAccessor>();
                contextAccessor.HttpContext.Session.TryGetValue("SomeKey", out bytes).ReturnsForAnyArgs(x =>
                {
                    x[1] = null;
                    return false;
                });
            var orderService = new OrderService(gameRepository, orderRepository, contextAccessor);

            //Act
            Func<Task<Order>> getOrderAsync = () => orderService.GetOrderAsync();

            //Assert
            Assert.Multiple(() =>
            {
                getOrderAsync.Should().Throw<InvalidOperationException>();
            });

        }

        [Test]
        public void UpdateGameAsync_ChangeCount()
        {
            byte[] bytes;
            int count = 4;
            int changeCount = 7;
            var game = Game.Mapper.Map(collectionGameDto.First());
            var order = Order.Mapper.Map(fakeOrderDto);
            order.Items.Add(game, count);
            Cart fakeCart = new Cart(order.Id, order.TotalCount, order.TotalPrice);

            var orderRepository = Substitute.For<IOrderRepositoryAsync>();
                orderRepository.GetByIdAsync(order.Id).Returns(order);
                orderRepository.UpdateAsync(order).Returns(Task.CompletedTask);
            var gameRepository = Substitute.For<IGetGamesRepositoryAsync>();
                gameRepository.GetGameByIdAsync(game.Id, false).Returns(game);
            var contextAccessor = Substitute.For<IHttpContextAccessor>();
            contextAccessor.HttpContext.Session.TryGetValue("SomeKey", out bytes).ReturnsForAnyArgs(x =>
            {
                x[1] = CreateByteMassivByCartContent(fakeCart);
                return true;
            });
            var orderService = new OrderService(gameRepository, orderRepository, contextAccessor);

            Task<OrderModel> task = orderService.UpdateGameAsync(game.Id, 7);
            OrderModel orderModel = task.Result;

            //Assert
            Assert.Multiple(() =>
            {
                orderModel.TotalCount.Should().Be(changeCount);
                orderRepository.Received(1).UpdateAsync(order);
                gameRepository.Received(1).GetGameByIdAsync(game.Id, false);
            });

        }

        [Test]
        public void SetUserForOrderAsync_Pass_UserAndOrderId()
        {
            var order = Order.Mapper.Map(fakeOrderDto);
            var fakeUser = CreateFakeUser();
            var orderRepository = Substitute.For<IOrderRepositoryAsync>();
                orderRepository.GetByIdAsync(order.Id).Returns(order);
                orderRepository.UpdateAsync(order).Returns(Task.CompletedTask);
            var gameRepository = Substitute.For<IGetGamesRepositoryAsync>();
            var orderService = new OrderService(gameRepository, orderRepository, contextAccessor);

            //Act
            var task = orderService.SetUserForOrderAsync(fakeUser, order.Id);

            Assert.Multiple(() =>
            {
                orderRepository.Received(1).GetByIdAsync(order.Id);
                orderRepository.Received(1).UpdateAsync(order);
            });
        }

        [Test]
        public void GetOrdersForUser_Pass_User()
        {
            var orderDtoCollect = new OrderDTO[] { fakeOrderDto };
            var orderCollection = orderDtoCollect.Select(Order.Mapper.Map).ToArray();
            var user = new User() { Id = Guid.NewGuid().ToString() };
            var orderRepository = Substitute.For<IOrderRepositoryAsync>();
                orderRepository.GetOrdersByUserIdAsync(user.Id).Returns(orderCollection);
            var gameRepository = Substitute.For<IGetGamesRepositoryAsync>();
            var orderService = new OrderService(gameRepository, orderRepository, contextAccessor);

            //Act
            Task<ShortOrderModel[]> taskOrderModels = orderService.GetOrdersForUser(user);
            ShortOrderModel[] shortOrderModels = taskOrderModels.Result;

            //Assert
            Assert.Multiple(() =>
            {
                shortOrderModels.First().Id.Should().Be(fakeOrderDto.Id);
                orderRepository.Received(1).GetOrdersByUserIdAsync(user.Id);              
            });

        }

        [Test]
        public void SetDeliveryAsync_Pass_Delivery()
        {
            byte[] bytes;
            var order = Order.Mapper.Map(fakeOrderDto);
            var delivery = CreateFakeDelivery();
            Cart fakeCart = new Cart(order.Id, order.TotalCount, order.TotalPrice);

            var orderRepository = Substitute.For<IOrderRepositoryAsync>();
                orderRepository.GetByIdAsync(order.Id).Returns(order);
                orderRepository.UpdateAsync(order).Returns(Task.CompletedTask);
            var gameRepository = Substitute.For<IGetGamesRepositoryAsync>();
            var contextAccessor = Substitute.For<IHttpContextAccessor>();
            contextAccessor.HttpContext.Session.TryGetValue("SomeKey", out bytes).ReturnsForAnyArgs(x =>
            {
                x[1] = CreateByteMassivByCartContent(fakeCart);
                return true;
            });
            var orderService = new OrderService(gameRepository, orderRepository, contextAccessor);

            //Act
            Task<OrderModel> taskOrderModel = orderService.SetDeliveryAsync(delivery);
            OrderModel orderModel = taskOrderModel.Result;

            Assert.Multiple(() =>
            {
                orderModel.DeliveryName.Should().Be(delivery.NameDelivery);
                orderRepository.Received(1).UpdateAsync(order);
            });
        }

        [Test]
        public void SetPaymentAsync_Pass_Payment()
        {
            byte[] bytes;
            var order = Order.Mapper.Map(fakeOrderDto);
            var payment = CreateFakePayment();
            Cart fakeCart = new Cart(order.Id, order.TotalCount, order.TotalPrice);

            var orderRepository = Substitute.For<IOrderRepositoryAsync>();
                orderRepository.GetByIdAsync(fakeCart.OrderId).Returns(order);
                orderRepository.UpdateAsync(order).Returns(Task.CompletedTask);
            var gameRepository = Substitute.For<IGetGamesRepositoryAsync>();
            var contextAccessor = Substitute.For<IHttpContextAccessor>();
            contextAccessor.HttpContext.Session.TryGetValue("SomeKey", out bytes).ReturnsForAnyArgs(x =>
            {
                x[1] = CreateByteMassivByCartContent(fakeCart);
                return true;
            });
            var orderService = new OrderService(gameRepository, orderRepository, contextAccessor);

            //Act
            Task<OrderModel> taskOrderModel = orderService.SetPaymentAsync(payment);
            OrderModel orderModel = taskOrderModel.Result;

            Assert.Multiple(() =>
            {
                orderModel.PaymentName.Should().Be(payment.NamePayment);
                orderRepository.Received(1).UpdateAsync(order);
            });
        }


        [Test]
        [TestCase(1, 2, SortOrderStates.OrderDateAsc, true)]
        public void GetOrdersForAdminByUserAsync_WithPassValue_UserName(
         int pageNo, int pageSize, SortOrderStates sortOrder, bool makeOrder)
        {
            var fakeUser = CreateFakeUser();
            fakeOrderDto.User = fakeUser;
            var orderDtoCollect = new OrderDTO[] { fakeOrderDto };
            var iQuerableOrderCollection = orderDtoCollect.AsQueryable();
            var gameRepository = Substitute.For<IGetGamesRepositoryAsync>();
            var orderRepository = Substitute.For<IOrderRepositoryAsync>();
                orderRepository.GetAllOrders().Returns(iQuerableOrderCollection);
            var orderService = new OrderService(gameRepository, orderRepository, contextAccessor);

            // Act
            var taskCollectionShortOrderModel = orderService.GetOrdersForAdminByUserAsync(
                                          pageNo, pageSize, sortOrder, fakeUser.Name, fakeUser.UserName, makeOrder);
            var (shortOrderModelCollect, count) = taskCollectionShortOrderModel.Result;
            //Assert
            Assert.Multiple(() =>
            {
                shortOrderModelCollect.First().UserName.Should().Be(fakeUser.Name);
                orderRepository.Received(1).GetAllOrders();
     
            });
        }

        [Test]
        public void GetOrderForAdminAsync_OrderNotReviewed_Pass_OrderId()
        {
            var order = Order.Mapper.Map(fakeOrderDto);
            order.OrderReviewed = false;
            var gameRepository = Substitute.For<IGetGamesRepositoryAsync>();
            var orderRepository = Substitute.For<IOrderRepositoryAsync>();
                orderRepository.GetByIdAsync(order.Id).Returns(order);
                orderRepository.UpdateAsync(order).Returns(Task.CompletedTask);
            var orderService = new OrderService(gameRepository, orderRepository, contextAccessor);

            Task<OrderModel> task = orderService.GetOrderForAdminAsync(order.Id);
            OrderModel orderModel = task.Result;

            Assert.Multiple(() =>
            {
                orderModel.OrderReviewed.Should().Be(true);
                orderRepository.Received(1).GetByIdAsync(order.Id);
                orderRepository.Received(1).UpdateAsync(order);
            });
        }

        [Test]
        public void GetOrderDetailAsync_Pass_OrderId()
        {
            var order = Order.Mapper.Map(fakeOrderDto);
            var gameRepository = Substitute.For<IGetGamesRepositoryAsync>();
            var orderRepository = Substitute.For<IOrderRepositoryAsync>();
                orderRepository.GetByIdAsync(order.Id).Returns(order);
            var orderService = new OrderService(gameRepository, orderRepository, contextAccessor);

            Task<OrderModel> task = orderService.GetOrderDetailAsync(order.Id);
            OrderModel orderModel = task.Result;

            orderRepository.Received(1).GetByIdAsync(order.Id);
        }

        [Test]
        public void RemoveOrderAsync_Pass_OrderId()
        {
            var order = Order.Mapper.Map(fakeOrderDto);
            var gameRepository = Substitute.For<IGetGamesRepositoryAsync>();
            var orderRepository = Substitute.For<IOrderRepositoryAsync>();
                orderRepository.GetByIdAsync(order.Id).Returns(order);
                orderRepository.RemoveAsync(order).Returns(Task.CompletedTask);
            var orderService = new OrderService(gameRepository, orderRepository, contextAccessor);

            //Act
            var task = orderService.RemoveOrderAsync(order.Id);

            Assert.Multiple(() =>
            {
                orderRepository.Received(1).GetByIdAsync(order.Id);
                orderRepository.Received(1).RemoveAsync(order);
            });
        }

      

        private byte[] CreateByteMassivByCartContent(Cart cart)
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream, Encoding.UTF8, true);
            writer.Write(cart.OrderId);
            writer.Write(cart.TotalCount);
            writer.Write(cart.TotalPrice);
            return stream.ToArray();
        }


        private Delivery CreateFakeDelivery()
        {
            var nameDelivery = Faker.Random.String2(3, 100, russianLettersAndNumbers);
            var description = Faker.Random.String2(3, 200, russianLettersAndNumbers);
            return new Delivery(nameDelivery, description, price, new Dictionary<string, string>());
        }


        private Payment CreateFakePayment()
        {
            var namePayment = Faker.Random.String2(3, 100, russianLettersAndNumbers);
            var description = Faker.Random.String2(3, 300, russianLettersAndNumbers);
            return new Payment(namePayment, description, new Dictionary<string, string>());
        }

        private User CreateFakeUser()
        {
            return new User()
            {
                Id = Guid.NewGuid().ToString(),
                Name = Faker.Random.String2(3, 30, russianLettersAndNumbers),
                UserName = Faker.Person.Email
            };
        }


    }
}
