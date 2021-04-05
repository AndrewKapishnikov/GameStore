using System;
using System.Collections.Generic;


namespace GameStore.DataEF
{
    public class OrderDTO
    {
        //Perhaps the data type should be changed to Guid
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //[Key]
        //public Guid Id { get; set; }

        public int Id { get; set; }
        public DateTime DateOfOrder { get; set; }
        public bool OrderReviewed { get; set; }

        public string UserId { get; set; }   
        public User User { get; set; }

        public string DeliveryName { get; set; }
        public string DeliveryDescription { get; set; }
        public decimal DeliveryPrice { get; set; }
        public Dictionary<string, string> DeliveryParameters { get; set; }

        public string PaymentName { get; set; }
        public string PaymentDescription { get; set; }
        public Dictionary<string, string> PaymentParameters { get; set; }

        public IList<OrderItemDTO> Items { get; set; } = new List<OrderItemDTO>();
    }
}
