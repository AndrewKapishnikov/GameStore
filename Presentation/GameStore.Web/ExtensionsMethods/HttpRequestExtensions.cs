using Microsoft.AspNetCore.Http;


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