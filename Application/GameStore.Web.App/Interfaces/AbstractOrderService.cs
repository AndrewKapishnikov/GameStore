using GameStore.DataEF;
using GameStore.Web.App.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.Web.App.Interfaces
{
    public abstract class AbstractOrderService
    {
        protected IGameRepository gameRepository;
        protected IOrderRepository orderRepository;
        protected IHttpContextAccessor httpContextAccessor;
        protected ISession Session => httpContextAccessor.HttpContext?.Session;

        public abstract Task<(bool hasValue, OrderModel model)> TryGetModelAsync();
        public abstract Task<IEnumerable<Game>> GetGamesAsync(Order order);
        public abstract Task<OrderModel> AddGameAsync(int gameId, int count);
        public abstract Task<OrderModel> RemoveGameAsync(int gameId);
        public abstract Task<Order> GetOrderAsync();
        public abstract Task<OrderModel> UpdateGameAsync(int gameId, int count);
        public abstract Task SetUserForOrderAsync(User user, int orderId);
        public abstract Task<ShortOrderModel[]> GetOrdersForUser(User user);
        public abstract Task<OrderModel> SetDeliveryAsync(Delivery delivery);
        public abstract Task<OrderModel> SetPaymentAsync(Payment payment);
        public abstract Task<(IReadOnlyCollection<ShortOrderModel>, int)> GetOrdersForAdminByUserAsync(int pageNo,
                        int pageSize, SortOrderStates sortOrder, string userName, string userEmail, bool makeOrder);
        public abstract Task<OrderModel> GetOrderForAdminAsync(int orderId);
        public abstract Task<OrderModel> GetOrderDetailAsync(int orderId);
        public abstract Task RemoveOrderAsync(int orderId);

        public static OrderModel Map(Order order)
        {
            var orderItemModel = new List<OrderItemModel>();
            foreach (OrderItem orderItem in order.Items)
            {
                orderItemModel.Add(new OrderItemModel()
                {
                    GameId = orderItem.Game?.Id ?? 0,
                    GameName = orderItem.Game?.Name,
                    Publisher = orderItem.Game?.GameDescription.Publisher,
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

        public static ShortOrderModel ShortOrderMap(Order order)
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
