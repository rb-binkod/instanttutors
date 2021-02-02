using InstantTutors.Helpers;
using InstantTutors.Models.ViewModels;
using InstantTutors.Services;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Configuration;
using System.Net.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Unity;

namespace InstantTutors.Controllers
{
    [OutputCache(Duration = 3600)]
    public class HomeController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private IAccountService _accountService;

        [InjectionConstructor]
        public HomeController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        public async Task<ActionResult> Index()
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
                else
                    return View();
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Contact(ContactUsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                model.Mobile = string.IsNullOrEmpty(model.Mobile) ? " - " : model.Mobile;
                var _subject = "Contact mail from " + model.FirstName + " " + model.LastName + " | instanttutors.org";
                var _body = "Mail from " + model.FirstName + " " + model.LastName + ",<br/><br/>"
                            + "<b>Email Address:</b> " + model.Email + "<br/>"
                            + "<b>Mobile:</b> " + model.Mobile + "<br/>"
                            + "<b>Comment:</b> " + model.Comment + "<br/><br/>"
                            + "<a href='http://instanttutors.org/' target='_blank'>Instant Tutors</a> @ " + DateTime.Now.Year;

                await EmailSender.SendEmailAsync(_subject, _body);
                ModelState.Clear();
                ViewBag.success = "<ul><li><p style='color:green'>Email has been sent. Thanks for contacting us.</p></li></ul>";
            }
            catch (Exception ex)
            {
                ViewBag.success = "<ul><li><p style='color:red'>ERROR: " + ex.Message + "</p><input type='hidden' value='" + ex + "'/></li></ul>";
            }
            return View();
        }

    }
}