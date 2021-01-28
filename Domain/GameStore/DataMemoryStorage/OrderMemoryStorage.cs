using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore
{
    public class OrderMemoryStorage
    {
        public int Id { get; }

        private List<OrderItemMemoryStorage> items;

        public OrderMemoryStorage(int id, IEnumerable<OrderItemMemoryStorage> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            Id = id;
            this.items = new List<OrderItemMemoryStorage>(items);
        }

        public IReadOnlyCollection<OrderItemMemoryStorage> Items => items;
      

        public int TotalCount => items.Sum(item => item.Count);
        public decimal TotalPrice => items.Sum(item => item.Price * item.Count);

        public OrderItemMemoryStorage GetItem(int gameId)
        {
            int index = items.FindIndex(item => item.GameId == gameId);
            if (index == -1)
                ThrowGameException("Game not found!", gameId);
            
            return items[index];
        }

        public bool TryGetOrderItem(int gameId, out OrderItemMemoryStorage orderItem)
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
            if (TryGetOrderItem(gameId, out OrderItemMemoryStorage orderItem))
                throw new InvalidOperationException("Game already exists!");

            items.Add(new OrderItemMemoryStorage(gameId, count, price));
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
