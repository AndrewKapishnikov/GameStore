
namespace GameStore.Web.App
{
    public class OrderItemModel
    {
        public int GameId { get; set; }
        public string GameName { get; set; }
        public string Publisher { get; set; }
        public string Category { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public byte[] ImageData { get; set; }
    }
}
