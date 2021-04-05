using GameStore.Web.App;
using GameStore.Web.App.Interfaces;
using NSubstitute;
using NUnit.Framework;
using GameStore.Web.Controllers;
using FluentAssertions;
using GameStore.DataEF;
using GameStore.Contractors;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using GameStore.Web.Models;
using Moq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using GameStore.Web.App.Models;

namespace GameStore.UnitTests.Presentation
{
    [TestFixture]
    public class AccountControllerTests: BaseTest
    {
        [Test]
        public void Register_ReturnsViewRegister()
        {
            //Arrange
            var userManager = FaketUserManager<User>();
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            //Act
            var actionResult = accountController.Register();
            var viewResult = actionResult as ViewResult;
            //Assert
            viewResult.ViewName.Should().Be("Register");
        }

        [Test]
        public void IsEmailInUse_WhenUserByEmailExists_Returns_JsonDataEmailInUse()
        {
            User user = CreateFakeUser();
            var userManager = FaketUserManager<User>();
                userManager.FindByEmailAsync(user.Email).Returns(user);
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            //Act
            var task = accountController.IsEmailInUse(user.Email);
            var jsonResult = task.Result as JsonResult;
            //Assert
            Assert.Multiple(() =>
            {
                jsonResult.Value.Should().Be($"Email {user.Email} уже используется");
                userManager.Received(1).FindByEmailAsync(user.Email);
            });

        }

        [Test]
        public void IsEmailInUse_WhenUserByEmailNotExists_Returns_JsonValueTrue()
        {
            User user = CreateFakeUser();
            var userManager = FaketUserManager<User>();
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            //Act
            var task = accountController.IsEmailInUse(user.Email);
            var jsonResult = task.Result as JsonResult;
            //Assert
            jsonResult.Value.Should().Be(true);
        }

        [Test]
        public void Register_WhenModelStateIsValid_And_CreateResultSuccess_ReturnsViewConfirmationEmail()
        {

            User user = CreateFakeUser();
            var registerViewModel = Map(user);
            string orderUrl = Faker.Random.String2(5, 20);
            var userManager = FaketUserManager<User>();
                userManager.CreateAsync(Arg.Any<User>(), registerViewModel.Password).Returns(IdentityResult.Success);
                userManager.FindByNameAsync(user.UserName).Returns(user);
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            //Act
            var task = accountController.Register(registerViewModel, orderUrl);
            var viewResult = task.Result as ViewResult;
            var model = viewResult.Model as ConfirmationViewModel;

            Assert.Multiple(() =>
            {
                viewResult.ViewName.Should().Be("ConfirmationEmail");
                model.OrderUrl.Should().Be(orderUrl);
                userManager.Received(1).CreateAsync(Arg.Any<User>(), registerViewModel.Password);
                userManager.Received(1).FindByNameAsync(user.UserName);
            });
        }


        [Test]
        public void Register_WhenModelStateIsValid_And_CreateResultNotSuccess_ReturnsRegisterViewModel()
        {

            User user = CreateFakeUser();
            var registerViewModel = Map(user);
            string orderUrl = Faker.Random.String2(5, 20);
            string description = "Some Error";
            var identityErrors = new IdentityError[] { new IdentityError() { Description = description} };
            var userManager = FaketUserManager<User>();
                userManager.CreateAsync(Arg.Any<User>(), registerViewModel.Password).Returns(IdentityResult.Failed(identityErrors));
                userManager.FindByNameAsync(user.UserName).Returns(user);
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            //Act
            var task = accountController.Register(registerViewModel, orderUrl);
            var viewResult = task.Result as ViewResult;
            var model = viewResult.Model as RegisterViewModel;

            Assert.Multiple(() =>
            {
                viewResult.ViewName.Should().NotBe("ConfirmationEmail");
                model.Name.Should().Be(registerViewModel.Name);
                model.Telephone.Should().Be(registerViewModel.Telephone);
            });
        }

        [Test]
        public void SendMessage_ReturnsViewConfirmationEmail()
        {
            string orderUrl = Faker.Random.String2(5, 20);
            string id = Faker.Random.String2(3, 10);
            var user = CreateFakeUser();
            var userManager = FaketUserManager<User>();
                userManager.FindByIdAsync(id).Returns(user);
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            //Act
            var task = accountController.SendMessage(orderUrl, id);
            var viewResult = task.Result as ViewResult;
            var model = viewResult.Model as ConfirmationViewModel;
            //Assert
            Assert.Multiple(() =>
            {
                viewResult.ViewName.Should().Be("ConfirmationEmail");
                model.OrderUrl.Should().Be(orderUrl);
            });

        }

        [Test]
        public void ConfirmEmail_WhenConfirmationViewIdIsNull_ReturnsViewRegister()
        {
            //Arrange
            var confirmationViewModel = new ConfirmationViewModel() { Id = null };
            int confirmationCode = Faker.Random.Int(1, 100);
            var userManager = FaketUserManager<User>();
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            //Act
            var task = accountController.ConfirmEmail(confirmationViewModel, confirmationCode);
            var viewResult = task.Result as ViewResult;
            //Assert
            viewResult.ViewName.Should().Be("Register");
        }

        [Test]
        public void ConfirmEmail_WhenConfirmationCodeIsNull_ReturnsViewConfirmationEmail()
        {
            var confirmationViewModel = new ConfirmationViewModel() { Id = "1" };
            int? confirmationCode = null;
            string errorMessage = "Неверный код. Проверьте и попробуйте ещё раз.";
            var userManager = FaketUserManager<User>();
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            //Act
            var task = accountController.ConfirmEmail(confirmationViewModel, confirmationCode);
            var viewResult = task.Result as ViewResult;
            var model = viewResult.Model as ConfirmationViewModel;
            //Assert
            Assert.Multiple(() =>
            {
                viewResult.ViewName.Should().Be("ConfirmationEmail");
                model.Errors["confirmationCode"].Should().Be(errorMessage);
            });
        }


        [Test]
        public void ConfirmEmail_WhenConfirmationCodeNotEqualSessionStoredCode_ReturnsViewConfirmationEmailWithError()
        {
            var confirmationViewModel = new ConfirmationViewModel() { Id = "1" };
            int confirmationCode = Faker.Random.Int(1, 100);
            string errorMessage = "Неверный код. Проверьте и попробуйте ещё раз.";
            var userManager = FaketUserManager<User>();
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            //Act
            var task = accountController.ConfirmEmail(confirmationViewModel, confirmationCode);
            var viewResult = task.Result as ViewResult;
            var model = viewResult.Model as ConfirmationViewModel;
            //Assert
            Assert.Multiple(() =>
            {
                viewResult.ViewName.Should().Be("ConfirmationEmail");
                model.Errors["confirmationCode"].Should().Be(errorMessage);
            });
        }

        [Test]
        public void Login_WhenModelStateIsValid_And_UserIsNotConfirmed_ReturnsJsonSuccessFalse()
        {
            var loginViewModel = CreateFakeLoginViewModel();
            var returnUrl = Faker.Random.String2(5, 20);
            var orderUrl = Faker.Random.String2(5, 20);
            User user = CreateFakeUser();
            var userManager = FaketUserManager<User>();
                userManager.FindByEmailAsync(loginViewModel.Email).Returns(user);
                userManager.CheckPasswordAsync(user, loginViewModel.Password).Returns(true);
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            //Act
            var task = accountController.Login(loginViewModel, returnUrl, orderUrl);
            var jsonResult = task.Result as JsonResult;
            //Assert
            Assert.Multiple(() =>
            {
                jsonResult.Value.ToString().Contains("Success = False").Should().Be(true);
                userManager.Received(1).FindByEmailAsync(loginViewModel.Email);
            });
         }

        [Test]
        public void Login_WhenModelStateIsValid_And_UserIsNotConfirmed_And_ResultSucceeded_ReturnsJsonMakeOrderIsTrue()
        {
            var loginViewModel = CreateFakeLoginViewModel();
            var returnUrl = Faker.Random.String2(5, 20);
            var order = Order.Mapper.Map(fakeOrderDto);
            var orderModel = OrderService.Map(order);
            var orderUrl = $"/order/makeorder?orderId={order.Id}";
            User user = CreateFakeUser(); user.EmailConfirmed = true;
            var userManager = FaketUserManager<User>();
                userManager.FindByEmailAsync(loginViewModel.Email).Returns(user);
                userManager.CheckPasswordAsync(user, loginViewModel.Password).Returns(true);
            var signInManager = FakeSignInManager<User>();
                signInManager.PasswordSignInAsync(Arg.Any<string>(), loginViewModel.Password, true, true)
                              .Returns(Microsoft.AspNetCore.Identity.SignInResult.Success);
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
                orderService.TryGetModelAsync().Returns((true, orderModel));
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            //Act
            var task = accountController.Login(loginViewModel, returnUrl, orderUrl);
            var jsonResult = task.Result as JsonResult;
            //Assert
            Assert.Multiple(() =>
            {
                jsonResult.Value.ToString().Contains("MakeOrder = True").Should().Be(true);
                signInManager.Received(1).PasswordSignInAsync(Arg.Any<string>(), loginViewModel.Password, true, true);
            });
         
        }


        [Test]
        public void Login_WhenModelStateIsValid_And_UserIsNotConfirmed_And_ResultSucceeded_And_OrderUrlIsNull_ReturnsJsonWithoutMakeOrderIsTrue()
        {
            var loginViewModel = CreateFakeLoginViewModel();
            var returnUrl = Faker.Random.String2(5, 20);
            User user = CreateFakeUser(); user.EmailConfirmed = true;
            var userManager = FaketUserManager<User>();
                userManager.FindByEmailAsync(loginViewModel.Email).Returns(user);
                userManager.CheckPasswordAsync(user, loginViewModel.Password).Returns(true);
            var signInManager = FakeSignInManager<User>();
                signInManager.PasswordSignInAsync(Arg.Any<string>(), loginViewModel.Password, true, true)
                             .Returns(Microsoft.AspNetCore.Identity.SignInResult.Success);
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            //Act
            var task = accountController.Login(loginViewModel, returnUrl, null);
            var jsonResult = task.Result as JsonResult;
            //Assert
            Assert.Multiple(() =>
            {
                jsonResult.Value.ToString().Contains("MakeOrder = True").Should().BeFalse();
                userManager.Received(0).CheckPasswordAsync(user, loginViewModel.Password);
            });
        }


        [Test]
        public void Login_WhenModelStateIsValid_And_UserIsNotConfirmed_And_ResultIsLockedOut_ReturnsBadRequest()
        {
            var loginViewModel = CreateFakeLoginViewModel();
            var returnUrl = Faker.Random.String2(5, 20);
            User user = CreateFakeUser(); user.EmailConfirmed = true;
            var userManager = FaketUserManager<User>();
                userManager.FindByEmailAsync(loginViewModel.Email).Returns(user);
                userManager.CheckPasswordAsync(user, loginViewModel.Password).Returns(true);
            var signInManager = FakeSignInManager<User>();
                signInManager.PasswordSignInAsync(Arg.Any<string>(), loginViewModel.Password, true, true)
                             .Returns(Microsoft.AspNetCore.Identity.SignInResult.LockedOut);
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            //Act
            var task = accountController.Login(loginViewModel, returnUrl, null);
            var badResult = task.Result as BadRequestObjectResult;

            //Assert
            badResult.StatusCode.Value.Should().Be(400);
        }

        [Test]
        public void Logout_RedirectToControllerHomeViewIndex()
        {
            //Arrange
            var userManager = FaketUserManager<User>();
            var signInManager = FakeSignInManager<User>();
                signInManager.SignOutAsync().Returns(Task.CompletedTask);
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            //Act
            var actionResult = accountController.Logout();
            var redirectResult = actionResult.Result as RedirectToActionResult;
            //Assert
            redirectResult.ActionName.Should().Be("index");
            Assert.Multiple(() =>
            {
                redirectResult.ActionName.Should().Be("index");
                redirectResult.ControllerName.Should().Be("home");
            });
        }

        [Test]
        public void AccessDenied_ReturnsViewAccessDenied()
        {
            //Arrange
            var userManager = FaketUserManager<User>();
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            //Act
            var actionResult = accountController.AccessDenied();
            var viewResult = actionResult as ViewResult;
            //Assert
            viewResult.ViewName.Should().Be("DeniedAccess");
        }

        [Test]
        public void ForgotPassword_ReturnsViewForgotPassword()
        {
            //Arrange
            var userManager = FaketUserManager<User>();
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            //Act
            var actionResult = accountController.ForgotPassword();
            var viewResult = actionResult as ViewResult;
            //Assert
            viewResult.ViewName.Should().Be("ForgotPassword");
        }

        [Test]
        public void ForgotPassword_WhenModelIsValidAndEmailConfirmed_ReturnsForgotPasswordConfirmation()
        {
            var fakeModel = new ForgotPasswordViewModel() { Email = Faker.Person.Email };
            var user = CreateFakeUser();
            var userManager = FaketUserManager<User>();
                userManager.FindByEmailAsync(fakeModel.Email).Returns(user);
                userManager.IsEmailConfirmedAsync(user).Returns(true);
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            //Act
            var actionResult = accountController.ForgotPassword(fakeModel);
            var viewResult = actionResult.Result as ViewResult;

            viewResult.ViewName.Should().Be("ForgotPasswordConfirmation");
        }

        [Test]
        public void ForgotPassword_WhenModelIsNotValid_ReturnsForgotPasswordViewModel()
        {
            var fakeModel = new ForgotPasswordViewModel() { Email = Faker.Person.Email };
            var userManager = FaketUserManager<User>();
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            accountController.ModelState.AddModelError("Test", "TestMessage");
            //Act
            var actionResult = accountController.ForgotPassword(fakeModel);
            var viewResult = actionResult.Result as ViewResult;
            var model = viewResult.Model as ForgotPasswordViewModel;
            //Assert
            model.Email.Should().Be(fakeModel.Email);
        }

        [Test]
        public void ResetPassword_WhenTokenOrEmailIsNull_AddModelError()
        {
            var userManager = FaketUserManager<User>();
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            //Act
            var actionResult = accountController.ResetPassword(null, null);
            var viewResult = actionResult as ViewResult;
            //Assert
            viewResult.ViewData.ModelState.ErrorCount.Should().Be(1);
        }

        [Test]
        public void ResetPassword_WhenUserNotNullAndResetPasswordSucceeded_RedirectToHomeControllerViewIndex()
        {
            var user = CreateFakeUser();
            var model = new ResetPasswordViewModel() { Email = Faker.Person.Email, Password = Faker.Random.String2(6, 20), Token = Faker.Random.String2(10, 20) };
            var userManager = FaketUserManager<User>();
                userManager.FindByEmailAsync(model.Email).Returns(user);
                userManager.ResetPasswordAsync(user, model.Token, model.Password).Returns(IdentityResult.Success);
                userManager.IsLockedOutAsync(user).Returns(true);
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            accountController.TempData = tempData;
            //Act
            var task = accountController.ResetPassword(model);
            var redirectResult = task.Result as RedirectToActionResult;

            Assert.Multiple(() => {
                redirectResult.ControllerName.Should().Be("home");
                redirectResult.ActionName.Should().Be("index");
                userManager.Received(1).FindByEmailAsync(model.Email);
                userManager.Received(1).ResetPasswordAsync(user, model.Token, model.Password);
                userManager.Received(1).IsLockedOutAsync(user);
            });
        }

        [Test]
        public void ResetPassword_WhenUserNotNullAndResetPasswordFailed_AddModelError()
        {
            var user = CreateFakeUser();
            var model = new ResetPasswordViewModel() { Email = Faker.Person.Email, Password = Faker.Random.String2(6, 20), Token = Faker.Random.String2(10, 20) };
            string description = "Some Error";
            var identityErrors = new IdentityError[] { new IdentityError() { Description = description } };
            var userManager = FaketUserManager<User>();
                userManager.FindByEmailAsync(model.Email).Returns(user);
                userManager.ResetPasswordAsync(user, model.Token, model.Password).Returns(IdentityResult.Failed(identityErrors));
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            //Act
            var task = accountController.ResetPassword(model);
            var viewResult = task.Result as ViewResult;
            var viewModel = viewResult.Model as ResetPasswordViewModel;

            Assert.Multiple(() => {
                viewResult.ViewData.ModelState.ErrorCount.Should().Be(1);
                viewModel.Token.Should().Be(model.Token);
                viewModel.Email.Should().Be(model.Email);
            });
        }


        [Test]
        public void ResetPassword_WhenUserNull_RedirectToActionIndexControllerHome()
        {
            var model = new ResetPasswordViewModel() { Email = Faker.Person.Email, Password = Faker.Random.String2(6, 20), Token = Faker.Random.String2(10, 20) };
            var userManager = FaketUserManager<User>();
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            accountController.TempData = tempData;
            //Act
            var task = accountController.ResetPassword(model);
            var redirectResult = task.Result as RedirectToActionResult;

            redirectResult.ControllerName.Should().Be("home");
            redirectResult.ActionName.Should().Be("index");
        }

        [Test]
        public void ResetPassword_WhenModelIsNotValid_ReturnsResetPasswordViewModel()
        {
            var model = new ResetPasswordViewModel() { Email = Faker.Person.Email, Password = Faker.Random.String2(6, 20), Token = Faker.Random.String2(10, 20) };
            var userManager = FaketUserManager<User>();
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            accountController.ModelState.AddModelError("Test", "TestMessage");

            //Act
            var task = accountController.ResetPassword(model);
            var viewResult = task.Result as ViewResult;
            var viewModel = viewResult.Model as ResetPasswordViewModel;

            viewModel.Email.Should().Be(model.Email);
        }

        [Test]
        public void PrivateOffice_ReturnsUserWithOrderModel()
        {
            string userName = Faker.Person.UserName;
            var user = new User() { UserName = userName, PhoneNumber = Faker.Person.Phone };
            var claims = new List<Claim>() { new Claim(ClaimTypes.Name, userName) };
            var identity = new ClaimsIdentity(claims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var userManager = FaketUserManager<User>();
                userManager.FindByNameAsync(userName).Returns(user);
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);

            var context = new ControllerContext { HttpContext = new DefaultHttpContext { User = claimsPrincipal } };
            accountController.ControllerContext = context;
            //Act
            var task = accountController.PrivateOffice();
            var viewResult = task.Result as ViewResult;
            var viewModel = viewResult.Model as UserWithOrderModel;

            Assert.Multiple(() =>
            {
                viewModel.UserModel.Name.Should().Be(user.Name);
                viewModel.UserModel.Phone.Should().Be(user.PhoneNumber);
                userManager.Received(1).FindByNameAsync(userName);
            });
        }

        [Test]
        public void OrderDetails_ReturnsOrderModel()
        {
            int id = Faker.Random.Int(1, 1000);
            var order = Order.Mapper.Map(fakeOrderDto);
            var orderModel = OrderService.Map(order);
            var userManager = FaketUserManager<User>();
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
                orderService.GetOrderDetailAsync(id).Returns(orderModel);
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            //Act
            var task = accountController.OrderDetails(id);
            var viewResult = task.Result as ViewResult;
            var viewModel = viewResult.Model as OrderModel;
            //Assert
            Assert.Multiple(() =>
            {
                orderService.Received(1).GetOrderDetailAsync(id);
                viewModel.Id.Should().Be(orderModel.Id);
                viewModel.TotalPrice.Should().Be(orderModel.TotalPrice);
            });
        }

        [Test]
        public void ChangeUserInfo_ReturnsChangeUserViewModel()
        {
            string userName = Faker.Person.UserName;
            var user = new User() { UserName = userName, PhoneNumber = Faker.Person.Phone };
            var claims = new List<Claim>() { new Claim(ClaimTypes.Name, userName) };
            var identity = new ClaimsIdentity(claims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var userManager = FaketUserManager<User>();
                userManager.FindByNameAsync(userName).Returns(user);
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            var context = new ControllerContext { HttpContext = new DefaultHttpContext { User = claimsPrincipal } };
            accountController.ControllerContext = context;
            //Act
            var task = accountController.ChangeUserInfo();
            var viewResult = task.Result as ViewResult;
            var viewModel = viewResult.Model as ChangeUserViewModel;
            Assert.Multiple(()=>
            {
                viewModel.Email.Should().Be(user.UserName);
                viewModel.Telephone.Should().Be(user.PhoneNumber);
                userManager.Received(1).FindByNameAsync(userName);
            });
        }

        [Test]
        public void ChangeUserInfo_WhenUserIsNotNullAndUserUpdateResultSucceeded_ReturnsSuceededTempDataMessage()
        {
            var model = CreateChangeUserViewModel();
            User user = CreateFakeUser();
            var userManager = FaketUserManager<User>();
                userManager.FindByNameAsync(model.Email).Returns(user);
                userManager.UpdateAsync(user).Returns(IdentityResult.Success);
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            accountController.TempData = tempData;
            //Act
            var task = accountController.ChangeUserInfo(model);
            var redirectResult = task.Result as RedirectToActionResult;

            Assert.Multiple(() =>
            {
                accountController.TempData["TempDataMessage"].Should().Be("Информация о пользователе успешно обновлена!");
                redirectResult.ActionName.Should().Be("privateoffice");
                redirectResult.ControllerName.Should().Be("account");
                userManager.Received(1).UpdateAsync(user);
            });
        }

        [Test]
        public void ChangeUserInfo_WhenUserIsNotNullAndUserUpdateResultFailed_ReturnsFailedTempDataMessage()
        {
            var model = CreateChangeUserViewModel();
            User user = CreateFakeUser();
            string description = "Some Error";
            var identityErrors = new IdentityError[] { new IdentityError() { Description = description } };
            var userManager = FaketUserManager<User>();
                userManager.FindByNameAsync(model.Email).Returns(user);
                userManager.UpdateAsync(user).Returns(IdentityResult.Failed(identityErrors));
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            accountController.TempData = tempData;
            //Act
            var task = accountController.ChangeUserInfo(model);
            var redirectResult = task.Result as RedirectToActionResult;

            Assert.Multiple(() =>
            {
                accountController.TempData["TempDataMessage"].Should().Be("При изменении информации о пользователе произошла ошибка!");
                redirectResult.ActionName.Should().Be("privateoffice");
                userManager.FindByNameAsync(model.Email);
            });
        }

        [Test]
        public void ChangeUserInfo_WhenModelIsNotValid_ReturnsChangeUserViewModel()
        {
            var model = CreateChangeUserViewModel();
            var userManager = FaketUserManager<User>();
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            accountController.ModelState.AddModelError("Test", "TestMessage");
            //Act
            var task = accountController.ChangeUserInfo(model);
            var viewResult = task.Result as ViewResult;
            var viewModel = viewResult.Model as ChangeUserViewModel;
            //Assert
            viewModel.Name.Should().Be(model.Name);
        }

        [Test]
        public void ChangePassword_ReturnsViewChangePassword()
        {
            var userManager = FaketUserManager<User>();
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            //Act
            var actionResult = accountController.ChangePassword();
            var viewResult = actionResult as ViewResult;
            //Assert
            viewResult.ViewName.Should().Be("ChangePassword");
        }

        [Test]
        public void ChangePassword_WhenUserIsNotNullAndChangePasswordSucceeded_ReturnsSuceessTempData()
        {
            var model = new ChangePasswordViewModel() { CurrentPassword = Faker.Random.String2(6, 20), NewPassword = Faker.Random.String2(6, 20) };
            var user = CreateFakeUser();
            var userManager = FaketUserManager<User>();
                userManager.GetUserAsync(Arg.Any<ClaimsPrincipal>()).Returns(user);
                userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword).Returns(IdentityResult.Success);
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            accountController.TempData = tempData;
            //Act
            var task = accountController.ChangePassword(model);
            var redirectResult = task.Result as RedirectToActionResult;

            Assert.Multiple(() =>
            {
                accountController.TempData["TempDataMessage"].Should().Be("Пароль успешно изменён!");
                redirectResult.ActionName.Should().Be("privateoffice");
                userManager.Received(1).GetUserAsync(Arg.Any<ClaimsPrincipal>());
                userManager.Received(1).ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            });
        }


        [Test]
        public void ChangePassword_WhenUserIsNotNullAndChangePasswordFailed_ReturnsViewChangePassword()
        {
            var model = new ChangePasswordViewModel() { CurrentPassword = Faker.Random.String2(6, 20), NewPassword = Faker.Random.String2(6, 20) };
            var user = CreateFakeUser();
            string description = "Some Error";
            var identityErrors = new IdentityError[] { new IdentityError() { Description = description } };
            var userManager = FaketUserManager<User>();
                userManager.GetUserAsync(Arg.Any<ClaimsPrincipal>()).Returns(user);
                userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword).Returns(IdentityResult.Failed(identityErrors));
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            //Act
            var task = accountController.ChangePassword(model);
            var viewResult = task.Result as ViewResult;
            //Assert
            viewResult.ViewName.Should().Be("ChangePassword");
        }

        [Test]
        public void ChangePassword_WhenUserIsNull_RedirectsToActionRegisterControllerAccount()
        {
            var model = new ChangePasswordViewModel() { CurrentPassword = Faker.Random.String2(6, 20), NewPassword = Faker.Random.String2(6, 20) };
            var userManager = FaketUserManager<User>();
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            //Act
            var task = accountController.ChangePassword(model);
            var redirectResult = task.Result as RedirectToActionResult;
            //Assert
            Assert.Multiple(() =>
            {
                redirectResult.ActionName.Should().Be("register");
                redirectResult.ControllerName.Should().Be("account");
            });
        }


        [Test]
        public void ChangePassword_WhenModelIsNotValid_ReturnsChangePasswordViewModel()
        {
            var model = new ChangePasswordViewModel() { CurrentPassword = Faker.Random.String2(6, 20), NewPassword = Faker.Random.String2(6, 20) };
            var userManager = FaketUserManager<User>();
            var signInManager = FakeSignInManager<User>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var deliveryServices = Substitute.For<IEnumerable<IDeliveryService>>();
            var emailService = Substitute.For<AbstractEmailService>();
            var orderService = Substitute.For<AbstractOrderService>();
            AccountController accountController = new AccountController(userManager, signInManager,
                                 httpContextAccessor, deliveryServices, emailService, orderService);
            accountController.ModelState.AddModelError("Test", "TestMessage");
            //Act
            var task = accountController.ChangePassword(model);
            var viewResult = task.Result as ViewResult;
            var viewModel = viewResult.Model as ChangePasswordViewModel;
            //Assert
            viewModel.NewPassword.Should().Be(model.NewPassword);
        }


        private User CreateFakeUser() =>
            new User()
            {
                Name = Faker.Person.FirstName,
                UserName = Faker.Person.Email,
                Email = Faker.Person.Email,
                Surname = Faker.Person.LastName,
                Address = Faker.Person.Address.ToString(),
                City = Faker.Random.String2(5, 20),
                PhoneNumber = Faker.Person.Phone
            };
        

        private RegisterViewModel Map(User user) => 
            new RegisterViewModel()
            {
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                Address = user.Address,
                City = user.City,
                Telephone = user.PhoneNumber
            };

        private LoginViewModel CreateFakeLoginViewModel() =>
            new LoginViewModel()
            {
                Email = Faker.Person.Email,
                Password = Faker.Random.String2(10),
                RememberMe = true
            };

        private ChangeUserViewModel CreateChangeUserViewModel() =>
            new ChangeUserViewModel()
            {
                Name = Faker.Person.FirstName,
                Surname = Faker.Person.LastName,
                Address = Faker.Address.ToString(),
                Email = Faker.Person.Email,
                Telephone = Faker.Person.Phone                
            };

    }
}
