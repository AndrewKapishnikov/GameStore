using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers
{
    public class ErrorController : Controller
    {
      
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult =  HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Страница не найдена";
                    break;
                
            }

            return View("NotFound");
        }

        [AllowAnonymous]
        [Route("Error")]
        public IActionResult Error()
        { 
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exceptionMessage = exceptionHandlerPathFeature.Error.Message;
            var exceptionPath = exceptionHandlerPathFeature.Path;
  
            switch (exceptionMessage)
            {
                case "Sequence contains no elements.":
                    ViewBag.ErrorMessage = "Страница не найдена";
                    return View("NotFound");
                case "Session is empty":
                    ViewBag.ErrorMessage = "Выберите новый товар";
                    return View("NotFound");
                case "An error occurred while updating the entries. See the inner exception for details.":
                    if (exceptionPath == "/admin/deletecategory")
                        return View("DeleteCategoryWarning");
                    else if (exceptionPath == "/admin/deleteuser")
                        return View("DeleteUserWarning");
                    else
                        return View("NotFound");
            }

            ViewBag.ExceptionMessage = exceptionMessage;
            ViewBag.ExceptionPath = exceptionPath;
            ViewBag.StackTrace = exceptionHandlerPathFeature.Error.StackTrace;

            return View("Error");
        }
    }
}
