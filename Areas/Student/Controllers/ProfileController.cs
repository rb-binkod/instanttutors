using InstantTutors.Areas.Student.ViewModels;
using InstantTutors.Helpers;
using InstantTutors.Models;
using InstantTutors.Models.ViewModels;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Unity;

namespace InstantTutors.Areas.Student.Controllers
{
    //[RedirectLoginFilter]
    [Authorize(Roles = "Student")]
    //[OutputCache(Duration = 360)]
    public class ProfileController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        [InjectionConstructor]
        public ProfileController() { }
        public ProfileController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
            var _user = await UserManager.FindByEmailAsync(User.Identity.Name);
            UpdateProfileViewModel model = new UpdateProfileViewModel()
            {
                Email = _user.Email,
                Title = _user.Title,
                Gender = _user.Gender,
                FirstName = _user.FirstName,
                LastName = _user.LastName,
                Mobile = _user.PhoneNumber,
                Bio = _user.Bio,
                Hobbies = _user.Hobbies,
                StudentGrade = _user.StudentGrade,
                StudentSchool = _user.StudentSchool,
                ProfileImage = _user.ProfileImage,
                Address = _user.Address,
                City = _user.City,
                TimeZone = _user.TimeZone,
                Country = _user.Country,
                Zip = _user.Zip,
                //DOB = _user.DOB,
                CreatedDate = DateTime.Now,
                //UpdatedDate = DateTime.Now
                UserId = _user.Id,
                //Concerns = _tutor.Concerns,
            };

            return View(model);
        }

        //[OutputCache(Duration = 360, VaryByParam = "id")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
                return View("Index");

            var _user = await UserManager.FindByIdAsync(id);
            UpdateProfileViewModel model = new UpdateProfileViewModel()
            {
                Email = _user.Email,
                Title = _user.Title,
                Gender = _user.Gender,
                FirstName = _user.FirstName,
                LastName = _user.LastName,
                Mobile = _user.PhoneNumber,
                Bio = _user.Bio,
                Hobbies = _user.Hobbies,
                StudentGrade = _user.StudentGrade,
                StudentSchool = _user.StudentSchool,
                ProfileImage = _user.ProfileImage,
                Address = _user.Address,
                City = _user.City,
                TimeZone = _user.TimeZone,
                Country = _user.Country,
                Zip = _user.Zip,
                //DOB = _user.DOB,
                CreatedDate = DateTime.Now,
                //UpdatedDate = DateTime.Now
                UserId = _user.Id,
                //Concerns = _tutor.Concerns,
            };

            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UpdateProfileViewModel model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var _user = await UserManager.FindByIdAsync(model.UserId);
                if (_user == null)
                {
                    ViewBag.success = "<ul><li><p style='color:red'>Error. Something went wrong, please try again.</p></li></ul>";
                    return View(model);
                }
                //profile upload
                if (file != null)
                {
                    var supportedTypes = new[] { "jpg", "jpeg", "png", "gif" };
                    var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                    if ((file.ContentLength / 1024 > 1024) || (!supportedTypes.Contains(fileExt)))
                    {
                        ViewBag.success = "<ul><li><p style='color:red'>Error: Image size more than 512KB are not allowed.</p></li></ul>";
                        return View(model);
                    }
                    else
                    {
                        var profileImage = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        file.SaveAs(HttpContext.Server.MapPath("~/Content/images/student/") + profileImage);
                        _user.ProfileImage = profileImage;
                    }
                }

                _user.Title = model.Title;
                _user.Gender = model.Gender;
                _user.FirstName = model.FirstName;
                _user.LastName = model.LastName;
                _user.PhoneNumber = model.Mobile;
                _user.Bio = model.Bio;
                _user.Hobbies = model.Hobbies;
                _user.StudentGrade = model.StudentGrade;
                _user.StudentSchool = model.StudentSchool;
                _user.Address = model.Address;
                _user.City = model.City;
                //_user.TimeZone = model.TimeZone;
                _user.Country = model.Country;
                _user.Zip = model.Zip;
                //_user.DOB = DateTime.Now.AddYears(-29),
                _user.UpdatedDate = DateTime.Now;
                await UserManager.UpdateAsync(_user);

                ModelState.Clear();
                ViewBag.success = "<ul><li><p style='color:green'>Profile has been updated successfully!! </p></li></ul>";
                return View(model);
            }
            return View(model);
        }

        public async Task<ActionResult> ViewProfile(string id)
        {
            var _user = await UserManager.FindByIdAsync(id);
            if (_user == null)
                return RedirectToAction("Index", "Dashboard", new { Area = "Student" });

            var IsTutor = await UserManager.IsInRoleAsync(_user.Id, "Tutor");

            UpdateProfileViewModel model = new UpdateProfileViewModel()
            {
                Email = _user.Email,
                Title = _user.Title,
                Gender = _user.Gender,
                FirstName = _user.FirstName,
                LastName = _user.LastName,
                Mobile = _user.PhoneNumber,
                Bio = _user.Bio,
                Hobbies = _user.Hobbies,
                StudentGrade = _user.StudentGrade,
                StudentSchool = _user.StudentSchool,
                ProfileImage = _user.ProfileImage,
                Address = _user.Address,
                City = _user.City,
                TimeZone = _user.TimeZone,
                Country = _user.Country,
                Zip = _user.Zip,
                //DOB = _user.DOB,
                CreatedDate = DateTime.Now,
                //UpdatedDate = DateTime.Now
                UserId = _user.Id,
                //Concerns = _tutor.Concerns,
                Role = IsTutor ? "Tutor" : "Student"
            };

            return View(model);
        }
    }
}
