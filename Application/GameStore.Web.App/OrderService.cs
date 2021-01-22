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
        public bool TryGetModel(out OrderModel model)
        {
            if (TryGetOrder(out Order order))
            {
                model = Map(order);
                return true;
            }
            model = null;
            return false;
        }

        internal bool TryGetOrder(out Order order)
        {
            if (Session.TryGetCart(out Cart cart))
            {
                order = orderRepository.GetById(cart.OrderId);
                return true;
            }
            order = null;
            return false;
        }

        internal OrderModel Map(Order order)
        {
            var games = GetGames(order);
            var items = order.Items.Join(games, orderItem => orderItem.GameId,
                                                game => game.Id,
                                                (orderItem, game) => new OrderItemModel
                                                {
                                                     GameId = game.Id,
                                                     Category = game.Category,
                                                     Count = orderItem.Count,
                                                     Name = game.Name,
                                                     Price = orderItem.Price,
                                                     Publisher = game.Publisher,
                                                     ImageData = game.ImageData
                                                });
            return new OrderModel
            {
                Id = order.Id,
                OrderItems = items.ToArray(),
                TotalCount = order.TotalCount,
                TotalPrice = order.TotalPrice,
             };
        }


        internal IEnumerable<Game> GetGames(Order order)
        {
            var gameIds = order.Items.Select(item => item.GameId);

            return gameRepository.GetGamesByIds(gameIds);
        }

        public OrderModel AddGame(int gameId, int count)
        {
            if (count < 1)
                throw new InvalidOperationException("The number of added books cannot be less than one!");

            if (!TryGetOrder(out Order order))
                order = orderRepository.Create();

            AddOrUpdateGame(order, gameId, count);
            UpdateSession(order);

            return Map(order);
        }

        internal void AddOrUpdateGame(Order order, int gameId, int count)
        {
            var game = gameRepository.GetGameById(gameId);
            if (order.TryGetOrderItem(gameId, out OrderItem orderItem))
                orderItem.Count += count;
            else
                order.AddOrderItem(game.Id, game.Price, count);

            orderRepository.Update(order);
        }

        public OrderModel RemoveGame(int gameId)
        {
            var order = GetOrder();
            order.RemoveOrderItem(gameId);

            orderRepository.Update(order);
            UpdateSession(order);

            return Map(order);
        }

        public Order GetOrder()
        {
            if (TryGetOrder(out Order order))
                return order;

            throw new InvalidOperationException("Session is empty.");
        }

        internal void UpdateSession(Order order)
        {
            var cart = new Cart(order.Id, order.TotalCount, order.TotalPrice);
            Session.Set(cart);
        }

        public OrderModel UpdateGame(int gameId, int count)
        {
            var order = GetOrder();
            order.GetItem(gameId).Count = count;

            orderRepository.Update(order);
            UpdateSession(order);

            return Map(order);
        }
    }
}
