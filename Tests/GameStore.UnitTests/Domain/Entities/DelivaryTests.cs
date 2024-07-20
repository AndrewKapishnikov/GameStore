using FluentAssertions;
using NUnit.Framework;
using System;


namespace GameStore.UnitTests.Domain.Entities
{
    [TestFixture]
    public class DeliveryTests: BaseTest
    {
        [Test]
        public void Delivery_With_Valid_Arguments_Is_Created()
        {
            var nameDelivery = Faker.Random.String2(3, 100, russianLettersAndNumbers);
            var description = Faker.Random.String2(1, 100, russianLettersAndNumbers);
      
            Func<Delivery> createDelivery = () =>
                 new Delivery(
                     nameDelivery,
                     description,
                     price,
                     parameters);
   
            createDelivery.Should().NotThrow();

            Assert.Multiple(() =>
            {
                var delivery = createDelivery();
                delivery.NameDelivery.Should().Be(nameDelivery);
                delivery.DeliveryPrice.Should().Be(price);
                delivery.Description.Should().Be(description);
                delivery.Parameters.Count.Should().Be(3);
            });
        }


        [Test]
        public void Delivery_With_Invalid_Arguments_Throw_Exceptions()
        {
            var nameDelivery = Faker.Random.String2(3, 50, russianLettersAndNumbers);
            var description = Faker.Random.String2(1, 90, russianLettersAndNumbers);
            var nameDeliveryLessThenMinItem = Faker.Random.String2(1, 2, russianLettersAndNumbers);
            var nameDeliveryGreaterThenMaxItem = Faker.Random.String2(101, 2000, russianLettersAndNumbers);
            var priceLessThenZero = Faker.Random.Decimal(Decimal.MinValue, 0);
            var priceGreterThenMax = Faker.Random.Decimal(100001m, Decimal.MaxValue);

            Func<Delivery> createDeliveryWithNullName = () =>  new Delivery(null, description, price, parameters);
            Func<Delivery> createDeliveryWithNullDescription = () => new Delivery(nameDelivery, null, price, parameters);
            Func<Delivery> createDeliveryWithNullParameters = () => new Delivery(nameDelivery, description, price, null);
            Func<Delivery> createDeliveryWithNameDeliveryLessThenMin = () => new Delivery(nameDeliveryLessThenMinItem, description, price, parameters);
            Func<Delivery> createDeliveryWithNameDeliveryGreaterThenMax = () => new Delivery(nameDeliveryGreaterThenMaxItem, description, price, parameters);
            Func<Delivery> createDeliveryWithPriceLessThenZero = () => new Delivery(nameDelivery, description, priceLessThenZero, parameters);
            Func<Delivery> createDeliveryWithPriceGreaterThenMax = () => new Delivery(nameDelivery, description, priceGreterThenMax, parameters);

            Assert.Multiple(() =>
            {
                createDeliveryWithNullName.Should().Throw<ArgumentException>();
                createDeliveryWithNullDescription.Should().Throw<ArgumentException>();
                createDeliveryWithNullParameters.Should().Throw<ArgumentNullException>();
                createDeliveryWithNameDeliveryLessThenMin.Should().Throw<ArgumentOutOfRangeException>();
                createDeliveryWithNameDeliveryGreaterThenMax.Should().Throw<ArgumentOutOfRangeException>();
                createDeliveryWithPriceLessThenZero.Should().Throw<ArgumentOutOfRangeException>();
                createDeliveryWithPriceGreaterThenMax.Should().Throw<ArgumentOutOfRangeException>();
            });
        }



    }
}
