using System.Collections.Generic;

namespace GameStore.DataEF
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UrlSlug { get; set; }
        public IList<GameDTO> Games { get; set; } = new List<GameDTO>();
    }
}
