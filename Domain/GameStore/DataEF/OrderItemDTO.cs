
namespace GameStore.DataEF
{
    public class OrderItemDTO
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }

        public int? GameId { get; set; }
        public GameDTO Game { get; set; }

        public int OrderId { get; set; }
        public OrderDTO Order { get; set; }

       
    }
}
