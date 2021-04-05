using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;


namespace GameStore.Web.App
{
    public class OrderMemoryService
    {
        private readonly IGameMemoryRepository gameRepository;
        private readonly IOrderMemoryRepository orderRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        protected ISession Session => httpContextAccessor.HttpContext.Session;

        public OrderMemoryService(IGameMemoryRepository gameRepository,
                            IOrderMemoryRepository orderRepository,
                            IHttpContextAccessor httpContextAccessor)
        {
            this.gameRepository = gameRepository;
            this.orderRepository = orderRepository;
            this.httpContextAccessor = httpContextAccessor;
        }
        public bool TryGetModel(out OrderModel model)
        {
            if (TryGetOrder(out OrderMemoryEntity order))
            {
                model = Map(order);
                return true;
            }
            model = null;
            return false;
        }

        internal bool TryGetOrder(out OrderMemoryEntity order)
        {
            if (Session.TryGetCart(out Cart cart))
            {
                order = orderRepository.GetById(cart.OrderId);
                return true;
            }
            order = null;
            return false;
        }

        internal OrderModel Map(OrderMemoryEntity order)
        {
            var games = GetGames(order);
            var items = order.Items.Join(games, orderItem => orderItem.GameId,
                                                game => game.Id,
                                                (orderItem, game) => new OrderItemModel
                                                {
                                                     GameId = game.Id,
                                                     Category = game.Category,
                                                     Count = orderItem.Count,
                                                     GameName = game.Name,
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


        internal IEnumerable<GameMemoryEntity> GetGames(OrderMemoryEntity order)
        {
            var gameIds = order.Items.Select(item => item.GameId);

            return gameRepository.GetGamesByIds(gameIds);
        }

        public OrderModel AddGame(int gameId, int count)
        {
            if (count < 1)
                throw new InvalidOperationException("The number of added books cannot be less than one!");

            if (!TryGetOrder(out OrderMemoryEntity order))
                order = orderRepository.Create();

            AddOrUpdateGame(order, gameId, count);
            UpdateSession(order);

            return Map(order);
        }

        internal void AddOrUpdateGame(OrderMemoryEntity order, int gameId, int count)
        {
            var game = gameRepository.GetGameById(gameId);
            if (order.TryGetOrderItem(gameId, out OrderItemMemoryEntity orderItem))
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

        public OrderMemoryEntity GetOrder()
        {
            if (TryGetOrder(out OrderMemoryEntity order))
                return order;

            throw new InvalidOperationException("Session is empty.");
        }

        internal void UpdateSession(OrderMemoryEntity order)
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
