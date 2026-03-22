using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace GameStore.Web.Components
{
    public class SlideShowViewComponent: ViewComponent
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        public SlideShowViewComponent(IWebHostEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }
        public IViewComponentResult Invoke()
        {
            var dir = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(),
                                        hostingEnvironment.WebRootPath, "images\\slider"));
            FileInfo[] images = dir.GetFiles();
            return View(images);
        }
    }
}
