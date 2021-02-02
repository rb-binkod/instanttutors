using InstantTutors.Controllers;
using InstantTutors.Models;
using InstantTutors.Services;
using InstantTutors.Services.Admin;
using InstantTutors.Services.Student;
using InstantTutors.Services.Tutor;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Mvc;
using Unity;
using Unity.Injection;
using Unity.Mvc5;

namespace InstantTutors
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers
            // e.g. container.RegisterType<ITestService, TestService>();
            
            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>();
            container.RegisterType<IAccountService, AccountService>();
            container.RegisterType<ITutorService, TutorService>();
            container.RegisterType<ITuitionService, TuitionService>();
            container.RegisterType<ISessionService, SessionService>();
            container.RegisterType<IScheduleService, ScheduleService>();
            container.RegisterType<IStudentService, StudentService>();
            container.RegisterType<IDashboardService, DashboardService>();
            //container.RegisterType<AccountController>(new InjectionConstructor());

            container.RegisterType<AccountController>(new InjectionConstructor(
                 //container.Resolve<ApplicationUserManager>(),
                 //container.Resolve<ApplicationSignInManager>(),
                 container.Resolve<IAccountService>()));
            //container.RegisterType<HomeController>(new InjectionConstructor());
            container.RegisterType<HomeController>(new InjectionConstructor(
                 container.Resolve<IAccountService>()));

            //Areas.Tutor 
            container.RegisterType<Areas.Tutor.Controllers.DashboardController>(new InjectionConstructor(
                 container.Resolve<ITuitionService>(),
                 container.Resolve<ITutorService>()
            ));
            container.RegisterType<Areas.Tutor.Controllers.SessionController>(new InjectionConstructor(
                 container.Resolve<ITuitionService>(),
                 container.Resolve<ITutorService>()
            ));

            //Areas.Student
            container.RegisterType<Areas.Student.Controllers.SessionController>(new InjectionConstructor(
                  container.Resolve<ISessionService>(),
                  container.Resolve<IScheduleService>()
            ));
            container.RegisterType<Areas.Student.Controllers.SearchController>(new InjectionConstructor(
                  container.Resolve<ITutorService>()
            ));

            //Areas.Admin
            container.RegisterType<Areas.Admin.Controllers.DashboardController>(new InjectionConstructor(
                 container.Resolve<IDashboardService>()
            ));

            container.RegisterType<Areas.Admin.Controllers.UserController>(new InjectionConstructor(
                 container.Resolve<IDashboardService>()
            ));
            container.RegisterType<Areas.Admin.Controllers.SessionController>(new InjectionConstructor(
                  container.Resolve<ITuitionService>(),
                  container.Resolve<ITutorService>(),
                  container.Resolve<ISessionService>(),
                  container.Resolve<IScheduleService>()
            ));

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}