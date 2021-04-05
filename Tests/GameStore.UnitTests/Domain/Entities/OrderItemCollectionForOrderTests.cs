using GameStore.DataEF;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;


namespace GameStore.UnitTests.Domain.Entities
{
    [TestFixture]
    public class OrderItemCollectionForOrderTests: BaseTest
    {
        [Test]
        public void Get_WithExistingItem_ReturnsOrderItem()
        {
            //Arrange
            var order = CreateTestOrder();
            var game = Game.Mapper.Map(collectionGameDto[0]);

            //Act
            var orderItem = order.Items.Get(game);

            //Assert
            Assert.AreEqual(collectionGameDto[0].Price, orderItem.Price);
            Assert.AreEqual(2, orderItem.Count);
        }

        [Test]
        public void Get_WithNonExistingItem_ThrowsInvalidOperationException()
        {
            var order = CreateTestOrder();
            var game = Game.Mapper.Map(collectionGameDto[2]);

            Assert.Throws<InvalidOperationException>(() =>
            {
                order.Items.Get(game);
            });
        }

        [Test]
        public void Add_WithNewItem_CheckCount()
        {
            var order = CreateTestOrder();
            var game = Game.Mapper.Map(collectionGameDto[2]);

            order.Items.Add(game, 5);

            Assert.AreEqual(5, order.Items.Get(game).Count);
        }

        [Test]
        public void Add_WithExistingItem_ThrowInvalidOperationException()
        {
            var order = CreateTestOrder();
            var game = Game.Mapper.Map(collectionGameDto[0]);

            Assert.Throws<InvalidOperationException>(() =>
            {
                order.Items.Add(game, 4);
            });
        }

        [Test]
        public void Remove_WithExistingItem_RemoveItem()
        {
            var order = CreateTestOrder();
            var orderItemSecond = CreateTestOrder().Items.Where(p => p.Game.Id == collectionGameDto[1].Id).First();

            order.Items.Remove(1);

            Assert.AreEqual(order.Items.First().Game.Id, orderItemSecond.Game.Id);
            Assert.AreEqual(order.Items.First().Price, orderItemSecond.Price);
            Assert.AreEqual(order.Items.First().Count, orderItemSecond.Count);
        }

        [Test]
        public void Remove_WithNonExistingItem_ThrowsInvalidOperationException()
        {
            var order = CreateTestOrder();
            var fakerGameId = Faker.Random.Int(3, int.MaxValue);

            Assert.Throws<InvalidOperationException>(() =>
            {
                order.Items.Remove(fakerGameId);
            });
        }

        private Order CreateTestOrder()
        {
            return new Order(new OrderDTO
            {
                Id = 1,
                Items = new List<OrderItemDTO>
                {
                    new OrderItemDTO { GameId = collectionGameDto[0].Id, 
                                       Game =  collectionGameDto[0],
                                       Price = collectionGameDto[0].Price,
                                       Count = 2},
                    new OrderItemDTO { GameId = collectionGameDto[1].Id,
                                       Game = collectionGameDto[1],
                                       Price = collectionGameDto[1].Price,
                                       Count = 7},
                }
            });
        }


    }
}
