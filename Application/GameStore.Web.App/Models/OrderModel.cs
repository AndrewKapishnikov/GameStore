using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Web.App
{
    public class OrderModel
    {
        public int Id { get; set; }
        public DateTime OrderDateAndTime { get; set; }
        public OrderItemModel[] OrderItems { get; set; } = new OrderItemModel[0];
        public int TotalCount { get; set; }
        public decimal TotalPrice { get; set; }

        public string UserName { get; set; }
        public string UserCity { get; set; }
        public string UserAddress { get; set; }
        public string UserEmail { get; set; }

        public string DeliveryName { get; set; }
        public string DeliveryDescription { get; set; }
        public decimal DeliveryPrice { get; set; }

        public string PaymentDescription { get; set; }

    }
}
