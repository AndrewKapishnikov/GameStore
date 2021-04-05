using GameStore.DataEF;
using System;
using System.Collections.Generic;


namespace GameStore
{
    //Entity Game
    public class Game
    {
        private readonly GameDTO dto;
        public GameDescription GameDescription { get; }
        public IReadOnlyCollection<OrderItem> GameOrders { get; }

        public Game(GameDTO dto)
        {
            this.dto = dto;
            GameOrders = new OrderItemCollectionForGame(dto.OrderItems);
            GameDescription = new GameDescription(dto.Publisher, dto.ShortDescription,
                                                  dto.Description, dto.ReleaseDate);
        }

        public int Id
        {
            get => dto.Id;
            set => dto.Id = value;
        }
        public string Name => dto.Name;
        public decimal Price => dto.Price;
        public byte[] ImageData => dto.ImageData;
        public DateTime DateOfAdding => dto.DateOfAdding;
        public bool OnSale => dto.OnSale;
        public CategoryDTO Category => dto.Category;
      
        public static class DTOFactory
        {
            public static GameDTO Create(
                          string name,
                          decimal price,
                          byte[] imageData,
                          DateTime dateOfAdding,
                          bool onSale, 
                          int categoryId,
                          GameDescription gameDescription)
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException(nameof(Name));
                
                if(imageData == null)
                    throw new ArgumentException("Picture not added");

                if (price > 100000m || price < 0)
                    throw new ArgumentOutOfRangeException("Price out of range");

                if (imageData == null)
                    throw new ArgumentException(nameof(ImageData));
   
                DateTime dateMinAdding = new DateTime(2021, 1, 01); 
                DateTime dateMax = new DateTime(2100, 1, 01);       
                if (dateOfAdding < dateMinAdding || dateOfAdding >= dateMax)
                    throw new ArgumentOutOfRangeException("DateOfAdding out of range");

                if (gameDescription == null)
                    throw new ArgumentException(nameof(GameDescription));

                return new GameDTO
                {
                    Name = name,
                    Publisher = gameDescription.Publisher.Trim(),
                    ShortDescription = gameDescription.ShortDescription.Trim(),
                    Description = gameDescription.Description.Trim(),
                    ReleaseDate = gameDescription.ReleaseDate,
                    Price = price,
                    ImageData = imageData,
                    DateOfAdding = dateOfAdding,
                    OnSale = onSale,
                    CategoryId = categoryId 
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
