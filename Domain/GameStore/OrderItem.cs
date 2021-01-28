using GameStore.DataEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore
{
    public class OrderItem
    {
        private readonly OrderItemDTO dto;

        internal OrderItem(OrderItemDTO dto)
        {
            this.dto = dto;
        }
        public int Id => dto.Id;
        public Game Game
        {
            get => Game.Mapper.Map(dto.Game);
        }
        public Order Order
        {
            get => Order.Mapper.Map(dto.Order);
        }

        public int Count
        {
            get { return dto.Count; }
            set
            {
                ThrowIfCountGreaterThanZero(value);
                dto.Count = value;
            }
        }

        public decimal Price
        {
            get => dto.Price;
            set => dto.Price = value;
        }
        private static void ThrowIfCountGreaterThanZero(int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException("Count must be greater than zero.");
        }

        public static class DtoFactory
        {
            public static OrderItemDTO Create(OrderDTO order, GameDTO game, decimal price, int count)
            {
                if (order == null)
                    throw new ArgumentNullException(nameof(order));

                ThrowIfCountGreaterThanZero(count);

                return new OrderItemDTO
                {
                    Order = order,
                    Game = game,
                    Price = price,
                    Count = count
                };
            }
        }

        public static class Mapper
        {
            public static OrderItem Map(OrderItemDTO dto) => new OrderItem(dto);

            public static OrderItemDTO Map(OrderItem domain) => domain.dto;
        }


    }
}
