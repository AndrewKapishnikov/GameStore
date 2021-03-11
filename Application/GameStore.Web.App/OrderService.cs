using GameStore.DataEF;
using GameStore.Web.App.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
        
        internal async Task<IEnumerable<Game>> GetGamesAsync(Order order)
        {
            var gameIds = order.Items.Select(orderItem => orderItem.Game.Id);

            return await gameRepository.GetGamesByIdsAsync(gameIds);
        }

        public async Task<OrderModel> AddGameAsync(int gameId, int count)
        {
            var (hasValue, order) = await TryGetOrderAsync();
            if (!hasValue)
                order = orderRepository.Create();

            await AddOrUpdateGameAsync(order, gameId, count);
            UpdateSession(order);

            return Map(order);
        }

        internal async Task AddOrUpdateGameAsync(Order order, int gameId, int count)
        {
            var game = await gameRepository.GetGameByIdAsync(gameId, false);
            if (order.Items.TryGet(game, out OrderItem orderItem))
            {
                if (count == 1 && orderItem.Count < 9 || count == -1 && orderItem.Count > 1)
                    orderItem.Count += count;
            }
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
            var game = await gameRepository.GetGameByIdAsync(gameId, false);
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


        public async Task<ShortOrderModel[]> GetOrdersForUser(User user)
        {
           var orders = await orderRepository.GetOrdersByUserIdAsync(user.Id);
           return orders.Select(ShortOrderMap).ToArray();
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

        public async Task<(IReadOnlyCollection<ShortOrderModel>, int)> GetOrdersForAdminByUserAsync(int pageNo, int pageSize, SortOrderStates sortOrder,
                                                                                                    string userName, string userEmail, bool makeOrder)
        {
            IQueryable<OrderDTO> orders = orderRepository.GetAllOrders();
            if (!String.IsNullOrEmpty(userName))
            {
                orders = orders.Where(p => p.User.Name.Contains(userName) || p.User.Surname.Contains(userName));
            }
            if (!String.IsNullOrEmpty(userEmail))
            {
                orders = orders.Where(p => p.User.UserName.Contains(userEmail));
            }
            if (makeOrder)
                orders = orders.Where(p => p.UserId != null && p.DeliveryName != null && p.PaymentName != null);
            else
            {
                orders = orders.Where(p => p.UserId == null || p.DeliveryName == null || p.PaymentName == null);
            }

            switch (sortOrder)
            {
                case SortOrderStates.OrderDateDesc:
                    orders = orders.OrderByDescending(p => p.DateOfOrder);
                    break;
                case SortOrderStates.UserEmailAsc:
                    orders = orders.OrderBy(p => p.User.UserName);
                    break;
                case SortOrderStates.UserEmailDesc:
                    orders = orders.OrderByDescending(p => p.User.UserName);
                    break;
                case SortOrderStates.UserNameAsc:
                    orders = orders.OrderBy(p => p.User.Name);
                    break;
                case SortOrderStates.UserNameDesc:
                    orders = orders.OrderByDescending(p => p.User.Name);
                    break;
                default:
                    orders = orders.OrderBy(p => p.DateOfOrder);
                    break;
            }

            var count = await orders.CountAsync();
            var ordersFinal = await orders.Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
            var ordersModel = ordersFinal.Select(Order.Mapper.Map).Select(ShortOrderMap).ToArray();

            return (ordersModel, count);

        }

        public async Task<OrderModel> GetOrderForAdminAsync(int orderId)
        {
            var order = await orderRepository.GetByIdAsync(orderId);
            if (!order.OrderReviewed)
            {
                order.OrderReviewed = true;
                await orderRepository.UpdateAsync(order);
            }
            return Map(order);
        }

        public async Task<OrderModel> GetOrderDetailAsync(int orderId)
        {
            var order = await orderRepository.GetByIdAsync(orderId);
            return Map(order);
        }


        public async Task RemoveOrderAsync(int orderId)
        {
            var order = await orderRepository.GetByIdAsync(orderId);
            await orderRepository.RemoveAsync(order);
            Session.RemoveCart();
        }

        internal OrderModel Map(Order order)
        {
            var orderItemModel = new List<OrderItemModel>();
            foreach (OrderItem orderItem in order.Items)
            {
                orderItemModel.Add(new OrderItemModel()
                {
                    GameId = orderItem.Game?.Id ?? 0,
                    GameName = orderItem.Game?.Name,
                    Publisher = orderItem.Game?.Publisher,
                    Category = orderItem.Game?.Category?.Name,
                    Count = orderItem.Count,
                    Price = orderItem.Price,
                    ImageData = orderItem.Game?.ImageData
                });
            }

            return new OrderModel
            {
                Id = order.Id,
                OrderDateAndTime = order.DateOfOrder,
                OrderReviewed = order.OrderReviewed,
                OrderItems = orderItemModel.ToArray(),
                TotalCount = order.TotalCount,
                TotalPrice = order.TotalPrice,
                DeliveryName = order.Delivery?.NameDelivery,
                DeliveryDescription = order.Delivery?.Description,
                DeliveryPrice = order.Delivery?.DeliveryPrice ?? 0m,
                PaymentDescription = order.Payment?.Description,
                PaymentName = order.Payment?.NamePayment,
                PaymentParameters = order.Payment?.Parameters,
                UserName = string.Concat(order.User?.Name, " ", order.User?.Surname),
                UserCity = order.User?.City,
                UserAddress = order.User?.Address,
                UserEmail = order.User?.Email,
                UserPhone = order.User?.PhoneNumber
            };
        }

        internal ShortOrderModel ShortOrderMap(Order order)
        {
            return new ShortOrderModel
            {
                Id = order.Id,
                OrderDateAndTime = order.DateOfOrder,
                TotalCount = order.TotalCount,
                TotalPrice = order.TotalPrice,
                UserEmail = order.User?.UserName,
                UserName = order.User?.Name,
                UserSurname = order.User?.Surname,
                OrderReviewed = order.OrderReviewed
            };
        }

    }
}
