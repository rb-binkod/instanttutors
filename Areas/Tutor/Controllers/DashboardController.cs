using InstantTutors.Areas.Tutor.ViewModels;
using InstantTutors.Helpers;
using InstantTutors.Models;
using InstantTutors.Models.ViewModels;
using InstantTutors.Services.Tutor;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Unity;

namespace InstantTutors.Areas.Tutor.Controllers
{
    //[RedirectLoginFilter]
    [Authorize(Roles = "Tutor")]
    //[OutputCache(Duration = 360)]
    public class DashboardController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ITuitionService _tuitionService;
        private ITutorService _tutorService;

        [InjectionConstructor]
        public DashboardController(ITuitionService tuitionService, ITutorService tutorService)
        {
            _tuitionService = tuitionService;
            _tutorService = tutorService;
        }

        public DashboardController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Tutor/Dashboard
        public async Task<ActionResult> Index()
        {
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            TutorViewModel model = await _tutorService.GetTutorAvailabilityAsync(_user);
            
            var _sessionRequests = await _tuitionService.GetSessionByTutorUserIdAsync(_user.Id, 5);
            _sessionRequests.ForEach(x =>
            {
                model.SessionRequests.Add(new SessionRequestsViewModel()
                {
                    Session = x,
                    TutorUserId = x.TutorUserId,
                    StudentUserId = x.UserId,
                    StudentUser = x.User
                });
            });

            return View(model);
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
