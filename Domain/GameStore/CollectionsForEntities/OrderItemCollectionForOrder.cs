using GameStore.DataEF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore
{
    public class OrderItemCollectionForOrder: IReadOnlyCollection<OrderItem>
    {
        private readonly OrderDTO orderDto;
        private readonly List<OrderItem> items;

        public OrderItemCollectionForOrder(OrderDTO orderDto)
        {
            if (orderDto == null)
                throw new ArgumentNullException(nameof(orderDto));

            this.orderDto = orderDto;

            items = orderDto.Items.Select(OrderItem.Mapper.Map).ToList();
        }

        public int Count => items.Count;
        public IEnumerator<OrderItem> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (items as IEnumerable).GetEnumerator();
        }

        public OrderItem Get(Game game)
        {
            if (TryGet(game, out OrderItem orderItem))
                return orderItem;

            throw new InvalidOperationException("Book not found.");
        }

        public bool TryGet(Game game, out OrderItem orderItem)
        {
            var index = items.FindIndex(item => item.Game.Id == game.Id);
            if (index == -1)
            {
                orderItem = null;
                return false;
            }

            orderItem = items[index];
            return true;
        }

        public OrderItem Add(Game game, decimal price, int count)
        {
            if (TryGet(game, out OrderItem orderItem))
                throw new InvalidOperationException("Game already exists.");

            var orderItemDto = OrderItem.DtoFactory.Create(orderDto, Game.Mapper.Map(game), price, count);
            orderDto.Items.Add(orderItemDto);  //Add in database

            orderItem = OrderItem.Mapper.Map(orderItemDto);
            items.Add(orderItem);

            return orderItem;
        }

        public void Remove(Game game)
        {
            var index = items.FindIndex(item => item.Game.Id == game.Id);
            if (index == -1)
                throw new InvalidOperationException("Can't find such game to remove from order.");

            orderDto.Items.RemoveAt(index);
            items.RemoveAt(index);
        }
    }
}
