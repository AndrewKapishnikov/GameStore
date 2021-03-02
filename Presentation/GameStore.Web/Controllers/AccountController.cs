using GameStore.Contractors;
using GameStore.DataEF;
using GameStore.Web.App;
using GameStore.Web.App.Models;
using GameStore.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.Web.Controllers
{
    [Authorize]
    public class AccountController: Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IEnumerable<IDeliveryService> deliveryServices;
        private readonly EmailService emailService;
        private readonly OrderService orderService;

        private const string emailKey = "emailkey";
        protected ISession Session => httpContextAccessor.HttpContext.Session;

        public AccountController (UserManager<User> userManager,
                                  SignInManager<User> signInManager,
                                  IHttpContextAccessor httpContextAccessor,
                                  IEnumerable<IDeliveryService> deliveryServices,
                                  EmailService emailService,
                                  OrderService orderService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.httpContextAccessor = httpContextAccessor;
            this.deliveryServices = deliveryServices;
            this.emailService = emailService;
            this.orderService = orderService;
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} уже используется");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model, string orderUrl)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    Surname = model.Surname,
                    Address = model.Address,
                    City = model.City,
                    PhoneNumber = model.Telephone,
                    
                };

                var result = await userManager.CreateAsync(user, model.Password);
                var userResult = await userManager.FindByNameAsync(user.UserName);
                
                if (result.Succeeded)
                {
                     await SendEmail(model.Email);
                     return View("ConfirmationEmail", new ConfirmationViewModel() { Id = userResult.Id, OrderUrl = orderUrl });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SendMessage(string orderUrl, string id)
        {
            var user = await userManager.FindByIdAsync(id);
            await SendEmail(user.Email);
            return View("ConfirmationEmail", new ConfirmationViewModel() { Id = id, OrderUrl = orderUrl });
        }

        private async Task SendEmail(string userEmail)
        {
            //var confirmationCode = 5555;
            var confirmationCode = new Random().Next(10001, 99999);
            Session.SetInt32(emailKey, confirmationCode);
            await emailService.SendEmailAsync(userEmail, "Сообщение от Games Store. Подтвердите свой аккаунт.",
                $"<div>Подтвердите регистрацию, введя в форме следующий код подтверждения: {confirmationCode}</div>");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(ConfirmationViewModel confirmationView, int? confirmationCode)
        {
            if (confirmationView.Id == null)
            {
                return View("Register");
            }
            if (confirmationCode == null)
            {
                confirmationView.Errors["confirmationCode"] = "Неверный код. Проверьте и попробуйте ещё раз.";
                return View("ConfirmationEmail", confirmationView);
            }

            int? storedCode = Session.GetInt32(emailKey);
            
            if (confirmationCode != storedCode)
            {
                confirmationView.Errors["confirmationCode"] = "Неверный код. Проверьте и попробуйте ещё раз.";
                return View("ConfirmationEmail", confirmationView);
            }

            var user = await userManager.FindByIdAsync(confirmationView.Id);
            if (user != null)
            {
                var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                await userManager.ConfirmEmailAsync(user, code);
                await signInManager.SignInAsync(user, isPersistent: false);

                if(confirmationView.OrderUrl != null && confirmationView.OrderUrl != "undefined")
                {
                    string checkStr = null;
                    var (path, orderId) = ParseOrderUrl(confirmationView.OrderUrl);
                    var (hasValue, orderModel) = await orderService.TryGetModelAsync();
                    if (hasValue) checkStr = $"/order/makeorder?orderId={orderModel.Id}";

                    if (confirmationView.OrderUrl == checkStr)
                    {
                        await orderService.SetUserForOrderAsync(user, orderId);
                        var deliveryMethods = deliveryServices.ToDictionary(service => service.Name,
                                                                            service => service.Title);
                        ViewBag.OrderId = orderId;
                        return View("../Order/DeliveryChoice", deliveryMethods);
                    }
                }

                return View("ConfirmedEmail");
            }

            return View("Register");
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl, string orderUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null && !user.EmailConfirmed &&
                            (await userManager.CheckPasswordAsync(user, model.Password)))
                {
                    //The user is not confirmed
                    return Json(new { Success = false, OrderUrl = orderUrl, Id = user.Id });
                }

                var result = await signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, true);

                if (result.Succeeded)
                {
                    if(orderUrl != null && orderUrl != "undefined")
                    {
                        string checkStr = null; 
                        var (path, orderId) = ParseOrderUrl(orderUrl);
                        var (hasValue, orderModel) = await orderService.TryGetModelAsync();
                        if (hasValue) checkStr = $"/order/makeorder?orderId={orderModel.Id}";

                        if (orderUrl == checkStr)
                        {
                            //User confirmed and make order
                            return Json(new { Success = true, MakeOrder = true, Path = path, OrderId = orderId });
                        }
                    }
                    //User confirmed and don't make order
                    return  Json(new { Success = true, ReturnUrl = returnUrl });
                }
                if(result.IsLockedOut)
                {
                    ModelState.AddModelError("RegisterError", "Превышено количество попыток входа. Аккаунт блокирован на 15 минут");
                }
                ModelState.AddModelError("RegisterError", "Неверный логин или пароль");
            }

            return BadRequest(ModelState);
        }

        private (string path, int orderId) ParseOrderUrl(string orderUrl)
        {
            string path = null;
            int orderId = default;
            if (orderUrl == null) return ("", orderId);
            string[] orderStrings = orderUrl.Split("?");
            if (orderStrings.Length >= 2) path = orderStrings[0];
            string[] idStrings = orderUrl.Split("=");
            if (idStrings.Length >= 2) int.TryParse(idStrings[1], out orderId);

            return (path, orderId);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [Authorize(Policy = "ModeratorAndAdminRolePolicy")]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && await userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);

                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                            new { email = model.Email, token = token }, Request.Scheme);

                    await emailService.SendEmailAsync(model.Email, "Сменить пароль",
                       $"Перейдите по ссылке, чтобы сбросить старый пароль и задать новый: <a href='{passwordResetLink}'>Ссылка для смены пароля</a>");

                    return View("ForgotPasswordConfirmation");
                }

                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
                ModelState.AddModelError("", "Неверный токен сброса пароля");

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        if (await userManager.IsLockedOutAsync(user))
                        {
                            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                        }
                        
                        TempData["TempDataMessage"] = $"Пароль для учётной записи {model.Email} успешно изменён!";
                        return RedirectToAction("index", "home");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

                TempData["TempDataMessage"] = $"Пользователь с электронной почтой {model.Email} не зарегистрирован!";
                return RedirectToAction("index", "home");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> PrivateOffice()
        {
            string name = User.Identity.Name;
            var user = await userManager.FindByNameAsync(name);
            var orders = await orderService.GetOrdersForUser(user);
            var userModel = new UserModel(){ Id = user.Id,
                                             Name = user.Name,
                                             Surname = user.Surname,
                                             Email = user.Email,
                                             Phone = user.PhoneNumber,
                                             EmailConfirmed = user.EmailConfirmed,
                                             City = user.City,
                                             Address = user.Address
                                           };
            var model = new UserWithOrderModel()
            {
                UserModel = userModel,
                UserOrdersModels = orders
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetails(int id)
        {
           var order = await orderService.GetOrderDetailAsync(id);
           return View(order);
        }


        [HttpGet]
        public async Task<IActionResult> ChangeUserInfo()
        {
            string name = User.Identity.Name;
            var user = await userManager.FindByNameAsync(name);
            var userModel = new ChangeUserViewModel()
            {
                Email = user.UserName,
                Name = user.Name,
                Surname = user.Surname,
                Address = user.Address,
                City = user.City,
                Telephone = user.PhoneNumber
            };
            return View(userModel);
        }


        [HttpPost]
        public async Task<IActionResult> ChangeUserInfo(ChangeUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    user.Name = model.Name;
                    user.Surname = model.Surname;
                    user.PhoneNumber = model.Telephone;
                    user.City = model.City;
                    user.Address = model.Address;
                    var result = await userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        TempData["TempDataMessage"] = "Информация о пользователе успешно обновлена!";
                        return RedirectToAction("privateoffice", "account");
                    }
                    TempData["TempDataMessage"] = "При изменении информации о пользователе произошла ошибка!";
                    return RedirectToAction("privateoffice", "account");
                }
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("register","account");
                }

                var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                TempData["TempDataMessage"] = "Пароль успешно изменён!";
                return RedirectToAction("privateoffice", "account");
            }

            return View(model);
        }


    }
}
