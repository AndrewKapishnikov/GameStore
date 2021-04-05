using System;

namespace GameStore
{
    //ValueObject GameDescription
    public class GameDescription
    {
        public string Publisher { get; }
        public string ShortDescription { get; }
        public string Description { get; }
        public DateTime ReleaseDate { get; }

        public GameDescription(string publisher, 
               string shortDescription, 
               string description,
               DateTime releaseDate)
        {
            if (string.IsNullOrWhiteSpace(publisher))
                throw new ArgumentException(nameof(Publisher));

            if (string.IsNullOrWhiteSpace(shortDescription))
                throw new ArgumentException(nameof(ShortDescription));

            if(shortDescription.Length < 20 || shortDescription.Length > 170)
                throw new ArgumentOutOfRangeException("ShortDescription out of range");

            if (releaseDate < new DateTime(1980, 1, 01) ||
                releaseDate >= new DateTime(2100, 1, 01))
                throw new ArgumentOutOfRangeException("ReleaseDate out of range");

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException(nameof(ShortDescription));

            Publisher = publisher;
            ShortDescription = shortDescription;
            Description = description;
            ReleaseDate = releaseDate;
        }

        public static GameDescription Create(string publisher,
                      string shortDescription,
                      string description,
                      DateTime releaseDate)
                      => new GameDescription(publisher, shortDescription, description, releaseDate);

    }
}
