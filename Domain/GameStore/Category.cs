using GameStore.DataEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore
{
    public class Category
    {
        private readonly CategoryDTO dto;
        public IReadOnlyCollection<Game> CategoryGames { get; }

        public Category(CategoryDTO categorydto)
        {
            dto = categorydto;
            CategoryGames = new GameCollectionForCategory(dto.Games);
        }

        public int Id => dto.Id;
        public string UrlSlug => dto.UrlSlug;
        public string Name
        {
            get => dto.Name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(nameof(Name));
                dto.Name = value.Trim();
            }
        }

      
        public static class DtoFactory
        {
            public static CategoryDTO Create(string name, string urlSlug)
            {
                if (string.IsNullOrWhiteSpace(name) ||
                    string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException(nameof(Name));

                return new CategoryDTO() { Name = name, UrlSlug = urlSlug };
            }
        }

        public static class Mapper
        {
            public static Category Map(CategoryDTO dto) => new Category(dto);

            public static CategoryDTO Map(Category domain) => domain.dto;
        }

    }
}
