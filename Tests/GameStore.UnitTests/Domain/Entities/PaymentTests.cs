using FluentAssertions;
using NUnit.Framework;
using System;


namespace GameStore.UnitTests.Domain.Entities
{
    [TestFixture]
    public class PaymentTests: BaseTest
    {
        [Test]
        public void Payment_With_Valid_Arguments_Is_Created()
        {
            var namePayment = Faker.Random.String2(3, 100, russianLettersAndNumbers);
            var description = Faker.Random.String2(1, 999, russianLettersAndNumbers);

            Func<Payment> createPayment = () => new Payment(namePayment, description, parameters);

            createPayment.Should().NotThrow();
            Assert.Multiple(() =>
            {
                var payment = createPayment();
                payment.NamePayment.Should().Be(namePayment);
                payment.Description.Should().Be(description);
                payment.Parameters.Count.Should().Be(3);
            });
        }


        [Test]
        public void Payment_With_Invalid_Arguments_Throw_Exceptions()
        {
            var namePayment = Faker.Random.String2(3, 100, russianLettersAndNumbers);
            var description = Faker.Random.String2(1, 999, russianLettersAndNumbers);
            var namePaymentLessThenMinItem = Faker.Random.String2(1, 2, russianLettersAndNumbers);
            var namePaymentGreaterThenMaxItem = Faker.Random.String2(101, 2000, russianLettersAndNumbers);
            var descriptionGreaterThenMaxItem = Faker.Random.String2(1001, 3000, russianLettersAndNumbers);

            Func<Payment> createPaymentWithNullNamePayment = () => new Payment(null, description, parameters);
            Func<Payment> createPaymentWithNamePaymentLessThenMin = () => new Payment(namePaymentLessThenMinItem, description, parameters);
            Func<Payment> createPaymentWithNamePaymentGreaterThenMax = () => new Payment(namePaymentGreaterThenMaxItem, description, parameters);
            Func<Payment> createPaymentWithNullDescription = () => new Payment(namePayment, null, parameters);
            Func<Payment> createPaymentWithNullParameters = () => new Payment(namePayment, description, null);

            Assert.Multiple(() =>
            {
                createPaymentWithNullNamePayment.Should().Throw<ArgumentException>();
                createPaymentWithNamePaymentLessThenMin.Should().Throw<ArgumentOutOfRangeException>();
                createPaymentWithNamePaymentGreaterThenMax.Should().Throw<ArgumentOutOfRangeException>();
                createPaymentWithNullDescription.Should().Throw<ArgumentException>();
                createPaymentWithNullParameters.Should().Throw<ArgumentNullException>();
            });
        }


    }
}
