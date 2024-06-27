using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Http;
using System;

namespace GameStore.Web.ExtensionsMethods
{
    public static class StaticFilesConfigExtensions
    {
        public static IApplicationBuilder UseStaticOptionsFiles(this IApplicationBuilder builder)
        {
            Action<StaticFileResponseContext> onPrepareResponse = responseContext =>
            {
                if (responseContext.Context.Request.Path.StartsWithSegments(new PathString("/js")) &&
                   !responseContext.Context.User.IsInRole("Admin"))
                {
                    responseContext.Context.Response.StatusCode = 404;
                    responseContext.Context.Response.Redirect("/error/404");
                }
            };

            return builder.UseStaticFiles(new StaticFileOptions() { OnPrepareResponse = onPrepareResponse });
        }

    }
}
