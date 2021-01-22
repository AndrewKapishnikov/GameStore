using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Web.ExtensionsMethods
{
    public static class ExtensionHttpRequest
    {
        public static string GetCurrentUrl(this HttpRequest httpRequest)
        {
            var path = httpRequest.Path.ToString();
            var query = httpRequest.QueryString.ToString();
            return $"{path}{query}";
        }
    }
}
