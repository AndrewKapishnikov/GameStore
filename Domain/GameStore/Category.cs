using GameStore.DataEF;
using System;
using System.Collections.Generic;


namespace GameStore
{
    //Entity Category
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
        public string Name => dto.Name;
        
        public static class DtoFactory
        {
            public static CategoryDTO Create(string name, string urlSlug)
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException(nameof(Name));

                if (string.IsNullOrWhiteSpace(urlSlug))
                    throw new ArgumentException(nameof(UrlSlug));

                if (name.Length < 3 || name.Length > 40)
                    throw new ArgumentOutOfRangeException("Name is less than 3 characters or longer than 40 characters");

                if (urlSlug.Length < 3 || urlSlug.Length > 30)
                    throw new ArgumentOutOfRangeException("UrlSlug is less than 3 characters or longer than 30 characters");

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
