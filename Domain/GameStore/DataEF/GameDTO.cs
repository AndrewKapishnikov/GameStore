using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.DataEF
{
    public class GameDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Publisher { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public byte[] ImageData { get; set; }
        public string ReleaseDate { get; set; }
        public DateTime DateOfAdding { get; set; }
        public bool OnSale { get; set; }

        public IList<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();

        public int CategoryId { get; set; }
        public CategoryDTO Category { get; set; }
    }
}
