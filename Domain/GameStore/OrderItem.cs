using GameStore.DataEF;
using System;

namespace GameStore
{
    //ValueObject OrderItem
    public class OrderItem
    {
        private readonly OrderItemDTO dto;

        internal OrderItem(OrderItemDTO dto)
        {
            this.dto = dto;
        }

        public int Count => dto.Count;
        public decimal Price => dto.Price;
        public Order Order => Order.Mapper.Map(dto.Order);
       
        public Game Game
        {
            get
            {
                if (dto.Game != null)
                    return Game.Mapper.Map(dto.Game);
                else
                    return null;
            }
        }

        public void ChangeCountByOneItem(int count)
        {
            if (count == 1 && Count < 9 || count == -1 && Count > 1)
                dto.Count += count;
        }

        public void ChangeCount(int count)
        {
            if (count > 0 && count < 10)
                dto.Count = count;
        }
       
        private static void ThrowIfCountGreaterThanZero(int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException("Count must be greater than zero.");
        }

        public static class DtoFactory
        {
            public static OrderItemDTO Create(OrderDTO order, GameDTO game, int count)
            {
                if (order == null)
                    throw new ArgumentNullException(nameof(order));

                ThrowIfCountGreaterThanZero(count);

                if (game == null)
                    throw new ArgumentNullException(nameof(game));

                return new OrderItemDTO
                {
                    Order = order,
                    Game = game,
                    Price = game.Price,
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
