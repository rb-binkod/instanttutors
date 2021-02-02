using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InstantTutors.Models
{
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        public string Title { get; set; }
        public string Gender { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string TimeZone { get; set; }
        public string Country { get; set; }
        public int Zip { get; set; }
        public DateTime? DOB { get; set; }
        public string ProfileImage { get; set; }
        public bool HasDisabled { get; set; }
        public bool HasSocialLogin { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Hobbies { get; set; }
        public string StudentGrade { get; set; }
        public string StudentSchool { get; set; }
    }
}