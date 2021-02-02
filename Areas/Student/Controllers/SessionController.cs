using InstantTutors.Areas.Student.ViewModels;
using InstantTutors.Helpers;
using InstantTutors.Models;
using InstantTutors.Services.Student;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Unity;

namespace InstantTutors.Areas.Student.Controllers
{
    [Authorize(Roles = "Student")]
    //[OutputCache(Duration = 360)]
    public class SessionController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ISessionService _sessionService;
        private IScheduleService _scheduleService;

        [InjectionConstructor]
        public SessionController(ISessionService sessionService, IScheduleService scheduleService)
        {
            _sessionService = sessionService;
            _scheduleService = scheduleService;
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

        // GET: Student/Session
        public async Task<ActionResult> Index()
        {
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            StudentViewModel model = new StudentViewModel()
            {
                UserId = _user.Id,
                User = _user,
                SessionsVMList = new List<StudentSessions>()
            };
            model.SessionsList = await _sessionService.GetSessionByUserIdAsync(_user.Id);
            model.SessionsList.ForEach(x =>
            {
                var _tutorUser = UserManager.FindByIdAsync(x.TutorUserId).Result;
                model.SessionsVMList.Add(new StudentSessions()
                {
                    Session = x,
                    TutorUserId = x.TutorUserId,
                    TutorUser = _tutorUser
                });
            });
            return View(model);
        }
        public async Task<ActionResult> NewSchedule(string id)
        {
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            var _session = new Sessions();

            var _tutorsList = await _sessionService.GetTutorsAsync();
            if (!string.IsNullOrEmpty(id))
            {
                var selectedTutor = _tutorsList.Where(x => x.Value == id).FirstOrDefault();
                selectedTutor.Selected = true;
            }
            ViewBag.TutorsList = _tutorsList;

            SessionViewModel model = new SessionViewModel()
            {
                Id = _session.Id,
                Title = _session.Title,
                Description = _session.Description,
                Concerns = _session.Concerns,
                CommunicationMethod = _session.CommunicationMethod,
                CreatedDate = _session.CreatedDate == null ? DateTime.Now : _session.CreatedDate,
                UserId = _user.Id,
                User = _user,
                TutorUserId = id,
                TutorsList = _tutorsList
            };

            var SessionSchedules = new List<SessionScheduleViewModel>();
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)).OfType<DayOfWeek>().ToList().Skip(1))
            {
                SessionSchedules.Add(new SessionScheduleViewModel()
                {
                    //Id
                    UserId = _user.Id,
                    SessionId = _session.Id,
                    Timing = Utils.Common.GetTiming(),
                    Day = day
                });
            }
            if (_session.SessionSchedule != null)
            {
                if (_session.SessionSchedule.Count > 0)
                {
                    foreach (var schedule in SessionSchedules)
                    {
                        foreach (var _time in schedule.Timing)
                        {
                            var availTime = _session.SessionSchedule.FirstOrDefault(x => x.Time == _time.AvailabilityTime && x.Day == schedule.Day);
                            if (availTime != null)
                            {
                                schedule.Id = availTime.Id;
                                _time.Selected = true;
                                schedule.Time = availTime.Time;
                            }
                        }
                    }
                }
            }
            model.SessionSchedules = SessionSchedules;
            return View(model);
        }
        public async Task<ActionResult> Schedule(int? id)
        {
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            var _session = await _sessionService.GetSessionByIdAsync(id ?? 0);
            _session = _session == null ? new Sessions() : _session;

            var _tutorsList = await _sessionService.GetTutorsAsync();
            if (!string.IsNullOrEmpty(_session.TutorUserId))
            {
                var selectedTutor = _tutorsList.Where(x => x.Value == _session.TutorUserId).FirstOrDefault();
                selectedTutor.Selected = true;
            }
            ViewBag.TutorsList = _tutorsList;

            SessionViewModel model = new SessionViewModel()
            {
                Id = _session.Id,
                Title = _session.Title,
                Description = _session.Description,
                Concerns = _session.Concerns,
                CommunicationMethod = _session.CommunicationMethod,
                CreatedDate = _session.CreatedDate == null ? DateTime.Now : _session.CreatedDate,
                UserId = _user.Id,
                User = _user,
                TutorUserId = _session.TutorUserId,
                TutorsList = _tutorsList
            };

            var SessionSchedules = new List<SessionScheduleViewModel>();
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)).OfType<DayOfWeek>().ToList().Skip(1))
            {
                SessionSchedules.Add(new SessionScheduleViewModel()
                {
                    //Id
                    UserId = _user.Id,
                    SessionId = _session.Id,
                    Timing = Utils.Common.GetTiming(),
                    Day = day
                });
            }
            if (_session.SessionSchedule != null)
            {
                if (_session.SessionSchedule.Count > 0)
                {
                    foreach (var schedule in SessionSchedules)
                    {
                        foreach (var _time in schedule.Timing)
                        {
                            var availTime = _session.SessionSchedule.FirstOrDefault(x => x.Time == _time.AvailabilityTime && x.Day == schedule.Day);
                            if (availTime != null)
                            {
                                schedule.Id = availTime.Id;
                                _time.Selected = true;
                                schedule.Time = availTime.Time;
                            }
                        }
                    }
                }
            }
            model.SessionSchedules = SessionSchedules;
            return View(model);
        }

        [HttpPost] //Student/Session/Schedule
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Schedule(SessionViewModel model)
        {
            model.TutorsList = model.TutorsList == null ? await _sessionService.GetTutorsAsync() : model.TutorsList;
            if (ModelState.IsValid)
            {
                try
                {
                    var sessionInfo = string.Empty;
                    int DayTimeChecked = 0;
                    if (model.SessionSchedules != null)
                    {
                        foreach (var schedule in model.SessionSchedules)
                        {
                            foreach (var time in schedule.Timing)
                            {
                                if (time.Selected)
                                {
                                    DayTimeChecked += 1;
                                    sessionInfo += "<b>" + schedule.Day.ToString() + "</b>&nbsp;&nbsp;&nbsp;<b>" + time.Time + "</b>&nbsp;&nbsp;&nbsp;<b>" + model.CommunicationMethod + "</b>&nbsp;&nbsp;&nbsp;<b>(" + model.Title + ")</b><br/><br/>";
                                }
                            }
                        }
                    }
                    if (DayTimeChecked < 2)
                    {
                        ViewBag.success = "<ul><li><p style='color:red'>Validation ERROR: Please choose atleast 2+ Days/Times that you are available.</p></li></ul>";
                        return View(model);
                    }

                    var message = "New Session has been scheduled.";
                    try { model.CreatedBy = User.Identity.GetUserId(); } catch { }
                    if (model.Id > 0)
                    {
                        (bool success, SessionViewModel data) = await _scheduleService.UpdateScheduleAsync(model);
                        if (success == false)
                        {
                            ViewBag.success = "<ul><li><p style='color:red'>ERROR: Session Doesn't Exist.</p></li></ul>";
                            return View(model);
                        }
                        message = "A scheduled session has been updated.";
                    }
                    else
                    {
                        (bool success, SessionViewModel data) = await _scheduleService.CreateScheduleAsync(model);
                    }

                    //var t = Task.Run(() =>
                    //{
                    try
                    {
                        var tutorInfo = await UserManager.FindByIdAsync(model.TutorUserId);
                        var _subject = "New Session Scheduled - " + model.User.FirstName + " " + model.User.LastName + " | instanttutors.org";
                        var _body = "<h3>" + message + "</h3>"
                            + "<b>Student Name:</b> " + model.User.FirstName + " " + model.User.LastName + "<br/>"
                            + "<b>Email:</b> " + model.User.Email + "<br/>"
                            + "<b>Mobile:</b> " + model.User.PhoneNumber + "<br/>"
                            + "<b>Session Title:</b> " + model.Title + "<br/>"
                            + "<b>Description <small>(If Any)</small>:</b> " + model.Description + "<br/>"
                            + "<b>Tutor Information:</b> " + tutorInfo.FirstName + " " + tutorInfo.LastName + " <small>(" + tutorInfo.Gender + ")</small>, E: " + tutorInfo.Email + ", M: " + tutorInfo.PhoneNumber + "<br/>"
                            + "<b>Communication Method:</b> " + model.CommunicationMethod + "<br/>"
                            + "<b>Comment/Concerns <small>(If Any)</small>:</b> " + model.Concerns
                            + "<h4>AVAILABLE ON : </h4>" + sessionInfo + "<br/><br/>"
                            + "<a href='http://instanttutors.org/' target='_blank'>Instant Tutors</a> Team @" + DateTime.Now.Year;

                        await EmailSender.SendEmailAsync(_subject, _body);
                        await SMSSender.SMSSenderAsync(message + " Student name is " + model.User.FirstName + " " + model.User.LastName);
                    }
                    catch (Exception ex1)
                    {
                    }
                    //});

                    ModelState.Clear();
                    ViewBag.success = "<ul><li><p style='color:green'>Hurray!! Session info successfully submitted.</p></li></ul>";
                    return View(model);
                }
                catch (Exception ex)
                {
                    ViewBag.success = "<ul><li><p style='color:red'>ERROR: " + ex.Message + "</p></li></ul>";
                    return View(model);
                }
            }
            return View(model);
        }
    }
}
