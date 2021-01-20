using System;

namespace GameStore
{
    public class Game
    {
        public int Id { get; }
        public string Name { get; }
        public string Publisher { get; }
        public string Description { get; }
        public string Category { get; }
        public decimal Price { get; }
        public byte[] ImageData { get; }
        public string ReleaseData { get; set; }
        public Game (int id, string name, string publisher, string description, string category, 
                    decimal price, byte[] imageData, string releaseData)
        {
            Id = id;
            Name = name;
            Publisher = publisher;
            Description = description;
            Category = category;
            Price = price;
            ImageData = imageData;
            ReleaseData = releaseData;
        }

       
    }
}
