using GameStore.DataEF;
using NUnit.Framework;

namespace GameStore.UnitTests.Domain.Entities
{
    [TestFixture]
    public class OrderTests : BaseTest
    {
        [Test]
        public void TotalCount_WithEmptyItems_ReturnsZero()
        {
            var order = CreateEmptyTestOrder();

            Assert.AreEqual(0, order.TotalCount);
        }

        [Test]
        public void TotalPrice_WithEmptyItems_ReturnsZero()
        {
            var order = CreateEmptyTestOrder();

            Assert.AreEqual(0m, order.TotalCount);
        }

        [Test]
        public void TotalCountItemsOfOrder_WithNonEmptyItems_CalcualtesTotalCount()
        {
            var order = CreateTestOrder();

            Assert.AreEqual(2 + 5 + 1 + 4, order.TotalCount);
        }

        [Test]
        public void TotalPrice_WithNonEmptyItems_CalcualtesTotalPrice()
        {
            var order = CreateTestOrder();

            Assert.AreEqual(2 * 1000m + 5 * 2000m + 3000m + 4 * 1500m, order.TotalPrice);
        }


        private static Order CreateEmptyTestOrder()
        {
            return new Order(new OrderDTO
            {
                Id = 1,
                Items = new OrderItemDTO[0]
            });
        }
        private static Order CreateTestOrder()
        {
            return new Order(new OrderDTO
            {
                Id = 1,
                Items = new[]
                {
                    new OrderItemDTO { Id = 1, GameId = 1, Price = 1000m, Count = 2},
                    new OrderItemDTO { Id = 2, GameId = 2, Price = 2000m, Count = 5},
                    new OrderItemDTO { Id = 3, GameId = 3, Price = 3000m, Count = 1},
                    new OrderItemDTO { Id = 4, GameId = 4, Price = 1500m, Count = 4},
                }
            });
        }


    }
}
