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

namespace InstantTutors.Controllers
{
    [OutputCache(Duration = 3600)]
    public class ErrorController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private IAccountService _accountService;

        [InjectionConstructor]
        public ErrorController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public ErrorController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult BadRequest()
        {
            return RedirectToAction("NotFound");
        }

        public async Task<ActionResult> Homepage()
        {
            if (User.Identity.IsAuthenticated)
            {
                var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
                if (await UserManager.IsInRoleAsync(_user.Id, "Admin"))
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
            }
            return RedirectToAction("Index", "Home", new { area = "" });
        }

    }
}