using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace GameStore.DataEF
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public IList<OrderDTO> Orders { get; set; } = new List<OrderDTO>();
    }
}
