using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace GameStore.Web.ExtensionsMethods
{
    public static class RequestLocalizationExtensions
    {
        public static IApplicationBuilder UseRequestLocalizationOptions(this IApplicationBuilder builder)
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("en"),
                new CultureInfo("ru-RU"),
                new CultureInfo("ru")
            };
            return builder.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("ru-RU"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });
        }
    }
}
