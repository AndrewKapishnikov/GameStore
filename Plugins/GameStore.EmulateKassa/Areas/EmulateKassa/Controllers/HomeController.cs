using GameStore.EmulateKassa.Areas.EmulateKassa.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.EmulateKassa.Areas.EmulateKassa.Controllers
{
    [Area("EmulateKassa")]
    public class HomeController : Controller
    {
        public IActionResult Index(int orderId, string returnUri)
        {
            var model = new TestModel
            {
                OrderId = orderId,
                ReturnUri = returnUri,
            };

            return View(model);
        }

        public IActionResult Callback(int orderId, string returnUri)
        {
            var model = new TestModel
            {
                OrderId = orderId,
                ReturnUri = returnUri,
            };

            return View(model);
        }
    }
}
