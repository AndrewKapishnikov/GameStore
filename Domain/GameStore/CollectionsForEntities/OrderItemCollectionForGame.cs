using GameStore.DataEF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace GameStore
{
    public class OrderItemCollectionForGame : IReadOnlyCollection<OrderItem>
    {

        private readonly List<OrderItem> items;

        public OrderItemCollectionForGame(IEnumerable<OrderItemDTO> orderDto)
        {
            if (orderDto == null)
                throw new ArgumentNullException(nameof(orderDto));


            items = orderDto.Select(OrderItem.Mapper.Map).ToList();
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
    }
}
