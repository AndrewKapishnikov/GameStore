using GameStore.DataEF;
using System;
using System.Linq;


namespace GameStore
{
    //Entity Order
    public class Order
    {
        private readonly OrderDTO dto; 
        public int Id => dto.Id;
        public DateTime DateOfOrder
        {
            get => dto.DateOfOrder;
            set => dto.DateOfOrder = value;
        }
        public bool OrderReviewed 
        {
            get => dto.OrderReviewed;
            set => dto.OrderReviewed = value;
        }
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
        public string UserId
        {
            get => dto.UserId;
            set
            {
                if (value == null)
                    throw new ArgumentException(nameof(UserId));
                dto.UserId = value;
            }
        }
        public int TotalCount => Items.Sum(item => item.Count);

        public decimal TotalPrice => Items.Sum(item => item.Price * item.Count)
                                    + (Delivery?.DeliveryPrice ?? 0m);

        public Delivery Delivery
        {
            get
            {
                if (dto.DeliveryName == null)
                    return null;
                
                return new Delivery(
                    dto.DeliveryName,
                    dto.DeliveryDescription,
                    dto.DeliveryPrice,
                    dto.DeliveryParameters);
            }
            set
            {
                if (value == null)
                    throw new ArgumentException(nameof(Delivery));

                dto.DeliveryName = value.NameDelivery;
                dto.DeliveryDescription = value.Description;
                dto.DeliveryPrice = value.DeliveryPrice;
                dto.DeliveryParameters = value.Parameters.ToDictionary(p => p.Key, p => p.Value);
            }
        }

        public Payment Payment
        {
            get
            {
                if (dto.PaymentName == null)
                    return null;

                return new Payment(
                    dto.PaymentName,
                    dto.PaymentDescription,
                    dto.PaymentParameters);
            }
            set
            {
                if (value == null)
                    throw new ArgumentException(nameof(Payment));

                dto.PaymentName = value.NamePayment;
                dto.PaymentDescription = value.Description;
                dto.PaymentParameters = value.Parameters.ToDictionary(p => p.Key, p => p.Value);
            }
        }

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
