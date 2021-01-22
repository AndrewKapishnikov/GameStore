using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Web.App
{
    public class Cart
    {
        public int OrderId { get; }
        public decimal TotalPrice { get; }
        public int TotalCount { get; }
        public Cart(int orderId, int totalCount, decimal totalPrice)
        {
            OrderId = orderId;
            TotalCount = totalCount;
            TotalPrice = totalPrice;
        }
    }
}
