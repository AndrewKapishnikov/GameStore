using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.MemoryStorage
{
    public class OrderMemoryRepository : IOrderMemoryRepository
    {
        private readonly List<OrderMemoryStorage> orders = new List<OrderMemoryStorage>();
        public OrderMemoryStorage Create()
        {
            int nextId = orders.Count + 1;
            var order = new OrderMemoryStorage(nextId, new OrderItemMemoryStorage[0]);

            orders.Add(order);

            return order;
        }

        public OrderMemoryStorage GetById(int id)
        {
            return orders.Single(order => order.Id == id);
        }

        public void Update(OrderMemoryStorage order){;}
      
    }
}
