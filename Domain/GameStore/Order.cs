using GameStore.DataEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore
{
    public class Order
    {
        private readonly OrderDTO dto; 
        public int Id => dto.Id;
        public OrderItemCollectionForOrder Items { get; }
        public Order(OrderDTO dto)
        {
            this.dto = dto;
            Items = new OrderItemCollectionForOrder(dto);
           
        }

        public User User
        {
            get => dto.User;
            set
            {
                if (value == null)
                    throw new ArgumentException(nameof(User));
                dto.User = value;
            }
        }
        public int TotalCount => Items.Sum(item => item.Count);

        public decimal TotalPrice => Items.Sum(item => item.Price * item.Count);

        public static class DtoFactory
        {
            public static OrderDTO Create() => new OrderDTO() { DateOfOrder = DateTime.UtcNow};
        }

        public static class Mapper
        {
            public static Order Map(OrderDTO dto) => new Order(dto);

            public static OrderDTO Map(Order domain) => domain.dto;
        }

    }

   
}
