using GameStore.DataEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore
{
    public class Game
    {
        private readonly GameDTO dto;
        public IReadOnlyCollection<OrderItem> GameOrders { get; }
        internal Game(GameDTO dto)
        {
            this.dto = dto;
            GameOrders = new OrderItemCollectionForGame(dto.OrderItems);
        }

        public int Id => dto.Id;

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

        public string Publisher
        {
            get => dto.Publisher;
            set 
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(nameof(Publisher));
                dto.Publisher = value.Trim();
            }
        }
        public decimal Price
        {
            get => dto.Price;
            set
            {
                dto.Price = value;
            }
        }
        public string ShortDescription
        {
            get => dto.ShortDescription;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(nameof(ShortDescription));
                dto.Description = value.Trim();
            }
        }

        public string Description
        {
            get => dto.Description;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(nameof(Description));
                dto.Description = value;
            }
        }
        public byte[] ImageData
        {
            get => dto.ImageData;
            set
            {
                dto.ImageData = value;
            }
        }

        public string ReleaseDate
        {
            get => dto.ReleaseDate;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(nameof(ReleaseDate));
                dto.ReleaseDate = value.Trim();
            }
        }
        public DateTime DateOfAdding
        {
            get => dto.DateOfAdding;
            set
            {

                if (value == null)
                    throw new ArgumentException(nameof(DateOfAdding));
                dto.DateOfAdding = value;
            }
        }

        public bool OnSale
        {
            get => dto.OnSale;
            set => dto.OnSale = value;
            
        }

        public CategoryDTO Category
        {
            get => dto.Category;
            set => dto.Category = value;
        }

        public static class DTOFactory
        {
            public static GameDTO Create(string name,
                                         string publisher,
                                         string shortDescription,
                                         string description,
                                         decimal price,
                                         byte[] imageData,
                                         string releaseDate,
                                         DateTime dateOfAdding,
                                         bool onSale,
                                         CategoryDTO category)
            {
                if (string.IsNullOrWhiteSpace(name)             ||
                    string.IsNullOrWhiteSpace(publisher)        ||
                    string.IsNullOrWhiteSpace(shortDescription) ||
                    string.IsNullOrWhiteSpace(description)      ||
                    string.IsNullOrWhiteSpace(releaseDate))      
                    throw new ArgumentException("One or more arguments has null value");
                
                if (dateOfAdding == null)
                    throw new ArgumentException(nameof(dateOfAdding));

                return new GameDTO
                {
                    Name = name,
                    Publisher = publisher.Trim(),
                    ShortDescription = shortDescription.Trim(),
                    Description = description.Trim(),
                    Price = price,
                    ImageData = imageData,
                    ReleaseDate = releaseDate,
                    DateOfAdding = dateOfAdding,
                    OnSale = onSale,
                    Category = category 
                };
            }
        }

        public static class Mapper
        {
            public static Game Map(GameDTO dto) => new Game(dto);

            public static GameDTO Map(Game domain) => domain.dto;
        }


    }
}
