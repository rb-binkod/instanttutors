using InstantTutors.Models.ViewModels;
using InstantTutors.Services.Student;
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
    [Authorize(Roles = "Tutor")]
    public class SearchController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private IStudentService _studentService;
        [InjectionConstructor]
        public SearchController(IStudentService studentService)
        {
            _studentService = studentService;
        }
        public SearchController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Student/Search
        //[OutputCache(Duration = 3600)]
        public async Task<ActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(SearchViewModel model, string Reset, string SeeAll)
        {
            if (!string.IsNullOrEmpty(Reset))
            {
                ModelState.Clear();
                model = new SearchViewModel();
                return View(model);
            }
            else if (!string.IsNullOrEmpty(SeeAll))
            {
                model.SearchText = "IT";
                ModelState.Clear();
                model.SearchStudentsList = await _studentService.SearchStudentsAsync(model, true);
            }
            else if (ModelState.IsValid)
            {
                model.SearchStudentsList = await _studentService.SearchStudentsAsync(model);
            }
            model.SearchText = "";
            return View(model);
        }
    }
}