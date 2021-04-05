using System.Collections.Generic;
using System.Linq;


namespace GameStore.MemoryStorage
{
    public class OrderMemoryRepository : IOrderMemoryRepository
    {
        private readonly List<OrderMemoryEntity> orders = new List<OrderMemoryEntity>();
        public OrderMemoryEntity Create()
        {
            int nextId = orders.Count + 1;
            var order = new OrderMemoryEntity(nextId, new OrderItemMemoryEntity[0]);

            orders.Add(order);

            return order;
        }

        public OrderMemoryEntity GetById(int id)
        {
            return orders.Single(order => order.Id == id);
        }

        public void Update(OrderMemoryEntity order){;}
      
    }
}
