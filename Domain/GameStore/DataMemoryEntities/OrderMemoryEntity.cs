using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStore
{
    public class OrderMemoryEntity
    {
        public int Id { get; }

        private List<OrderItemMemoryEntity> items;

        public OrderMemoryEntity(int id, IEnumerable<OrderItemMemoryEntity> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            Id = id;
            this.items = new List<OrderItemMemoryEntity>(items);
        }

        public IReadOnlyCollection<OrderItemMemoryEntity> Items => items;
      

        public int TotalCount => items.Sum(item => item.Count);
        public decimal TotalPrice => items.Sum(item => item.Price * item.Count);

        public OrderItemMemoryEntity GetItem(int gameId)
        {
            int index = items.FindIndex(item => item.GameId == gameId);
            if (index == -1)
                ThrowGameException("Game not found!", gameId);
            
            return items[index];
        }

        public bool TryGetOrderItem(int gameId, out OrderItemMemoryEntity orderItem)
        {
            int index = items.FindIndex(item => item.GameId == gameId);
            if (index == -1)
            {
                orderItem = null;
                return false;
            }
            else
            {
                orderItem = items[index];
                return true;
            }
                
        }
        public void AddOrderItem(int gameId, decimal price,int count)
        {
            if (TryGetOrderItem(gameId, out OrderItemMemoryEntity orderItem))
                throw new InvalidOperationException("Game already exists!");

            items.Add(new OrderItemMemoryEntity(gameId, count, price));
        }

        public void RemoveOrderItem(int gameId)
        {
            int index = items.FindIndex(item => item.GameId == gameId);

            if (index == -1)
                ThrowGameException("Order does not contain item with such Id.", gameId);

            items.RemoveAt(index);
        }


        private void ThrowGameException(string message, int gameId)
        {
            var exception = new InvalidOperationException(message);

            exception.Data["GameId"] = gameId;

            throw exception;
        }
    }
}
