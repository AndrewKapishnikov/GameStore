using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.DataEF
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<GameDTO> Games { get; set; } = new List<GameDTO>();
    }
}
