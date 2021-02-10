 using GameStore.DataEF;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Web.App
{
    public class OrderService
    {
        private readonly IGameRepository gameRepository;
        private readonly IOrderRepository orderRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        protected ISession Session => httpContextAccessor.HttpContext.Session;

        public OrderService(IGameRepository gameRepository,
                            IOrderRepository orderRepository,
                            IHttpContextAccessor httpContextAccessor)
        {
            this.gameRepository = gameRepository;
            this.orderRepository = orderRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<(bool hasValue, OrderModel model)> TryGetModelAsync()
        {
            var (hasValue, order) = await TryGetOrderAsync();
            if (hasValue)
                return (true, Map(order));

            return (false, null);
        }

        internal async Task<(bool hasValue, Order order)> TryGetOrderAsync()
        {
            if (Session.TryGetCart(out Cart cart))
            {
                var order = await orderRepository.GetByIdAsync(cart.OrderId);
                return (true, order);
            }
            return (false, null);
        }

        internal OrderModel Map(Order order)
        {
            var orderItemModel = new List<OrderItemModel>();
            foreach (OrderItem orderItem in order.Items)
            {
                orderItemModel.Add(new OrderItemModel()
                {
                    GameId = orderItem.Game.Id,
                    GameName = orderItem.Game.Name,
                    Publisher = orderItem.Game.Publisher,
                    Category = orderItem.Game.Category.Name,
                    Count = orderItem.Count,
                    Price = orderItem.Price,
                    ImageData = orderItem.Game.ImageData
                });
            }

            return new OrderModel
            {
                Id = order.Id,
                OrderDateAndTime = order.DateOfOrder,
                OrderItems = orderItemModel.ToArray(),
                TotalCount = order.TotalCount,
                TotalPrice = order.TotalPrice,
                DeliveryName = order.Delivery?.NameDelivery,
                DeliveryDescription = order.Delivery?.Description,
                DeliveryPrice = order.Delivery?.DeliveryPrice ?? 0m,
                PaymentDescription = order.Payment?.Description,
                UserName = order.User?.UserName,
                UserCity = order.User?.City,
                UserAddress = order.User?.Address,
                UserEmail = order.User?.Email,
                                
            };
        }


        internal async Task<IEnumerable<Game>> GetGamesAsync(Order order)
        {
            var gameIds = order.Items.Select(orderItem => orderItem.Game.Id);

            return await gameRepository.GetGamesByIdsAsync(gameIds);
        }

        public async Task<OrderModel> AddGameAsync(int gameId, int count)
        {
            if (count < 1)
                throw new InvalidOperationException("The number of added books cannot be less than one!");
            var (hasValue, order) = await TryGetOrderAsync();
            if (!hasValue)
                order = orderRepository.Create();

            await AddOrUpdateGameAsync(order, gameId, count);
            UpdateSession(order);

            return Map(order);
        }

        internal async Task AddOrUpdateGameAsync(Order order, int gameId, int count)
        {
            var game = await gameRepository.GetGameByIdAsync(gameId);
            if (order.Items.TryGet(game, out OrderItem orderItem))
                orderItem.Count += count;
            else
                order.Items.Add(game, count);

            await orderRepository.UpdateAsync(order);
        }

        public async Task<OrderModel> RemoveGameAsync(int gameId)
        {
            var order = await GetOrderAsync();
            order.Items.Remove(gameId);

            await orderRepository.UpdateAsync(order);
            UpdateSession(order);
            if (order.Items.Count == 0)
            // if (order.Items.Count == 0 && order.UserId == null)
            {
                await orderRepository.RemoveAsync(order);
                Session.RemoveCart();
            }
            return Map(order);
        }


        internal void UpdateSession(Order order)
        {
            var cart = new Cart(order.Id, order.TotalCount, order.TotalPrice);
            Session.Set(cart);
        }

        public async Task<Order> GetOrderAsync()
        {
            var (hasValue, order) = await TryGetOrderAsync();
            if (hasValue)
                return order;

            throw new InvalidOperationException("Session is empty");
        }

        public async Task<OrderModel> UpdateGameAsync(int gameId, int count)
        {
            var order = await GetOrderAsync();
            var game = await gameRepository.GetGameByIdAsync(gameId);
            order.Items.Get(game).Count = count;

            await orderRepository.UpdateAsync(order);
            UpdateSession(order);

            return Map(order);
        }

        public async Task SetUserForOrderAsync(User user, int orderId)
        {
            var order = await orderRepository.GetByIdAsync(orderId);
            order.UserId = user.Id;
            await orderRepository.UpdateAsync(order);
        }


        public async Task<Order[]> GetOrdersForUser(User user)
        {
            return await orderRepository.GetOrdersByUserIdAsync(user.Id);
        }

        public async Task<OrderModel> SetDeliveryAsync(Delivery delivery)
        {
            var order = await GetOrderAsync();
            order.Delivery = delivery;
            await orderRepository.UpdateAsync(order);
            UpdateSession(order);
            return Map(order);
        }


        public async Task<OrderModel> SetPaymentAsync(Payment payment)
        {
            var order = await GetOrderAsync();
            order.Payment = payment;
            await orderRepository.UpdateAsync(order);
            Session.RemoveCart();   //
            return Map(order);
        }



    }
}
