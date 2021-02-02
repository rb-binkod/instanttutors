using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using InstantTutors.Models;
using InstantTutors.Models.ViewModels;
using InstantTutors.Services;
using Unity;
using InstantTutors.Helpers;
using System.IO;
using InstantTutors.Areas.Tutor.ViewModels;
using System.Collections.Generic;

namespace InstantTutors.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private IAccountService _accountService;

        [InjectionConstructor]
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.VerificationLink = false;
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    var _user = await UserManager.FindByEmailAsync(model.Email);
                    if (!_user.EmailConfirmed) {
                        ModelState.AddModelError("", "Email address is not verified yet. Please check your inbox for verification link.");
                        ViewBag.VerificationLink = true;
                        return View(model);
                    }
                    else if (await UserManager.IsInRoleAsync(_user.Id, "Admin"))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }
                    else if (await UserManager.IsInRoleAsync(_user.Id, "Tutor"))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Tutor" });
                    }
                    else if (await UserManager.IsInRoleAsync(_user.Id, "Student"))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Student" });
                    }
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }


        #region
        
        [AllowAnonymous]
        public ActionResult Tutor()
        {
            TutorRegisterViewModel model = new TutorRegisterViewModel();
            model.TuitionSubjects = new List<TuitionSubjectsViewModel>()
            {
                new TuitionSubjectsViewModel {Name="K-5th", Description="K-5th", Selected = false, TutorId = 0 },
                new TuitionSubjectsViewModel {Name="6th-8th", Description="6th-8th", Selected = false, TutorId = 0 },
                new TuitionSubjectsViewModel {Name="FSA/EOC", Description="FSA/EOC", Selected = false, TutorId = 0 },
                new TuitionSubjectsViewModel {Name="World Languages (Spanish)", Description="World Languages (Spanish)", Selected = false, TutorId = 0 }
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Tutor(TutorRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var _existUser = await UserManager.FindByEmailAsync(model.Email);
                if (_existUser != null)
                {
                    await UserManager.UpdateAsync(_existUser);
                    ViewBag.Message = "Email Already Exists.";
                    ModelState.AddModelError("", "Email Already Exists.");
                    return View(model);
                }

                //profile upload
                if (model.File != null)
                {
                    //var supportedTypes = new[] { "jpg", "jpeg", "png", "gif" };
                    //var fileExt = System.IO.Path.GetExtension(model.File.FileName).Substring(1);

                    try
                    {
                        var supportedTypes = new[] { "jpg", "jpeg", "png", "gif", "tif" };
                        var fileExt = System.IO.Path.GetExtension(model.File.FileName).Substring(1);
                        if (supportedTypes.Contains(fileExt.ToLower()))
                        {
                            var profileImage = Guid.NewGuid() + Path.GetExtension(model.File.FileName);
                            model.File.SaveAs(HttpContext.Server.MapPath("~/Content/images/tutor/") + profileImage);
                            model.ProfileImage = profileImage;
                        }
                    }
                    catch (Exception exi)
                    {

                    }
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Title = model.Title,
                    Gender = model.Gender,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.Mobile, 
                    Address = model.Address == null ? model.City + " " + model.Country + " " + model.Zip : model.Address,
                    City = model.City,
                    TimeZone = model.TimeZone,
                    Country = model.Country,
                    Zip = model.Zip,
                    Bio = model.Bio, //summary
                    Hobbies = model.Hobbies,
                    ProfileImage = model.ProfileImage,
                    //DOB = DateTime.Now.AddYears(-29),
                    CreatedDate = DateTime.Now,
                    //UpdatedDate = DateTime.Now
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "Tutor");
                    var __tutor = await this._accountService.CreateTutorsAsync(new Tutors
                    {
                        UserId = user.Id,
                        Experience = model.Experience,
                        GradeLevel = model.GradeLevel,
                        NameOfSchool = model.GradeLevel,
                        //Concerns
                        PreviousSubjects = model.GradeLevel
                    });
                    try
                    {
                        if (model.TuitionSubjects != null && __tutor.Id > 0)
                            await this._accountService.AddTutorSubjectAsync(model.TuitionSubjects, __tutor.Id);
                    }
                    catch (Exception ex)
                    {

                    }
                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // Send an email with this link
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                    await UserManager.SendEmailAsync(user.Id, "Confirm your account",
                        "Hi " + model.FirstName + ",<br/><br/>"
                        + "Thanks for registering with us.<br/>"
                        + "Please confirm your tutor account by clicking <a href=\"" + callbackUrl + "\">" + callbackUrl + "</a><br/><br/>"
                        + "Regards, <br/><a href='http://instanttutors.org/' target='_blank'>Instant Tutors Team</a>");

                    //await EmailSender.SendEmailAsync("Confirm your account", "Hi " + model.FirstName + ",<br/><br/>"
                    //    + "Thanks for registering with us.<br/>"
                    //    + "Please confirm your student account by clicking <a href=\"" + callbackUrl + "\">" + callbackUrl + "</a><br/><br/>"
                    //    + "Regards, <br/><a href='http://instanttutors.org/' target='_blank'>Instant Tutors @" + DateTime.Now.Year + "</a>", user.Email, true);

                    try
                    {
                        var _subject = "New Tutor Signup | instanttutors.org";
                        var _body = "<h5>New Tutor Signup</h5>" + ",<br/><br/>"
                            + "<b>Fullname:</b> " + model.FirstName + " " + model.LastName + "<br/>"
                            + "<b>Email Address:</b> " + model.Email + "<br/>"
                            + "<b>Mobile:</b> " + model.Mobile + "<br/><br/>"
                            + "<a href='http://instanttutors.org/' target='_blank'>Instant Tutors</a> @ " + DateTime.Now.Year;

                        await EmailSender.SendEmailAsync(_subject, _body);

                        await SMSSender.SMSSenderAsync("Thanks! Please verify your tutor a/c " + callbackUrl + " <br>Instant Tutors Team", user.PhoneNumber);
                        await SMSSender.SMSSenderAsync("New Tutor Registration - Name: " + user.FirstName + " " + user.LastName + ", M: " + user.PhoneNumber + ", Email: " + user.Email);
                    }
                    catch { }

                    ModelState.Clear();
                    ViewBag.success= "<ul><li><p style='color:green'>Successfully registered!! An email has been sent to your email address, Please verify.</p></li></ul>";
                    return View(model);
                }
                AddErrors(result);
            }
            //var error = ModelState.FirstOrDefault(x => x.Value.Errors.Count > 0).Value.Errors.First().ErrorMessage;

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Student()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Student(StudentRegisterViewModel model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var _existUser = await UserManager.FindByEmailAsync(model.Email);
                if (_existUser != null)
                {
                    ViewBag.Message = "Email Already Exists.";
                    ModelState.AddModelError("", "Email Already Exists.");
                    return View(model);
                }
                //profile upload
                //if (file != null)
                //{
                //    var supportedTypes = new[] { "jpg", "jpeg", "png", "gif" };
                //    var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                //    //if ((file.ContentLength / 1024 > 1024) || (!supportedTypes.Contains(fileExt)))
                //    //{
                //    //    ViewBag.Message = "Only JPEG/PNG image smaller than 1MB is allowed.";
                //    //    ModelState.AddModelError("", "Only JPEG/PNG image smaller than 1MB is allowed.");
                //    //    return View(model);
                //    //}
                //    if (supportedTypes.Contains(fileExt))
                //    {
                //        var profileImage = Guid.NewGuid() + Path.GetExtension(file.FileName);
                //        file.SaveAs(HttpContext.Server.MapPath("~/Content/images/student/") + profileImage);
                //        model.ProfileImage = profileImage;
                //    }
                //}

                //profile upload
                if (model.File != null)
                {
                    try
                    {
                        var supportedTypes = new[] { "jpg", "jpeg", "png", "gif", "tif" };
                        var fileExt = System.IO.Path.GetExtension(model.File.FileName).Substring(1);
                        if (supportedTypes.Contains(fileExt.ToLower()))
                        {
                            var profileImage = Guid.NewGuid() + Path.GetExtension(model.File.FileName);
                            model.File.SaveAs(HttpContext.Server.MapPath("~/Content/images/student/") + profileImage);
                            model.ProfileImage = profileImage;
                        }
                    }
                    catch (Exception exi)
                    {

                    }
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Gender = model.Gender,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.Mobile,
                    Address = model.Address,
                    City = model.City,
                    TimeZone = model.TimeZone,
                    Country = model.Country,
                    Zip = model.Zip,
                    Bio = model.Bio,
                    StudentSchool = model.StudentSchool,
                    StudentGrade = model.StudentGrade,
                    Hobbies = model.Hobbies,
                    ProfileImage = model.ProfileImage,
                    //DOB = DateTime.Now.AddYears(-29),
                    CreatedDate = DateTime.Now,
                    //UpdatedDate = DateTime.Now
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "Student");
                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // Send an email with this link
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                    await UserManager.SendEmailAsync(user.Id, "Confirm your account",
                        "Hi " + model.FirstName + ",<br/><br/>"
                        + "Thanks for registering with us.<br/>"
                        + "Please confirm your student account by clicking <a href=\"" + callbackUrl + "\">" + callbackUrl + "</a><br/><br/>"
                        + "Regards, <br/><a href='http://instanttutors.org/' target='_blank'>Instant Tutors Team</a>");

                    //await EmailSender.SendEmailAsync("Confirm your account", "Hi " + model.FirstName + ",<br/><br/>"
                    //    + "Thanks for registering with us.<br/>"
                    //    + "Please confirm your student account by clicking <a href=\"" + callbackUrl + "\">" + callbackUrl + "</a><br/><br/>"
                    //    + "Regards, <br/><a href='http://instanttutors.org/' target='_blank'>Instant Tutors @" + DateTime.Now.Year + "</a>", user.Email, true);

                    try
                    {
                        var _subject = "New Student Signup | instanttutors.org";
                        var _body = "<h5>New Student Signup</h5>" + ",<br/><br/>"
                            + "<b>Fullname:</b> " + model.FirstName + " " + model.LastName + "<br/>"
                            + "<b>Email Address:</b> " + model.Email + "<br/>"
                            + "<b>Mobile:</b> " + model.Mobile + "<br/><br/>"
                            + "<a href='http://instanttutors.org/' target='_blank'>Instant Tutors</a> @ " + DateTime.Now.Year;

                        await EmailSender.SendEmailAsync(_subject, _body);

                        await SMSSender.SMSSenderAsync("Thanks! Please verify your student a/c " + callbackUrl + " <br>Instant Tutors Team", user.PhoneNumber);
                        await SMSSender.SMSSenderAsync("New Student Registration - Name: " + user.FirstName + " " + user.LastName + ", M: " + user.PhoneNumber + ", Email: " + user.Email);
                    }
                    catch { }

                    ModelState.Clear();
                    ViewBag.success = "<ul><li><p style='color:green'>Successfully registered!! An email has been sent to your email address, Please verify.</p></li></ul>";
                    return View(model);
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion



        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        //[AllowAnonymous]
        [Authorize(Roles = "Student")]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Obsolete]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    
                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Resend(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                ViewBag.resend = "<ul><li><p style='color:red'>Email doesn't exists.</p></li></ul>";
            }
            else
            {
                var user = await UserManager.FindByEmailAsync(id);
                if (user == null)
                {
                    ViewBag.resend = "<ul><li><p style='color:red'>Email doesn't exists.</p></li></ul>";
                    return RedirectToAction("Login");
                }

                // Send an email with this link
                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                await UserManager.SendEmailAsync(user.Id, "Confirm your account| Resend verification link",
                    "Hi " + user.FirstName + " " + user.LastName + ",<br/><br/>"
                    + "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">" + callbackUrl + "</a><br/><br/>"
                    + "Regards, <br/><a href='http://instanttutors.org/' target='_blank'>Instant Tutors Team</a>");
                
                ViewBag.resend = "<ul><li><p style='color:green'>Successfully resend a verification link, please check your inbox & verify.</p></li></ul>";
                return RedirectToAction("Login");
            }
            return RedirectToAction("Login");
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null)// || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                //await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">" + callbackUrl + "</a>");

                await EmailSender.SendEmailAsync("Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">" + callbackUrl + "</a><br/><br/>Regards, <br/><a href='http://instanttutors.org/' target='_blank'>Instant Tutors Team @" + DateTime.Now.Year + "</a>", user.Email, true);

                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Session.Clear();
            Session.Abandon();
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}