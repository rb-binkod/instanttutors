using InstantTutors.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System;

[assembly: OwinStartup(typeof(InstantTutors.Startup))]
namespace InstantTutors
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            try { CreateDefaultRolesandUsers(); } catch { }
        }

        // In this method we will create default User roles and Admin user for login    
        private void CreateDefaultRolesandUsers()
        {
            ApplicationDbContext _dbContext = new ApplicationDbContext();

            var roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(_dbContext));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_dbContext));

            //creating a default Admin User     
            if (!roleManager.RoleExists("Admin"))
            {
                // first we create Admin role
                var role = new ApplicationRole();
                role.Name = "Admin";
                role.Description = "Administrator";
                role.CreatedDate = DateTime.Now;
                roleManager.Create(role);

                //Here we create a Admin user who will maintain the website    
                var user = new ApplicationUser();
                user.UserName = "admin@instanttutors.com";
                user.Email = "admin@instanttutors.com";
                user.EmailConfirmed = true;
                string userPWD = "Admin@99";
                user.CreatedDate = DateTime.Now;

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin    
                if (chkUser.Succeeded)
                {
                    var result = UserManager.AddToRole(user.Id, "Admin");
                }
            }

            // creating Creating Tutor role     
            if (!roleManager.RoleExists("Tutor"))
            {
                var role = new ApplicationRole();
                role.Name = "Tutor";
                role.Description = "Tutor";
                role.CreatedDate = DateTime.Now;
                roleManager.Create(role);
            }

            // creating Creating Student/Parent role     
            if (!roleManager.RoleExists("Student"))
            {
                var role = new ApplicationRole();
                role.Name = "Student";
                role.Description = "Parent/Student";
                role.CreatedDate = DateTime.Now;
                roleManager.Create(role);
            }
        }
    }
}
