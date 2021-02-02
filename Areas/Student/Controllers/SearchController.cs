using InstantTutors.Areas.Student.ViewModels;
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

namespace InstantTutors.Areas.Student.Controllers
{
    [Authorize(Roles = "Student")]
    public class SearchController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ITutorService _tutorService;
        [InjectionConstructor]
        public SearchController(ITutorService tutorService)
        {
            _tutorService = tutorService;
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
        public async Task<ActionResult> Index(SearchViewModel model, string Reset)
        {
            if (!string.IsNullOrEmpty(Reset))
            {
                ModelState.Clear();
                model = new SearchViewModel();
                return View(model);
            }
            else if (ModelState.IsValid)
            {
                model.SearchTutorsList = await _tutorService.SearchTutorsAsync(model);
            }
            return View(model);
        }

    }
}
