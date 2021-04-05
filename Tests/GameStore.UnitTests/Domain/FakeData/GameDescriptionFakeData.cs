using Bogus;
using System;


namespace GameStore.UnitTests.Domain.FakeData
{
    public class GameDescriptionFakeData
    {
        public GameDescriptionFakeData(int seed)
        {
            Valid = Valid.UseSeed(seed);
        }

        public Faker<GameDescription> Valid { get; private set; }
               = new Faker<GameDescription>("ru")
                 .CustomInstantiator(f => new GameDescription(f.Company.CompanyName(),
                                    f.Random.String2(20, 25, "абвгдеёжзиклмнопрстуфхцчшщэюя1234567890"),
                                    f.Random.String2(20, 10000, "абвгдеёжзиклмнопрстуфхцчшщэюя1234567890"),
                                    f.Date.Between(new DateTime(1980, 1, 01), new DateTime(2099, 12, 31)) ));
    }
}
