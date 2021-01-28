using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.DataEF
{
    public class User : IdentityUser
    {
        public string City { get; set; }
        public IList<OrderDTO> Orders { get; set; } = new List<OrderDTO>();
    }
}
