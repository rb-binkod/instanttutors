using System;
using System.ComponentModel.DataAnnotations;

namespace InstantTutors.Areas.Student.ViewModels
{
    public class UpdateProfileViewModel
    {
        public string UserId { get; set; }
        //public int TutorId { get; set; }

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

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        public string Mobile { get; set; }

        //[Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }
        public string TimeZone { get; set; }
        [Required]
        public string Country { get; set; }
        public int Zip { get; set; }
        //public DateTime? DOB { get; set; }
        public DateTime? CreatedDate { get; set; }        

        [Display(Name = "Questions/Comments/Concerns")]
        public string Concerns { get; set; }

        [Display(Name = "Profile Image")]
        public string ProfileImage { get; set; }

        [Display(Name = "Tell Us About Your Hobbies")]
        public string Hobbies { get; set; }

        [Display(Name = "Summary (eg. status, bio, exp.)")]
        public string Bio { get; set; }

        [Display(Name = "Grade Level")]
        public string StudentGrade { get; set; }

        [Display(Name = "School Name")]
        public string StudentSchool { get; set; }
        public string Role { get; set; }
    }

}