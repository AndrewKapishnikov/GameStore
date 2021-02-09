using GameStore.Contractors.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.Contractors
{
    public class EmulateKassaPaymentService : IPaymentService, IExternalWebService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public EmulateKassaPaymentService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        private HttpRequest Request => httpContextAccessor.HttpContext.Request;
        public string Name => "EmulateKassa";
        public string Title => "Оплата банковской картой (Emulate)";

        public DataSteps FirstStep(Order order)
        {
            return DataSteps.CreateFirst(Name)
                        .AddParameter("orderId", order.Id.ToString());
        }

        public Payment GetPayment(DataSteps data)
        {
            if (data.ServiceName != Name || !data.IsFinal)
                throw new InvalidOperationException("Invalid payment form");

            return new Payment(Name, Title, data.Parameters);
        }

        public Task<Uri> GetServiceUriAsync(IReadOnlyDictionary<string, string> parameters, Uri returnUri)
        {
            var queryString = QueryString.Create(parameters);
            queryString += QueryString.Create("returnUri", returnUri.ToString());

            var builder = new UriBuilder(Request.Scheme, Request.Host.Host)
            {
                Path = "EmulateKassa",
                Query = queryString.ToString(),
            };

            if (Request.Host.Port != null)
                builder.Port = Request.Host.Port.Value;

            return Task.FromResult(builder.Uri);
        }

        public DataSteps NextStep(int step, IReadOnlyDictionary<string, string> values)
        {
            if (step != 1)
                throw new InvalidOperationException("Invalid step Kassa payment");

            return DataSteps.CreateLast(Name, step + 1, values);
        }
    }
}
