using Bogus;
using GameStore.DataEF;
using GameStore.UnitTests.Domain.FakeData;
using GameStore.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

[assembly: Parallelizable(ParallelScope.Fixtures)]
[assembly: LevelOfParallelism(4)]

namespace GameStore.UnitTests
{
    public class BaseTest
    {
        protected string russianLettersAndNumbers = "абвгдеёжзиклмнопрстуфхцчшщэюя1234567890";
        protected IReadOnlyDictionary<string, string> parameters;
        protected IList<OrderItemDTO> collectionItemDto;
        protected IList<GameDTO> collectionGameDto;
        protected IList<CategoryDTO> categoryDto;
        protected OrderDTO fakeOrderDto;
        protected string publisher;
        protected string shortDescription;
        protected string description;
        protected DateTime releaseDate;
        protected decimal price;

        protected Faker Faker { get; private set; }
        protected GameDescriptionFakeData GameDescriptionFakeData { get; private set; }
          
        [SetUp]
        public void SetupBeforeEachTest()
        {
            const int seed = 5678;
            Randomizer.Seed = new Random(seed);
            Faker = new Faker("ru");

            publisher = Faker.Company.CompanyName();
            shortDescription = Faker.Random.String2(20, 25, russianLettersAndNumbers);
            description = Faker.Random.String2(20, 10000, russianLettersAndNumbers);
            releaseDate = Faker.Date.Between(new DateTime(1980, 1, 01), new DateTime(2099, 12, 31));
            price = Faker.Random.Decimal(0, 100000m);
            parameters = CreateFakeParameters();
            collectionItemDto = CreateListFakeOrderItemsDto();
            GameDescriptionFakeData = new GameDescriptionFakeData(seed);
            collectionGameDto = CreateFakeListGameDto();
            categoryDto = CreateListFakeCategoryDto();
            fakeOrderDto = CreateFakeOrder();
        }


        private IReadOnlyDictionary<string, string> CreateFakeParameters()
        {
            return new Dictionary<string, string>()
            {
                { Faker.Random.String2(10, russianLettersAndNumbers), Faker.Random.String2(15, russianLettersAndNumbers) },
                { Faker.Random.String2(7, russianLettersAndNumbers), Faker.Random.String2(25, russianLettersAndNumbers) },
                { Faker.Random.String2(12, russianLettersAndNumbers), Faker.Random.String2(14, russianLettersAndNumbers) }
            }; 

        }

        private IList<OrderItemDTO> CreateListFakeOrderItemsDto() =>
                new List<OrderItemDTO>()
                {
                    new OrderItemDTO(){ GameId = 1, Price = 1000m, Count = 2},
                    new OrderItemDTO(){ GameId = 2, Price = 2000m, Count = 4}
                };
        

        private IList<GameDTO> CreateFakeListGameDto()
        {
            return new List<GameDTO>()
            {
                new GameDTO(){ Id = 1,
                    Name = Faker.Random.String2(5, 30, russianLettersAndNumbers),
                    Price =  Faker.Random.Decimal(0, 100000m),
                    Publisher = publisher,
                    Description = description,
                    ShortDescription = shortDescription,
                    ImageData = Faker.Random.Bytes(500),
                    DateOfAdding = Faker.Date.Between(new DateTime(2021, 1, 01), new DateTime(2100, 1, 01)),
                    ReleaseDate = releaseDate },
                new GameDTO(){ Id = 2,
                    Name = Faker.Random.String2(5, 10, russianLettersAndNumbers),
                    Price = Faker.Random.Decimal(0, 100000m),
                    Publisher = publisher,
                    ShortDescription = shortDescription,
                    Description = description,
                    ImageData = Faker.Random.Bytes(500),
                    DateOfAdding = Faker.Date.Between(new DateTime(2021, 1, 01), new DateTime(2100, 1, 01)),
                    ReleaseDate = releaseDate},
                new GameDTO(){ Id = 3,
                    Name = Faker.Random.String2(5, 70, russianLettersAndNumbers),
                    Price = Faker.Random.Decimal(0, 100000m),
                    Publisher = publisher,
                    ShortDescription = shortDescription,
                    Description = description,
                    ImageData = Faker.Random.Bytes(500),
                    DateOfAdding = Faker.Date.Between(new DateTime(2021, 1, 01), new DateTime(2100, 1, 01)),
                    ReleaseDate = releaseDate}
            };
        }

        private IList<CategoryDTO> CreateListFakeCategoryDto()
        {
            return new List<CategoryDTO>()
            {
                new CategoryDTO(){
                    Id = 1, 
                    Name = Faker.Random.String2(3, 40, russianLettersAndNumbers),
                    UrlSlug = Faker.Random.String2(3, 30, russianLettersAndNumbers)
                 },
               new CategoryDTO(){
                    Id = 2,
                    Name = Faker.Random.String2(3, 40, russianLettersAndNumbers),
                    UrlSlug = Faker.Random.String2(3, 30, russianLettersAndNumbers)
                 },
               new CategoryDTO(){
                    Id = 3,
                    Name = Faker.Random.String2(3, 40, russianLettersAndNumbers),
                    UrlSlug = Faker.Random.String2(3, 30, russianLettersAndNumbers)
                 }
            };
        }

        private OrderDTO CreateFakeOrder() =>
            new OrderDTO()
            {
                Id = 1,
                DateOfOrder = DateTime.UtcNow,
                DeliveryDescription = Faker.Random.String2(3, 20, russianLettersAndNumbers),
                DeliveryName = Faker.Random.String2(3, 20, russianLettersAndNumbers),
                DeliveryPrice = price,
                DeliveryParameters = new Dictionary<string, string>(),
                Items = new List<OrderItemDTO>(),
                OrderReviewed = true,
                PaymentDescription = Faker.Random.String2(3, 20, russianLettersAndNumbers),
                PaymentName = Faker.Random.String2(3, 20, russianLettersAndNumbers),
                PaymentParameters = new Dictionary<string, string>(),
                UserId = Guid.NewGuid().ToString()
            };
        

        public static UserManager<TUser> FaketUserManager<TUser>() where TUser : class
        {
            var userStore = Substitute.For<IUserStore<TUser>>();
            var optionAccessor = Substitute.For<IOptions<IdentityOptions>>();
            var passwordHasher = Substitute.For<IPasswordHasher<TUser>>();
            var userValidators = Substitute.For<IEnumerable<IUserValidator<TUser>>>();
            var passwordValidators = Substitute.For<IEnumerable<IPasswordValidator<TUser>>>();
            var keyNormalizer = Substitute.For<ILookupNormalizer>();
            var errors = new IdentityErrorDescriber();
            var services = Substitute.For<IServiceProvider>();
            var logger = Substitute.For<ILogger<UserManager<TUser>>>();
            var userManager = Substitute.For<UserManager<TUser>>(userStore, optionAccessor, passwordHasher,
                userValidators, passwordValidators, keyNormalizer, errors, services, logger);
            return userManager;
        }

        public static SignInManager<TUser>  FakeSignInManager<TUser>() where TUser: class
        {
            var userManager = FaketUserManager<TUser>();
            var contextAccessor = Substitute.For<IHttpContextAccessor>();
            var claimsPrincipalFactory = Substitute.For<IUserClaimsPrincipalFactory<TUser>>();
            var identityOptions = Substitute.For<IOptions<IdentityOptions>>();
            var logger = Substitute.For<ILogger<SignInManager<TUser>>> ();
            var schemeProvider = Substitute.For<IAuthenticationSchemeProvider>();
            var userConfirmation = Substitute.For<IUserConfirmation<TUser>>();
            return Substitute.For<SignInManager<TUser>>(userManager, contextAccessor, claimsPrincipalFactory,
                                                        identityOptions, logger, schemeProvider, userConfirmation);
       }
     
        

    }
}
