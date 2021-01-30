using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Data.EF
{
    public class OrderRepository: IOrderRepository
    {
        private readonly ContextDBFactory dbFactory;

        public OrderRepository(ContextDBFactory dbContextFactory)
        {
            this.dbFactory = dbContextFactory;
        }

        public Order GetById(int id)
        {
            var db = dbFactory.Create(typeof(OrderRepository));

            var orderDto = db.Orders.Include(order => order.Items)
                                    .ThenInclude(orderItem => orderItem.Game)
                                    .ThenInclude(game => game.Category)
                                    .Include(order => order.User)
                                    .Single(order => order.Id == id);
      
            return Order.Mapper.Map(orderDto);
        }
        public async Task<Order> GetByIdAsync(int id)
        {
            var db = dbFactory.Create(typeof(OrderRepository));

            var orderDto = await db.Orders.Include(order => order.Items)
                                    .ThenInclude(orderItem => orderItem.Game)
                                    .ThenInclude(game => game.Category)
                                    .Include(order => order.User)
                                    .SingleAsync(order => order.Id == id);

            return Order.Mapper.Map(orderDto);
        }


        public Order Create()
        {
            var db = dbFactory.Create(typeof(OrderRepository));

            var orderDto = Order.DtoFactory.Create();
            db.Orders.Add(orderDto);
            db.SaveChanges();

            return Order.Mapper.Map(orderDto);
        }
        public async Task<Order> CreateAsync()
        {
            var db = dbFactory.Create(typeof(OrderRepository));

            var orderDto = Order.DtoFactory.Create();
            await db.Orders.AddAsync(orderDto);
            await db.SaveChangesAsync();

            return Order.Mapper.Map(orderDto);
        }



        public void Update(Order order)
        {
            var db = dbFactory.Create(typeof(OrderRepository));
            db.SaveChanges();
        }
        public async Task UpdateAsync(Order order)
        {
            var db = dbFactory.Create(typeof(OrderRepository));
            await db.SaveChangesAsync();
        }



    }
}
