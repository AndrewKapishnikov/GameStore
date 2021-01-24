using System;

namespace GameStore
{
    public class Game
    {
        public int Id { get; }
        public string Name { get; }
        public string Publisher { get; }
        public string ShortDescription { get; }
        public string Description { get; }
        public string Category { get; }
        public decimal Price { get; }
        public byte[] ImageData { get; }
        public string ReleaseDate { get; }
        public DateTime DateOfAdding { get; }
        public Game (int id, string name, string publisher, string shortDescription, string description,
                    string category, decimal price, byte[] imageData, string releaseDate, DateTime dateOfAdding)
        {
            Id = id;
            Name = name;
            Publisher = publisher;
            ShortDescription = shortDescription;
            Description = description;
            Category = category;
            Price = price;
            ImageData = imageData;
            ReleaseDate = releaseDate;
            DateOfAdding = dateOfAdding;
        }

       
    }
}
