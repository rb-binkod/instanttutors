using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InstantTutors.Models.ViewModels
{
    public class StudentRegisterViewModel
    {
        public string UserId { get; set; }
        public int StudentId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        public string Mobile { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        //[Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        public string TimeZone { get; set; }
        [Required]
        public string Country { get; set; }
        public int Zip { get; set; }
        //public DateTime? DOB { get; set; }

        [Display(Name = "Profile Image")]
        public string ProfileImage { get; set; }

        [Display(Name = "Profile Image")]
        public HttpPostedFileBase File { get; set; }



        [Display(Name = "Tell Us About Your Hobbies")]
        public string Hobbies { get; set; }

        [Display(Name = "Summary (eg. status, bio, exp.)")]
        public string Bio { get; set; }

        [Required]
        [Display(Name = "Grade Level")]
        public string StudentGrade { get; set; }

        [Required]
        [Display(Name = "School Name")]
        public string StudentSchool { get; set; }


    }

}