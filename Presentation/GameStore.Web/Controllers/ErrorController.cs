using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
  
            if (exceptionMessage =="Sequence contains no elements.")
            {
                ViewBag.ErrorMessage = "Страница не найдена";
                return View("NotFound");
            }

            ViewBag.ExceptionMessage = exceptionMessage;
            ViewBag.ExceptionPath = exceptionPath;
            ViewBag.StackTrace = exceptionHandlerPathFeature.Error.StackTrace;

            return View("Error");
        }
    }
}
