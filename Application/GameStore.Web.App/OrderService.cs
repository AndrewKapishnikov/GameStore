using GameStore.DataEF;
using GameStore.Web.App.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using GameStore.Web.App.Interfaces;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("GameStore.UnitTests")]

namespace GameStore.Web.App
{
    public class OrderService : AbstractOrderService
    {
        public OrderService(IGetGamesRepositoryAsync gameRepository,
                            IOrderRepositoryAsync orderRepository,
                            IHttpContextAccessor httpContextAccessor)
        {
            this.gameRepository = gameRepository;
            this.orderRepository = orderRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        public override async Task<(bool hasValue, OrderModel model)> TryGetModelAsync()
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

        //This method is not used in the project yet.
        public override async Task<IEnumerable<Game>> GetGamesAsync(Order order)
        {
            var gameIds = order.Items.Select(orderItem => orderItem.Game.Id);

            return await gameRepository.GetGamesByIdsAsync(gameIds);
        }

        /// <summary>
        /// Add Game in Order. If the game already exists in the order, change the count of games in OrderItem. 
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override async Task<OrderModel> AddGameAsync(int gameId, int count)
        {
            var (hasValue, order) = await TryGetOrderAsync();
            if (!hasValue)
                order = await orderRepository.CreateAsync();

            await AddOrUpdateGameAsync(order, gameId, count);
            UpdateSession(order);

            return Map(order);
        }

        internal async Task AddOrUpdateGameAsync(Order order, int gameId, int count)
        {
            var game = await gameRepository.GetGameByIdAsync(gameId, false);
            if (order.Items.TryGet(game, out OrderItem orderItem))
            {
                orderItem.ChangeCountByOneItem(count);
            }
            else
                order.Items.Add(game, count);

            await orderRepository.UpdateAsync(order);
        }

        /// <summary>
        /// Remove Game from Order.
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public override async Task<OrderModel> RemoveGameAsync(int gameId)
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

        public override async Task<Order> GetOrderAsync()
        {
            var (hasValue, order) = await TryGetOrderAsync();
            if (hasValue)
                return order;

            throw new InvalidOperationException("Session is empty");
        }

        public override async Task<OrderModel> UpdateGameAsync(int gameId, int count)
        {
            var order = await GetOrderAsync();
            var game = await gameRepository.GetGameByIdAsync(gameId, false);
            order.Items.Get(game).ChangeCount(count);

            await orderRepository.UpdateAsync(order);
            UpdateSession(order);

            return Map(order);
        }

        public override async Task SetUserForOrderAsync(User user, int orderId)
        {
            var order = await orderRepository.GetByIdAsync(orderId);
            order.UserId = user.Id;
            await orderRepository.UpdateAsync(order);
        }


        public override async Task<ShortOrderModel[]> GetOrdersForUser(User user)
        {
           var orders = await orderRepository.GetOrdersByUserIdAsync(user.Id);
           return orders.Select(ShortOrderMap).ToArray();
        }

        public override async Task<OrderModel> SetDeliveryAsync(Delivery delivery)
        {
            var order = await GetOrderAsync();
            order.Delivery = delivery;
            await orderRepository.UpdateAsync(order);
            UpdateSession(order);
            return Map(order);
        }

        public override async Task<OrderModel> SetPaymentAsync(Payment payment)
        {
            var order = await GetOrderAsync();
            order.Payment = payment;
            await orderRepository.UpdateAsync(order);
            Session.RemoveCart();   //
            return Map(order);
        }

        public override async Task<(IReadOnlyCollection<ShortOrderModel>, int)> GetOrdersForAdminByUserAsync(int pageNo, int pageSize, SortOrderStates sortOrder,
                                                                                                    string userName, string userEmail, bool makeOrder)
        {
            IQueryable<OrderDTO> orders = orderRepository.GetAllOrders();
            
            //userName can be Name and Surname here
            if (!string.IsNullOrEmpty(userName))
            {
                string[] nameSurname = SplitParameterIntoNameAndSurname(ref userName);
                if (nameSurname.Length == 1)
                    orders = orders.Where(p => p.User.Name.Contains(userName) || p.User.Surname.Contains(userName));
                else
                    orders = orders.Where(p => p.User.Name.Contains(nameSurname[0]) && p.User.Surname.Contains(nameSurname[1]));
            }
            if (!string.IsNullOrEmpty(userEmail))
            {
                orders = orders.Where(p => p.User.UserName.Contains(userEmail));
            }
            if (makeOrder)
                orders = orders.Where(p => p.UserId != null && p.DeliveryName != null && p.PaymentName != null);
            else
                orders = orders.Where(p => p.UserId == null || p.DeliveryName == null || p.PaymentName == null);
   
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

            var count = orders.Count();
            var ordersFinal = orders.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();
            var ordersModel = ordersFinal.Select(Order.Mapper.Map).Select(ShortOrderMap).ToArray();

            return await Task.FromResult((ordersModel, count));

        }

        public override async Task<OrderModel> GetOrderForAdminAsync(int orderId)
        {
            var order = await orderRepository.GetByIdAsync(orderId);
            if (!order.OrderReviewed)
            {
                order.OrderReviewed = true;
                await orderRepository.UpdateAsync(order);
            }
            return Map(order);
        }

        public override async Task<OrderModel> GetOrderDetailAsync(int orderId)
        {
            var order = await orderRepository.GetByIdAsync(orderId);
            return Map(order);
        }

        public override async Task RemoveOrderAsync(int orderId)
        {
            var order = await orderRepository.GetByIdAsync(orderId);
            await orderRepository.RemoveAsync(order);
            Session.RemoveCart();
        }
       

    }
}
