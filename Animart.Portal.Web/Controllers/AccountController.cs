﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Configuration.Startup;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Threading;
using Abp.UI;
using Abp.Web.Mvc.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Animart.Portal.Authorization;
using Animart.Portal.Extension;
using Animart.Portal.MultiTenancy;
using Animart.Portal.Users;
using Animart.Portal.Web.Models;
using Microsoft.Owin.Security.DataProtection;

namespace Animart.Portal.Web.Controllers
{
    public class AccountController : PortalControllerBase
    {
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IMultiTenancyConfig _multiTenancyConfig;

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public AccountController(
          UserManager userManager,
          RoleManager roleManager,
          IUnitOfWorkManager unitOfWorkManager,
          IMultiTenancyConfig multiTenancyConfig)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWorkManager = unitOfWorkManager;
            _multiTenancyConfig = multiTenancyConfig;
        }

        public ActionResult Login(string returnUrl = "")
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = Request.ApplicationPath;
            }

            return View(
                new LoginFormViewModel
                {
                    ReturnUrl = returnUrl,
                    IsMultiTenancyEnabled = _multiTenancyConfig.IsEnabled
                });
        }

        [HttpPost]
        [DisableAuditing]
        public async Task<JsonResult> Login(LoginViewModel loginModel, string returnUrl = "")
        {
            try
            {
                CheckModelState();

                var loginResult = await GetLoginResultAsync(
                    loginModel.UsernameOrEmailAddress,
                    loginModel.Password,
                    loginModel.TenancyName
                    );

                await SignInAsync(loginResult.User, loginResult.Identity, loginModel.RememberMe);

                if (string.IsNullOrWhiteSpace(returnUrl))
                {
                    returnUrl = Request.ApplicationPath;
                }
                return Json(new MvcAjaxResponse { TargetUrl = returnUrl});
            }
            catch (Exception ex)
            {

                return Json(new {Error = true,Message=ex.Message});
            }
           
        }

        private async Task<AbpUserManager<Tenant, Role, Users.User>.AbpLoginResult> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
        {
            var loginResult = await _userManager.LoginAsync(usernameOrEmailAddress, password, tenancyName);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    return loginResult;
                default:
                    throw CreateExceptionForFailedLoginAttempt(loginResult.Result, usernameOrEmailAddress, tenancyName);
            }
        }

        private async Task SignInAsync(Users.User user, ClaimsIdentity identity = null, bool rememberMe = false)
        {
            if (identity == null)
            {
                identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            }

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = rememberMe }, identity);
        }

        private Exception CreateExceptionForFailedLoginAttempt(AbpLoginResultType result, string usernameOrEmailAddress, string tenancyName)
        {
            switch (result)
            {
                case AbpLoginResultType.Success:
                    return new ApplicationException("Don't call this method with a success result!");
                case AbpLoginResultType.InvalidUserNameOrEmailAddress:
                case AbpLoginResultType.InvalidPassword:
                    return new UserFriendlyException(L("LoginFailed"), L("InvalidUserNameOrPassword"));
                case AbpLoginResultType.InvalidTenancyName:
                    return new UserFriendlyException(L("LoginFailed"), L("ThereIsNoTenantDefinedWithName{0}", tenancyName));
                case AbpLoginResultType.TenantIsNotActive:
                    return new UserFriendlyException(L("LoginFailed"), L("TenantIsNotActive", tenancyName));
                case AbpLoginResultType.UserIsNotActive:
                    return new UserFriendlyException(L("LoginFailed"), L("UserIsNotActiveAndCanNotLogin", usernameOrEmailAddress));
                case AbpLoginResultType.UserEmailIsNotConfirmed:
                    return new UserFriendlyException(L("LoginFailed"), "Your email address is not confirmed. You can not login"); //TODO: localize message
                default: //Can not fall to default actually. But other result types can be added in the future and we may forget to handle it
                    Logger.Warn("Unhandled login fail reason: " + result);
                    return new UserFriendlyException(L("LoginFailed"));
            }
        }

        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login");
        }

        #region Register

        public ActionResult Register()
        {
            return RegisterView(new RegisterViewModel());
        }

        private ActionResult RegisterView(RegisterViewModel model)
        {
            ViewBag.IsMultiTenancyEnabled = _multiTenancyConfig.IsEnabled;

            return View("Register", model);
        }

        [HttpPost]
        [UnitOfWork]
        public virtual async Task<ActionResult> Register(RegisterViewModel model)
        {
            try
            {
                CheckModelState();

                //Get tenancy name and tenant
                //if (!_multiTenancyConfig.IsEnabled)
                //{
                //    model.TenancyName = Tenant.DefaultTenantName;
                //}
                //else if (model.TenancyName.IsNullOrEmpty())
                //{
                //    throw new UserFriendlyException(L("TenantNameCanNotBeEmpty"));
                //}

             //   var tenant = await GetActiveTenantAsync(model.TenancyName);

                //Create user
                var user = new Users.User
                {
                    TenantId = 1,
                    Name = model.Name,
                    Surname = model.Surname,
                    EmailAddress = model.EmailAddress,
                    IsActive = true
                };

                //Get external login info if possible
                ExternalLoginInfo externalLoginInfo = null;
                if (model.IsExternalLogin)
                {
                    externalLoginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
                    if (externalLoginInfo == null)
                    {
                        throw new ApplicationException("Can not external login!");
                    }

                    user.Logins = new List<UserLogin>
                    {
                        new UserLogin
                        {
                            LoginProvider = externalLoginInfo.Login.LoginProvider,
                            ProviderKey = externalLoginInfo.Login.ProviderKey
                        }
                    };

                    if (string.IsNullOrWhiteSpace(model.UserName))
                    {
                        model.UserName = model.EmailAddress;
                    }

                    if (string.IsNullOrWhiteSpace(model.Password))
                    {
                        model.Password = Users.User.CreateRandomPassword();
                    }

                    if (string.Equals(externalLoginInfo.Email, model.EmailAddress, StringComparison.InvariantCultureIgnoreCase))
                    {
                        user.IsEmailConfirmed = true;
                    }
                }
                else
                {
                    //Username and Password are required if not external login
                    if (model.UserName.IsNullOrEmpty() || model.Password.IsNullOrEmpty())
                    {
                        throw new UserFriendlyException("Please insert your email or username, and password to login."); //L("FormIsNotValidMessage"));
                    }
                }

                user.UserName = model.UserName;
                user.Password = new PasswordHasher().HashPassword(model.Password);

                //Switch to the tenant
                _unitOfWorkManager.Current.EnableFilter(AbpDataFilters.MayHaveTenant);
                _unitOfWorkManager.Current.SetFilterParameter(AbpDataFilters.MayHaveTenant, AbpDataFilters.Parameters.TenantId, 1);

                //Add default roles
                user.Roles = new List<UserRole>();
                var role = await _roleManager.GetRoleByNameAsync("Retailer");
                role.IsDefault = true;
                _roleManager.Update(role);
                foreach (var defaultRole in await _roleManager.Roles.Where(r => r.IsDefault).ToListAsync())
                {
                    user.Roles.Add(new UserRole { RoleId = defaultRole.Id });
                }

                //Save user
                CheckErrors(await _userManager.CreateAsync(user));
                await _unitOfWorkManager.Current.SaveChangesAsync();

                //Directly login if possible
                if (user.IsActive)
                {
                    AbpUserManager<Tenant, Role, Users.User>.AbpLoginResult loginResult;
                    if (externalLoginInfo != null)
                    {
                        loginResult = await _userManager.LoginAsync(externalLoginInfo.Login, "");
                    }
                    else
                    {
                        loginResult = await GetLoginResultAsync(user.UserName, model.Password, "");
                    }

                    if (loginResult.Result == AbpLoginResultType.Success)
                    {
                        await SignInAsync(loginResult.User, loginResult.Identity);
                        return Redirect(Url.Action("Index", "Home"));
                    }

                    Logger.Warn("New registered user could not be login. This should not be normally. login result: " + loginResult.Result);
                }

                //If can not login, show a register result page
                return View("RegisterResult", new RegisterResultViewModel
                {
                    TenancyName = "Animart",
                    NameAndSurname = user.Name + " " + user.Surname,
                    UserName = user.UserName,
                    EmailAddress = user.EmailAddress,
                    IsActive = user.IsActive,
                    IsEmailConfirmationRequired = false
                });
            }
            catch (UserFriendlyException ex)
            {
                ViewBag.IsMultiTenancyEnabled = _multiTenancyConfig.IsEnabled;
                ViewBag.ErrorMessage = ex.Message;

                return View("Register", model);
            }
        }

        #endregion


        #region ForgotPassword

        public ActionResult ForgotPassword()
        {
            return ForgotPasswordView(new ForgotPasswordViewModel());
        }

        private ActionResult ForgotPasswordView(ForgotPasswordViewModel model)
        {
            ViewBag.IsMultiTenancyEnabled = _multiTenancyConfig.IsEnabled;

            return View("ForgotPassword", model);
        }


        public ActionResult ResetPassword(long userId, string code)
        {
            if (userId > 0 && !code.IsNullOrEmpty())
            {
                var user = _userManager.FindById(userId);
                return View("ResetPassword", new ResetPasswordViewModel
                {
                    ResetCode = code,
                    UserId = userId,
                    EmailAddress = user.EmailAddress
                });
            }
            else
                return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [UnitOfWork]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            try {
                CheckModelState();
                bool isSuccess = false;
                var user = await _userManager.FindByEmailAsync(model.EmailAddress);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    return RedirectToAction("Login", "Account");
                }

                // var result = await _userManager.ResetPasswordAsync(user.Id, model.ResetCode, model.Password);
                if (model.ResetCode == user.PasswordResetCode && model.Password == model.ConfirmPassword)
                {
                    user.PasswordResetCode = null;
                    user.Password = new PasswordHasher().HashPassword(model.Password);

                    //_unitOfWorkManager.Current.EnableFilter(AbpDataFilters.MayHaveTenant);
                    //_unitOfWorkManager.Current.SetFilterParameter(AbpDataFilters.MayHaveTenant, AbpDataFilters.Parameters.TenantId, 1);

                    //Save user
                    CheckErrors(await _userManager.UpdateAsync(user));
                    //await _unitOfWorkManager.Current.SaveChangesAsync();
                    isSuccess = true;
                }
                else
                {
                    throw new UserFriendlyException("Your reset code is wrong.");
                }

                return View("ResetPasswordResult", new ResetPasswordResultViewModel
                {
                    EmailAddress = user.EmailAddress,
                    IsSuccess = isSuccess
                });
            }
            catch (UserFriendlyException ex)
            {
                ViewBag.IsMultiTenancyEnabled = _multiTenancyConfig.IsEnabled;
                ViewBag.ErrorMessage = ex.Message;

                return View("ForgotPassword", new ForgotPasswordViewModel
                {
                    EmailAddress = model.EmailAddress
                });
            }

        }

        [HttpPost]
        [UnitOfWork]
        public virtual async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                CheckModelState();
                
                bool isActive = true;

                if (model.EmailAddress.IsNullOrEmpty())
                {
                    throw new UserFriendlyException("Please insert your email."); //L("FormIsNotValidMessage"));
                }
                //var provider = new DpapiDataProtectionProvider("AnimartRetailer");
                //_userManager.UserTokenProvider = new DataProtectorTokenProvider<Users.User, long>(provider.Create("UserToken"));
                
                var _user = _userManager.FindByEmail(model.EmailAddress);
                if (_user != null)
                {
                    var ResetCode = Guid.NewGuid().ToString();
                    ResetCode = new PasswordHasher().HashPassword(ResetCode);
                    //await _userManager.GeneratePasswordResetTokenAsync(_user.Id);
                    var TrimmedCode = ResetCode.Substring(0,
                        Math.Min(Users.User.MaxPasswordResetCodeLength, ResetCode.Length));

                    _user.PasswordResetCode = TrimmedCode;

                    //Switch to the tenant
                    _unitOfWorkManager.Current.EnableFilter(AbpDataFilters.MayHaveTenant);
                    _unitOfWorkManager.Current.SetFilterParameter(AbpDataFilters.MayHaveTenant,
                        AbpDataFilters.Parameters.TenantId, 1);

                    //Save user
                    CheckErrors(await _userManager.UpdateAsync(_user));
                    await _unitOfWorkManager.Current.SaveChangesAsync();


                    var callbackUrl = Url.Action("ResetPassword", "Account",
                        new {userId = _user.Id, code = _user.PasswordResetCode}, protocol: Request.Url.Scheme);

                    GmailExtension gmail = new GmailExtension();
                    gmail.SendMessage("Animart Portal Reset Password",
                        "Please reset your password by clicking <a href=\"" + callbackUrl +
                        "\">here</a> <br/> If this is not you, please inform us by using email to marketing@animart.co.id.",
                        _user.EmailAddress,false,null,null);
                    isActive = _user.IsActive;
                }

                return View("ForgotPasswordResult", new ForgotPasswordResultViewModel
                {
                    EmailAddress = model.EmailAddress,
                    IsActive = isActive,
                    IsEmailConfirmationRequired = true
                });

            }
            catch (UserFriendlyException ex)
            {
                ViewBag.IsMultiTenancyEnabled = _multiTenancyConfig.IsEnabled;
                ViewBag.ErrorMessage = ex.Message;

                return View("ForgotPassword", model);
            }
        }

        #endregion

    }
}