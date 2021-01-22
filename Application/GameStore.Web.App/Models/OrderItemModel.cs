using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Web.App
{
    public class OrderItemModel
    {
        public int GameId { get; set; }
        public string Name { get; set; }
        public string Publisher { get; set; }
        public string Category { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public byte[] ImageData { get; set; }
    }
}
