using InstantTutors.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace InstantTutors.Areas.Tutor.ViewModels
{
    public class UpdateProfileViewModel
    {
        public string UserId { get; set; }
        public int TutorId { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        public string Mobile { get; set; }

        
        [Display(Name = "Experience")]
        public string Experience { get; set; }

        //[Required]
        public string Address { get; set; }
        [Required]
        [Display(Name = "City/State")]
        public string City { get; set; }
        public string TimeZone { get; set; }
        [Required]
        public string Country { get; set; }
        public int Zip { get; set; }
        //public DateTime? DOB { get; set; }
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "Grade Level")]
        public string GradeLevel { get; set; }

        [Display(Name = "Name of School")]
        public string NameOfSchool { get; set; }

        [Display(Name = "Questions/Comments/Concerns")]
        public string Concerns { get; set; }

        [Required(ErrorMessage = "Previous Subjects is Required")]
        [Display(Name = "What subjects have you taken up to now?")]
        public string PreviousSubjects { get; set; }

        [Display(Name = "Profile Image")]
        public string ProfileImage { get; set; }
        
        //[Required]
        //[ValidateFile(ErrorMessage = "Only JPEG/PNG image smaller than 1MB is allowed.")]
        [Display(Name = "Profile Image")]
        public HttpPostedFileBase File { get; set; }


        [Display(Name = "Tell Us About Your Hobbies")]
        public string Hobbies { get; set; }

        [Display(Name = "Summary (eg. status, bio, exp.)")]
        public string Bio { get; set; }
        public string Role { get; set; }

    }

}