using GameStore.Web.HelperClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace GameStore.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = ErrorMessages.PageNotFound;
                    break;
            }
            
            return View("NotFound");
        }

        [AllowAnonymous]
        [Route("Error")]
        public IActionResult Error()
        { 
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            string exceptionPage = "Exception Page";
            var exceptionMessage = exceptionHandlerPathFeature?.Error?.Message ?? exceptionPage;
            var exceptionPath = exceptionHandlerPathFeature?.Path;

            switch (exceptionMessage)
            {
                case ExceptionMessages.NoElements:
                    ViewBag.ErrorMessage = ErrorMessages.PageNotFound;
                    return View("NotFound");
                case ExceptionMessages.EmptySession:
                    ViewBag.ErrorMessage = ErrorMessages.SelectNewProduct;
                    return View("NotFound");
                case ExceptionMessages.DatabaseNoConnectionString:
                case ExceptionMessages.NoEstablishConnectionToSQLServer:
                    ViewBag.ErrorMessage = ErrorMessages.DatabaseNoConnection;
                    return View("NoConnection");
                case ExceptionMessages.UpdatingEntriesError:
                    switch(exceptionPath)
                    {
                        case "/admin/deletecategory": return View("DeleteCategoryWarning");
                        case "/admin/deleteuser":     return View("DeleteUserWarning");
                        default:                      return View("NotFound");
                    }
            }

            ViewBag.ExceptionMessage = exceptionMessage;
            ViewBag.ExceptionPath = exceptionPath;
            string stackTrace = exceptionHandlerPathFeature?.Error?.StackTrace;
            ViewBag.StackTrace = stackTrace;

            return (exceptionMessage.Equals(exceptionPage) && exceptionPath is null && stackTrace is null) ? View("NotFound") : View("Error");
           
        }
    }
}
