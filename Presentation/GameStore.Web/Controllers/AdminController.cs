using GameStore.DataEF;
using GameStore.Web.App;
using GameStore.Web.App.Models;
using GameStore.Web.Models.AdminPanelModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Web.Controllers
{
    [Authorize(Policy = "AdminRolePolicy")]
    public class AdminController : Controller
    {

        private IWebHostEnvironment hostingEnvironment;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<User> userManager;
        private readonly CategoryService categoryService;
        private readonly GameService gameService;
        private readonly OrderService orderService;
        public AdminController(IWebHostEnvironment hostingEnvironment,
                               RoleManager<IdentityRole> roleManager,
                               UserManager<User> userManager,
                               CategoryService categoryService,
                               OrderService orderService,
                               GameService gameService)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.gameService = gameService;
            this.categoryService = categoryService;
            this.orderService = orderService;
        }


        [HttpGet]
        [Route("{controller}/{action}/{page:int}")]
        public async Task<IActionResult> Games(int? page, int? pagesize, int? category, string name = "",
                                               SortGameState sortGame = SortGameState.NameAsc)
        {
            int pageNumber = (page ?? 1);
            if (pageNumber > 100) pageNumber = 100;
            if (pageNumber < 1) pageNumber = 1;

            int categoryId = (category ?? 0);

            int pageSize = (pagesize ?? 5), count;
            if (pageSize < 1) pageNumber = 1;
            if (pageSize > 100) pageSize = 100;

            IReadOnlyCollection<GameModel> gameModels;

            if (name == "" && categoryId == 0)
            {
                (gameModels, count) = await gameService.GetGamesForAdminByPageAsync(pageNumber, pageSize, sortGame);
            }
            else
            {
                (gameModels, count) = await gameService.GetGamesForAdminByCategoryAndNameAsync(pageNumber, pageSize, sortGame, name, categoryId);
            }

            var categoryModelList = await categoryService.GetAllAsync();
            AdminGamesViewModel viewModel = new AdminGamesViewModel
            {
                PageViewModel = new PaginationViewModel(count, pageNumber, pageSize),
                SortViewModel = new SortViewModel(sortGame),
                FilterViewModel = new FilterViewModel(categoryModelList.ToList(), category, pageSize, name),
                Games = gameModels
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AddGame()
        {
            ViewBag.Categories = await GetCategoriesSelectList(false);
            return View();
        }

        private async Task<SelectList> GetCategoriesSelectList(bool updateMode, int categoryId = 0)
        {
            var categories = await categoryService.GetAllAsync();
            return updateMode ? new SelectList(categories, "CategoryId", "Name", categoryId) 
                              : new SelectList(categories, "CategoryId", "Name");
        }

        [HttpPost]
        public async Task<IActionResult> AddGame(GameModel model, IFormFile titleImageFile)
        {
            if (ModelState.IsValid)
            {
                if (titleImageFile != null && IsPermittedPictureExtention(titleImageFile.ContentType))
                {
                    model.ImageData = GetBytesImageData(titleImageFile);
                    await gameService.AddNewGame(model);
                    TempData["TempDataMessage"] = "Новая игра успешно добавлена в список товаров! Можете продолжить добавление игр.";
                    return RedirectToAction("addgame", "admin");

                }
                ModelState.AddModelError("No picture", "Вам необходимо добавить главное изображение для игры");
            }
            ViewBag.Categories = await GetCategoriesSelectList(true, model.CategoryId ?? 1);
            return View(model);
        }
        private bool IsPermittedPictureExtention (string contentType)
        {
            string[] permittedExtensions = { "image/jpeg", "image/png", "image/gif" };
            bool permitted = false;
            foreach (var item in permittedExtensions)
            {
                if (contentType == item)
                {
                    permitted = true;
                    break;
                }
            }
            return permitted;
        }
        private byte[] GetBytesImageData(IFormFile titleImageFile)
        {
            byte[] imageData = null;
            using (var binaryReader = new BinaryReader(titleImageFile.OpenReadStream()))
            {
                imageData = binaryReader.ReadBytes((int)titleImageFile.Length);
            }
            return imageData;
        }


        [HttpGet]
        [Route("{controller}/{action}/{gameId:int}")]
        public IActionResult ConfirmGame(int gameId)
        {
            ViewBag.GameId = gameId;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteGame(int gameId)
        {
            await gameService.RemoveGame(gameId);
            TempData["TempDataMessage"] = "Игра успешно удалена!";
            return RedirectToAction("games", "admin", new { page = 1 });
        }


        [HttpGet]
        [Route("{controller}/{action}/{gameId:int}")]
        public async Task<IActionResult> UpdateGame(int gameId)
        {
            var gameModel = await gameService.GetGameByIdAsync(gameId);
            ViewBag.Categories = await GetCategoriesSelectList(true, gameModel.CategoryId ?? 1);
            return View(gameModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGame(GameModel gameModel, IFormFile titleImageFile)
        {
            if (ModelState.IsValid)
            {
                if (titleImageFile != null && IsPermittedPictureExtention(titleImageFile.ContentType))
                {
                    gameModel.ImageData = GetBytesImageData(titleImageFile);
                    await gameService.UpdateGame(gameModel);
                    TempData["TempDataMessage"] = "Игра успешно отредактированна";
                    return RedirectToAction("games", "admin", new { page = 1 });
                }
            }
            ViewBag.Categories = await GetCategoriesSelectList(true, gameModel.CategoryId ?? 1);
            return View(gameModel);
        }

        [HttpPost]
        public async Task<IActionResult> UploadCKEditor(IFormFile upload)
        {
            var fullfileName = DateTime.Now.ToString("yyyyMMddHHmmss") + upload.FileName;
            var path = Path.Combine(hostingEnvironment.WebRootPath, "images\\uploads", fullfileName);
            if (upload != null)
            {
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await upload.CopyToAsync(stream);
                }
                return new JsonResult(new
                {
                    uploaded = 1,
                    fileName = fullfileName,
                    url = "/images/uploads/" + fullfileName
                });

                //var url = $"{"/images/uploads/"}{fullfileName}";
                //var successMessage = "image is uploaded successfully";
                //var success = JsonConvert.DeserializeObject("{ 'uploaded': 1,'fileName': \"" + fullfileName + "\",'url': \"" + url + "\", 'error': { 'message': \"" + successMessage + "\"}}");
                //return Json(success);
            }
            return BadRequest();
        }

      
        [HttpGet]
        public IActionResult FileBrowser()
        {
            ViewBag.fileInfos = GetFilesByDirectory("images\\uploads");
            return View("FileBrowser");
        }

        [HttpPost]
        public async Task<IActionResult> UploadPictureFileBrowser(IFormFile uploadPicture)
        {
            if (uploadPicture != null)
            {
                var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + uploadPicture.FileName;
                var path = Path.Combine(hostingEnvironment.WebRootPath, "images\\uploads", fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await uploadPicture.CopyToAsync(stream);
                }
                //ViewBag.fileInfos = GetFilesByDirectory("images\\uploads");
                //return View("FileBrowser");
                return Json(new { data = fileName });
            }
            return BadRequest();
        }

        [HttpPost]
        public IActionResult DeleteImage(string name)
        {
            if (name != null)
            {
                var path = Path.Combine(hostingEnvironment.WebRootPath, "images\\uploads", name);
                FileInfo fileInfo = new FileInfo(path);
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                    // alternative with the File class
                    // File.Delete(path);
                }
            }
            ViewBag.fileInfos = GetFilesByDirectory("images\\uploads");
            return View("FileBrowser");
        }

        private FileInfo[] GetFilesByDirectory(string path)
        {
            var dir = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(),
                                       hostingEnvironment.WebRootPath, path));
            return dir.GetFiles();
        }
    }
}
