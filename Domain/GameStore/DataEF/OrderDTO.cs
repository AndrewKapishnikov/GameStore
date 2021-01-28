using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.DataEF
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public DateTime DateOfOrder { get; set; }

        public IList<OrderItemDTO> Items { get; set; } = new List<OrderItemDTO>();
        public string UserId { get; set; }   
        public User User { get; set; }
    }
}
