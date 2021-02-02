using InstantTutors.Areas.Tutor.ViewModels;
using InstantTutors.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace InstantTutors.Models.ViewModels
{
    public class TutorRegisterViewModel
    {
        public string UserId { get; set; }
        public int TutorId { get; set; }

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
        [Display(Name = "City/State")]
        public string City { get; set; }
        public string TimeZone { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public int Zip { get; set; }
        //public DateTime? DOB { get; set; }
        public DateTime? CreatedDate { get; set; }

        //[Display(Name = "Profile Image")]
        public string ProfileImage { get; set; }

        //[ValidateFile(ErrorMessage = "Only JPEG/PNG image smaller than 1MB is allowed.")]
        [Display(Name = "Profile Image")]
        public HttpPostedFileBase File { get; set; }

        [Display(Name = "Tell Us About Your Hobbies")]
        public string Hobbies { get; set; }

        [Display(Name = "Summary (eg. status, bio, exp.)")]
        public string Bio { get; set; }

        [Display(Name = "Experience")]
        public string Experience { get; set; }

        [Required]
        [Display(Name = "Grade Level")]
        public string GradeLevel { get; set; }

        [Required]
        [Display(Name = "School Name")]
        public string NameOfSchool { get; set; }

        [Display(Name = "Questions/Comments/Concerns")]
        public string Concerns { get; set; }

        [Required(ErrorMessage = "This field is Required")]
        [Display(Name = "What subjects have you taken up to now?")]
        public string PreviousSubjects { get; set; }
        
        public List<TuitionSubjectsViewModel> TuitionSubjects { get; set; }

    }

}