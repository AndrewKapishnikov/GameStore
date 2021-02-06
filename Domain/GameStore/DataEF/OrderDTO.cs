using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public IList<OrderItemDTO> Items { get; set; } = new List<OrderItemDTO>();
        public string UserId { get; set; }   
        public User User { get; set; }

        public string DeliveryName { get; set; }
        public string DeliveryDescription { get; set; }
        public decimal DeliveryPrice { get; set; }
        public Dictionary<string, string> DeliveryParameters { get; set; }
    }
}
