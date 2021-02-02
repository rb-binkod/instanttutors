using InstantTutors.Areas.Tutor.ViewModels;
using InstantTutors.Helpers;
using InstantTutors.Models;
using InstantTutors.Models.Enums;
using InstantTutors.Services.Tutor;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Unity;

namespace InstantTutors.Areas.Tutor.Controllers
{
    //[RedirectLoginFilter]
    [Authorize(Roles = "Tutor")]
    //[OutputCache(Duration = 360)]
    public class SessionController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ITuitionService _tuitionService;
        private ITutorService _tutorService;

        [InjectionConstructor]
        public SessionController(ITuitionService tuitionService, ITutorService tutorService)
        {
            _tuitionService = tuitionService;
            _tutorService = tutorService;
        }

        public SessionController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Tutor/Session
        public async Task<ActionResult> Index()
        {
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            TutorViewModel model = await _tutorService.GetTutorAvailabilityAsync(_user);

            var _sessionRequests = await _tuitionService.GetSessionByTutorUserIdAsync(_user.Id);
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

        public async Task<ActionResult> Availability()
        {
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            TutorViewModel model = await _tutorService.GetTutorAvailabilityAsync(_user);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Availability(TutorViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _tuitionService.SetAvailabilityAsync(model);
                    ModelState.Clear();
                    ViewBag.success = "<ul><li><p style='color:green'>Successfully Submitted!! Thanks for providing information.</p></li></ul>";

                    var t = Task.Run(() =>
                    {
                        try
                        {
                            var _subject = "Tuition Availability - " + model.User.FirstName + " " + model.User.LastName + " | instanttutors.org";
                            var _body = "<b>Tutor Availability Updation</b><br><br/> "
                                + "Tutor : <b>" + model.User.FirstName + " " + model.User.LastName + "</b><br/>"
                                + "Tutor has updated Tuition Availability Information. <br/>"
                                + "Comment/Concerns: " + model.Concerns + "<br/><br/>"
                                + "<a href='http://instanttutors.org/' target='_blank'>Instant Tutors</a> @ " + DateTime.Now.Year;

                            EmailSender.SendEmail(_subject, _body);
                            SMSSender.SMSSenderAsync("Tutor " + model.User.FirstName + " " + model.User.LastName + " just updated tuition availability time.");
                        }
                        catch { }
                    });

                    return View(model);
                }
                catch (Exception ex)
                {
                    ViewBag.success = "<ul><li><p style='color:red'>ERROR: " + ex.Message + "</p></li></ul>";
                    return View(model);
                }
            }
            else
            {
                var error = ModelState.FirstOrDefault(x => x.Value.Errors.Count > 0).Value.Errors.First().ErrorMessage;
                ViewBag.success = "<ul><li><p style='color:red'>ERROR: " + error + "</p></li></ul>";
            }
            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateSessionStatus(int SessionId, string UserId, SessionStatus status)
        {
            if(string.IsNullOrEmpty(UserId) || UserId == "0")
                UserId = User.Identity.GetUserId();

            var result =  await _tutorService.ApproveDeclineSession(SessionId, new Sessions()
            {
                Status = status,
                ApproveDeclineBy = UserId
            });

            if (result.Item1)
            {
                try
                {
                    var _session = result.Item2;
                    var _status = _session.Status == SessionStatus.Approved ? "Aprroved" : "Declined";
                    var _subject = status + " Session Request | instanttutors.org";
                    var _body = "<h3>Session request has been " + _status + ".</h3>"
                         + "<b>Session Title:</b> " + _session.Title + "<br/>"
                        + "<b>Description <small>(If Any)</small>:</b> " + _session.Description + "<br/>"
                        + "<b>Communication Method:</b> " + _session.CommunicationMethod + "<br/><br/>"
                        + "<a href='http://instanttutors.org/' target='_blank'>Instant Tutors</a> Team @" + DateTime.Now.Year;

                    await EmailSender.SendEmailAsync(_subject, _body);
                    await SMSSender.SMSSenderAsync(_session.Title + " Session request has been " + _status + ".");
                }
                catch (Exception ex1)
                {
                }
            }
            return Json(result.Item1, JsonRequestBehavior.AllowGet);
        }
    }
}
