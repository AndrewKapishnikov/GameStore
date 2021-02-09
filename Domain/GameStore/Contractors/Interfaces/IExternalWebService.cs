using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStore.Contractors.Interfaces
{
    public interface IExternalWebService
    {
        string Name { get; }
        Task<Uri> GetServiceUriAsync(IReadOnlyDictionary<string, string> parameters, Uri returnUri);
    }
}
