﻿using GameStore.DataEF;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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

        public async Task<Order[]> GetOrdersByUserIdAsync(string userId)
        {
            var db = dbFactory.Create(typeof(OrderRepository));

            var ordersDto = await db.Orders.Include(order => order.Items)
                                    .ThenInclude(orderItem => orderItem.Game)
                                    .ThenInclude(game => game.Category)
                                    .Include(order => order.User)
                                    .Where(order => order.UserId == userId 
                                           && order.PaymentDescription != null 
                                           && order.DeliveryDescription != null)
                                    .OrderByDescending(order => order.DateOfOrder)
                                    .ToArrayAsync();
            return ordersDto.Select(Order.Mapper.Map).ToArray();
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

        public async Task RemoveAsync(Order order)
        {
            var db = dbFactory.Create(typeof(OrderRepository));

            db.Orders.Remove(Order.Mapper.Map(order));
            await db.SaveChangesAsync();
            
        }

        public IQueryable<OrderDTO> GetAllOrders()
        {
            var db = dbFactory.Create(typeof(OrderRepository));
            IQueryable<OrderDTO> orders = db.Orders.Include(order => order.User)
                                                   .Include(order => order.Items);
            return orders;
        }

    }
}
