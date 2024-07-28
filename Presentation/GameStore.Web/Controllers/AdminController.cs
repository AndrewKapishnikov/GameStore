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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GameStore.Web.App.Interfaces;

namespace GameStore.Web.Controllers
{
    [Authorize(Policy = "ModeratorAndAdminRolePolicy")]
    public class AdminController : Controller
    {
        private IWebHostEnvironment hostingEnvironment;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<User> userManager;
        private readonly AbstractCategoryService categoryService;
        private readonly AbstractOrderService orderService;
        private readonly IGetGamesService getGamesService;
        private readonly IChangeGameService changeGameService;

        public AdminController(IWebHostEnvironment hostingEnvironment,
                               RoleManager<IdentityRole> roleManager,
                               UserManager<User> userManager,
                               AbstractCategoryService categoryService,
                               AbstractOrderService orderService,
                               IGetGamesService getGamesService,
                               IChangeGameService changeGameService)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.getGamesService = getGamesService;
            this.changeGameService = changeGameService;
            this.categoryService = categoryService; 
            this.orderService = orderService; 
        }
         
        [HttpGet]
        [Route("{controller}/{action}/{page:int}")]
        public async Task<IActionResult> Games(int? page, int? pagesize, int? category, string name = "",
                                               SortGameStates sort = SortGameStates.NameAsc)
        {
         
            int pageNumber = (page ?? 1);
            if (pageNumber > 100) pageNumber = 100;
            if (pageNumber < 1) pageNumber = 1;

            int categoryId = (category ?? 0);

            int pageSize = (pagesize ?? 5), count;
            if (pageSize < 1) pageSize = 1;
            if (pageSize > 100) pageSize = 100;

            IReadOnlyCollection<GameModel> gameModels;

            if (name == "" && categoryId == 0)
            {
                (gameModels, count) = await getGamesService.GetGamesForAdminByPageAsync(pageNumber, pageSize, sort);
            }
            else
            {
                (gameModels, count) = await getGamesService.GetGamesForAdminByCategoryAndNameAsync(pageNumber, pageSize, sort, name, categoryId);
            }

            var categoryModelList = await categoryService.GetAllAsync();
            AdminGamesViewModel viewModel = new AdminGamesViewModel
            {
                PageViewModel = new PaginationViewModel(count, pageNumber, pageSize),
                SortViewModel = new SortGamesViewModel(sort),
                FilterViewModel = new FilterGamesViewModel(categoryModelList.ToList(), category, pageSize, name),
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
            if (model.ReleaseDate < new DateTime(1980, 1, 01) || model.ReleaseDate >= new DateTime(2100, 1, 01))
                ModelState.AddModelError("ReleaseDate", "Дата выхода должна быть в диапазоне от января 1980 до января 2100");
            if (ModelState.IsValid)
            {
                if (titleImageFile != null && IsPermittedPictureExtention(titleImageFile.ContentType))
                {
                    model.ImageData = GetBytesImageData(titleImageFile);
                    await changeGameService.AddNewGame(model);
                    TempData["TempDataMessage"] = "Новая игра успешно добавлена в список товаров! Можете продолжить добавление игр.";
                    return RedirectToAction("addgame", "admin");

                }
                ModelState.AddModelError("ImageData", "Вам необходимо добавить главное изображение для игры");
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
            await changeGameService.RemoveGameByGameId(gameId);
            TempData["TempDataMessage"] = "Игра успешно удалена!";
            return RedirectToAction("games", "admin", new { page = 1 });
        }

        [HttpGet]
        [Route("{controller}/{action}/{gameId:int}")]
        public async Task<IActionResult> UpdateGame(int gameId)
        {
            var gameModel = await getGamesService.GetGameByIdAsync(gameId);
            ViewBag.Categories = await GetCategoriesSelectList(true, gameModel.CategoryId ?? 1);
            return View(gameModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGame(GameModel gameModel, IFormFile titleImageFile)
        {
            if(gameModel.ReleaseDate < new DateTime(1980, 1, 01) || gameModel.ReleaseDate >= new DateTime(2100, 1, 01))
                ModelState.AddModelError("ReleaseDate", "Дата выхода должна быть в диапазоне от января 1980 до января 2100");
            if (ModelState.IsValid)
            {
                if (titleImageFile != null && IsPermittedPictureExtention(titleImageFile.ContentType))
                {
                    gameModel.ImageData = GetBytesImageData(titleImageFile);
                    await changeGameService.UpdateGame(gameModel);
                    TempData["TempDataMessage"] = "Игра успешно отредактирована";
                    return RedirectToAction("games", "admin", new { page = 1 });
                }
                ModelState.AddModelError("", "Вам необходимо добавить главное изображение для игры");
            }
            ViewBag.Categories = await GetCategoriesSelectList(true, gameModel.CategoryId ?? 1);
            return View(gameModel);
        }

        [HttpPost]
        public async Task<IActionResult> UploadCKEditor(IFormFile upload)
        {
            var fullfileName = DateTime.Now.ToString("yyyyMMddHHmmss") + upload.FileName;
            var path = Path.Combine(hostingEnvironment.WebRootPath, "images\\uploads", fullfileName);
            if (upload != null && IsPermittedPictureExtention(upload.ContentType))
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
            if (uploadPicture != null && IsPermittedPictureExtention(uploadPicture.ContentType))
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

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var categoriesModels = await categoryService.GetAllAsync();
            return View(categoriesModels);
        }

        [HttpGet]
        public IActionResult AddCategory()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryModel categoryModel)
        {
            if(ModelState.IsValid)
            {
                await categoryService.AddNewCategory(categoryModel);
                TempData["TempDataMessage"] = "Новая категория успешно добавлена. Можете продолжить добавление категорий";
                return RedirectToAction("addcategory", "admin");
            }
            return View(categoryModel);
        }

        [HttpGet]
        [Route("{controller}/{action}/{categoryId:int}")]
        public IActionResult ConfirmDeleteCategory(int categoryId)
        {
            ViewBag.CategoryId = categoryId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            await categoryService.DeleteCategory(categoryId);
            TempData["TempDataMessage"] = "Категория успешно удалена!";
            return RedirectToAction("categories", "admin");
        }


        [HttpGet]
        [Route("{controller}/{action}/{page:int}")]
        public async Task<IActionResult> Orders(int? page, int? pagesize, string useremail = "", string username = "", bool makeOrder = true,
                                               SortOrderStates sort = SortOrderStates.OrderDateDesc)
        {
            int pageNumber = (page ?? 1);
            if (pageNumber > 1000) pageNumber = 1000;
            if (pageNumber < 1) pageNumber = 1;

            int pageSize = (pagesize ?? 10), count;
            if (pageSize < 1) pageSize = 1;
            if (pageSize > 100) pageSize = 100;

            IReadOnlyCollection<ShortOrderModel> orderModels;
            (orderModels, count) = await orderService.GetOrdersForAdminByUserAsync(pageNumber, pageSize, sort, username, useremail, makeOrder);
           
            AdminOrdersViewModel viewModel = new AdminOrdersViewModel
            {
                PageViewModel = new PaginationViewModel(count, pageNumber, pageSize),
                SortViewModel = new SortOrdersViewModel(sort),
                FilterViewModel = new FilterOrdersViewModel(username, useremail, pageSize, makeOrder),
                Orders = orderModels
            };

            return View(viewModel);
        }


        [HttpGet]
        [Route("{controller}/{action}/{orderId:int}")]
        public IActionResult ConfirmDeleteOrder(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            await orderService.RemoveOrderAsync(orderId);
            TempData["TempDataMessage"] = "Заказ успешно удалён!";
            return RedirectToAction("orders", "admin", new { page = 1, makeorder = false});
        }

        [HttpGet]
        [Route("{controller}/{action}/{orderId:int}")]
        public async Task<IActionResult> OrderInfo(int orderId)
        {
            var orderModel = await orderService.GetOrderForAdminAsync(orderId);
            return View(orderModel);
        }


        [HttpGet]
        [Route("{controller}/{action}/{page:int}")]
        public async Task<IActionResult> Users(int? page, int? pagesize, string userEmail = "", string userName = "", bool userConfirmed = true,
                                               SortUserStates sort = SortUserStates.UserNameDesc)
        {
            int pageNumber = (page ?? 1);
            if (pageNumber > 100) pageNumber = 100;
            if (pageNumber < 1) pageNumber = 1;

            int pageSize = (pagesize ?? 5), count;
            if (pageSize < 1) pageSize = 1;
            if (pageSize > 100) pageSize = 100;

            IQueryable<User> users = userManager.Users;

            if (!string.IsNullOrEmpty(userName))
            {
                string[] nameSurname = orderService.SplitParameterIntoNameAndSurname(ref userName);
                if(nameSurname.Length == 1)
                    users = users.Where(p => p.Name.Contains(userName) || p.Surname.Contains(userName));
                else
                    users = users.Where(p => p.Name.Contains(nameSurname[0]) && p.Surname.Contains(nameSurname[1]));
            }
            if (!string.IsNullOrEmpty(userEmail))
            {
                users = users.Where(p => p.UserName.Contains(userEmail));
            }
            if (userConfirmed)
                users = users.Where(p => p.EmailConfirmed);
            else
                users = users.Where(p => !p.EmailConfirmed);
                        
            switch (sort)
            {
                case SortUserStates.UserEmailAsc:
                    users = users.OrderByDescending(p => p.Email);
                    break;
                case SortUserStates.UserNameAsc:
                    users = users.OrderBy(p => p.Name).ThenBy(p => p.Surname);
                    break;
                case SortUserStates.UserNameDesc:
                    users = users.OrderByDescending(p => p.Name).ThenByDescending(p => p.Surname);
                    break;
                default:
                    users = users.OrderBy(p => p.Email);
                    break;
            }

            count = await users.CountAsync();
            var usersArray = await users.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            UserModel[] usersModel = usersArray.Select(MapUser).ToArray();

            AdminUsersViewModel viewModel = new AdminUsersViewModel
            {
                PageViewModel = new PaginationViewModel(count, pageNumber, pageSize),
                SortViewModel = new SortUsersViewModel(sort),
                FilterViewModel = new FilterUsersViewModel(userName, userEmail, pageSize, userConfirmed),
                Users = usersModel
            };

            return View(viewModel);
        }

        [Authorize(Policy = "AdminRolePolicy")]
        [HttpGet]
        [Route("{controller}/{action}/{userId}")]
        public IActionResult ConfirmDeleteUser(string userId)
        {
            ViewBag.UserId = userId;
            return View();
        }

        [Authorize(Policy = "AdminRolePolicy")]
        [HttpPost]
        public async  Task<IActionResult> DeleteUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                TempData["TempDataMessage"] = $"Пользователь с Id = {userId} не найден!";
                return RedirectToAction("users", "admin", new { page = 1, userconfirmed = false });
            }
            else
            {
                var result = await userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    TempData["TempDataMessage"] = "Пользователь успешно удалён!";
                    return RedirectToAction("users", "admin", new { page = 1, userconfirmed = false });
                }
            }
            TempData["TempDataMessage"] = "При удалении пользователя произошла ошибка!";
            return RedirectToAction("users", "admin", new { page = 1, userconfirmed = false });
        }

        [Authorize(Policy = "AdminRolePolicy")]
        [HttpGet]
        public IActionResult Roles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        [Authorize(Policy = "AdminRolePolicy")]
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [Authorize(Policy = "AdminRolePolicy")]
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                IdentityResult result = await roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    TempData["TempDataMessage"] = "Новая роль успешно создана!";
                    return RedirectToAction("roles", "admin");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [Authorize(Policy = "AdminRolePolicy")]
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                TempData["TempDataMessage"] = $"Роль с Id = {id} не найдена!";
                return RedirectToAction("roles", "admin");
            }
            else
            {
                try
                {
                    var result = await roleManager.DeleteAsync(role);

                    if (result.Succeeded)
                    {
                        TempData["TempDataMessage"] = "Выбранная роль успешно удалена!";
                        return RedirectToAction("roles", "admin");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    var roles = roleManager.Roles;
                    return View("Roles", roles);
                }
                catch (DbUpdateException)
                {
                   TempData["TempDataMessage"] = $"Роль {role.Name} не может быть удалена, так как существуют пользователи с этой ролью. " +
                                                 $"Если вы хотите удалить эту роль, то сначала удалите всех пользователей с этой ролью.";
                   return RedirectToAction("roles", "admin");
                }
            }
        }

        [Authorize(Policy = "AdminRolePolicy")]
        [HttpGet]
        [Route("{controller}/{action}/{userId}/{color?}")]
        public async Task<IActionResult> ManageUserRoles(string userId, int color = 1)
        {
            ViewBag.userId = userId;
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["TempDataMessage"] = $"Пользователь с Id = {userId} не найден!";
                return RedirectToAction("users", "admin", new { page = 1 });
            }
            ViewBag.Email = user.Email;
            ViewBag.Color = ChoiceColor(color);
            var model = new List<UserRolesViewModel>();
            foreach (var role in roleManager.Roles)
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if (await userManager.IsInRoleAsync(user, role.Name))
                { 
                    userRolesViewModel.IsSelected = true;
                }
                else
                {
                    userRolesViewModel.IsSelected = false;
                }
                model.Add(userRolesViewModel);
            }
            return View(model);
        }

        [Authorize(Policy = "AdminRolePolicy")]
        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                TempData["TempDataMessage"] = $"Пользователь с Id = {userId} не найден!";
                return RedirectToAction("users", "admin", new { page = 1});
            }

            var roles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Невозможно удалить существующие роли пользователя");
                return View(model);
            }

            result = await userManager.AddToRolesAsync(user,
                           model.Where(x => x.IsSelected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Невозможно добавить выбранные роли пользователю");
                return View(model);
            }

            var random = new Random().Next(1, 10);
            TempData["TempDataMessage"] = "Роли для пользователя успешно изменены!";
            return RedirectToAction("manageuserroles", "admin", new { userId = userId, color = random });
        }


        private string ChoiceColor(int color) => color switch
        {
            1 => "#ef031a",
            2 => "#2703ef",
            3 => "#16f5f5",
            4 => "#0ce763",
            5 => "#38e70c",
            6 => "#540d0d",
            7 => "#f3a414",
            8 => "#f314e2",
            9 => "#657088",
            _ => "#e93027"
        };

        private UserModel MapUser(User user)
        {
            return new UserModel()
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Phone = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                City = user.City
            };
        }


    }
}
