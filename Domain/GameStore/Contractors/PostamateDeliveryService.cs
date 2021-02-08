using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Contractors
{
    public class PostamateDeliveryService : IDeliveryService
    {
        public string Name => "Postamate";
        public string Title => "Доставка товаров через постаматы в Саратове и Москве";
        public decimal DeliveryPrice => 150m;

        private static IReadOnlyDictionary<string, string> cities = new Dictionary<string, string>
        {
            { "1", "Саратов" },
            { "2", "Москва" },
        };

        private static IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> postamates = new Dictionary<string, IReadOnlyDictionary<string, string>>
        {
            {
                "1",
                new Dictionary<string, string>
                {
                    { "1", "Торговый центр Триумф" },
                    { "2", "Тау Галлерея" },
                    { "3", "Детский Мир. Проспект Кирова" },
                    { "4", "Ж/д вокзал. Постаматы" },
                }
            },
            {
                "2",
                new Dictionary<string, string>
                {
                    { "5", "Павелецкий вокзал" },
                    { "6", "Ленинградский вокзал" },
                    { "7", "Охотный ряд" },
                    { "8", "Москворецкий рынок"}
                }
            }
        };

        public DataSteps FirstStep(Order order)
        {
            return DataSteps.CreateFirst(Name)
                       .AddServicePrice(DeliveryPrice)
                       .AddParameter("orderId", order.Id.ToString())
                       .AddField(new ChoiceField("Город", "city", cities));
        }

        public DataSteps NextStep(int step, IReadOnlyDictionary<string, string> values)
        {
            if (step == 1)
            {
                if (values["city"] == "1")
                {
                    return DataSteps.CreateNext(Name, 2, values)
                               .AddField(new ChoiceField("Постамат", "postamate", postamates["1"]));
                }
                else if (values["city"] == "2")
                {
                    return DataSteps.CreateNext(Name, 2, values)
                               .AddField(new ChoiceField("Постамат", "postamate", postamates["2"]));
                }
                else
                    throw new InvalidOperationException("Invalid postamate city");
            }
            else if (step == 2)
            {
                return DataSteps.CreateLast(Name, 3, values);
            }
            else
                throw new InvalidOperationException("Invalid step for postamate");
        }

        public Delivery GetDelivery(DataSteps data)
        {
            if (data.ServiceName != Name || !data.IsFinal)
                throw new InvalidOperationException("Invalid form.");

            var cityId = data.Parameters["city"];
            var cityName = cities[cityId];
            var postamateId = data.Parameters["postamate"];
            var postamateName = postamates[cityId][postamateId];

            var parameters = new Dictionary<string, string>
            {
                { nameof(cityId), cityId },
                { nameof(cityName), cityName },
                { nameof(postamateId), postamateId },
                { nameof(postamateName), postamateName },
            };

            var description = $"Город: {cityName}  \nПостамат: {postamateName}";

            return new Delivery(Name, description, DeliveryPrice, parameters);
        }
    }
}
