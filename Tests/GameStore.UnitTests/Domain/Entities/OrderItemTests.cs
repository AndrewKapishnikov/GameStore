using FluentAssertions;
using GameStore.DataEF;
using NUnit.Framework;
using System;


namespace GameStore.UnitTests.Domain.Entities
{
    [TestFixture]
    public class OrderItemTests: BaseTest
    {
        [Test]
        public void OrderItem_WithPositiveCount_SetsCount()
        {
            var gameDto = CreateTestGameDto();
            var orderItemDto = OrderItem.DtoFactory.Create(new OrderDTO(), gameDto, 3);
            var orderItem = OrderItem.Mapper.Map(orderItemDto);

            Assert.AreEqual(gameDto.Id, orderItem.Game.Id);
            Assert.AreEqual(gameDto.Price, orderItem.Price);
            Assert.AreEqual(3, orderItem.Count);
        }

        [Test]
        public void OrderItemCount_WithNegativeValue_ThrowsArgumentOfRangeException()
        {
            var negativeCount = Faker.Random.Int(int.MinValue, -1);
            var gameDto = CreateTestGameDto();
            Func<OrderItemDTO> createorderDto = () => OrderItem.DtoFactory.Create(new OrderDTO(), gameDto, negativeCount);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                createorderDto();
            });
        }

        [Test]
        public void ChangeCountByOneItem_ValidCount_Equal_To_One()
        {
            var count = 3;
            var gameDto = CreateTestGameDto();
            var orderItemDto = OrderItem.DtoFactory.Create(new OrderDTO(), gameDto, count);
            var orderItem = OrderItem.Mapper.Map(orderItemDto);

            orderItem.ChangeCountByOneItem(1);

            Assert.AreEqual(count + 1, orderItem.Count);
        }

        [Test]
        public void ChangeCountByOneItem_InValidCount()
        {
            var count = Faker.Random.Int(1, 9);
            var gameDto = CreateTestGameDto();
            var orderItemDto = OrderItem.DtoFactory.Create(new OrderDTO(), gameDto, count);
            var orderItem = OrderItem.Mapper.Map(orderItemDto);
            var invalidNegativeCount = Faker.Random.Int(int.MinValue, -2);
            var invalidPositiveCount = Faker.Random.Int(2, int.MaxValue);
            
            orderItem.ChangeCountByOneItem(invalidNegativeCount);
            orderItem.ChangeCountByOneItem(invalidPositiveCount);
          
            Assert.AreEqual(count, orderItem.Count);
        }


        [Test]
        public void ChangeCount_ValidCount()
        {
            var count = Faker.Random.Int(1, 9);
            var gameDto = CreateTestGameDto();
            var orderItemDto = OrderItem.DtoFactory.Create(new OrderDTO(), gameDto, count);
            var orderItem = OrderItem.Mapper.Map(orderItemDto);
            var validCount = Faker.Random.Int(1, 9);

            orderItem.ChangeCount(validCount);
        
            Assert.AreEqual(validCount, orderItem.Count);
        }


        [Test]
        public void ChangeCount_InValidCountLessThenZero()
        {
            var count = Faker.Random.Int(1, 9);
            var gameDto = CreateTestGameDto();
            var orderItemDto = OrderItem.DtoFactory.Create(new OrderDTO(), gameDto, count);
            var orderItem = OrderItem.Mapper.Map(orderItemDto);
            var invalidCountLessThenZero = Faker.Random.Int(int.MinValue, -1);

            orderItem.ChangeCount(invalidCountLessThenZero);

            Assert.AreNotEqual(invalidCountLessThenZero, orderItem.Count);
        }


        [Test]
        public void ChangeCount_InValidCountGreaterThenMaxPoint()
        {
            var count = Faker.Random.Int(1, 9);
            var gameDto = CreateTestGameDto();
            var orderItemDto = OrderItem.DtoFactory.Create(new OrderDTO(), gameDto, count);
            var orderItem = OrderItem.Mapper.Map(orderItemDto);
            var invalidCountGreaterThenMaxPoint = Faker.Random.Int(10, int.MaxValue);

            orderItem.ChangeCount(invalidCountGreaterThenMaxPoint);

            Assert.AreNotEqual(invalidCountGreaterThenMaxPoint, orderItem.Count);
        }


        [Test]
        public void OrderItem_With_Invalid_Arguments_Throw_Exception()
        {
            var gameDto = CreateTestGameDto();
            Func<OrderItemDTO> createOrderDtoWithNullCount = () => OrderItem.DtoFactory.Create(new OrderDTO(), gameDto, 0);
            Func<OrderItemDTO> createOrderDtoWithNullOrder = () => OrderItem.DtoFactory.Create(null, gameDto, 10);
            Func<OrderItemDTO> createOrderDtoWithNullGameDto = () => OrderItem.DtoFactory.Create(new OrderDTO(), null, 10);

            Assert.Multiple(() =>
            {
                createOrderDtoWithNullCount.Should().Throw<ArgumentOutOfRangeException>();
                createOrderDtoWithNullOrder.Should().Throw<ArgumentNullException>();
                createOrderDtoWithNullGameDto.Should().Throw<ArgumentNullException>();
            });
        }

        private GameDTO CreateTestGameDto()
        {
            var name = Faker.Random.String2(20, russianLettersAndNumbers);
            var price = Faker.Random.Decimal(0, 100000m);
            var imageData = Faker.Random.Bytes(500);
            var dateOfAdding = Faker.Date.Between(new DateTime(2021, 1, 01), new DateTime(2100, 1, 01));
            var categoryId = Faker.Random.Int(1, int.MaxValue);
            var gameDescription = GameDescriptionFakeData.Valid.Generate();
            var gameDto = Game.DTOFactory.Create(name, price, imageData, dateOfAdding, true, categoryId, gameDescription);
            gameDto.Id = Faker.Random.Int(1, int.MaxValue);
            return gameDto;
        }

    }
}
