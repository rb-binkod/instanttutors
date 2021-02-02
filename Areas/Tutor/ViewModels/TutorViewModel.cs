using InstantTutors.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InstantTutors.Areas.Tutor.ViewModels
{
    public class TutorViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } //FK

        //[Required(ErrorMessage = "Experience is Required")]
        public string Experience { get; set; }

        [Required(ErrorMessage = "Grade Level is Required")]
        [Display(Name = "Grade Level")]
        public string GradeLevel { get; set; }

        [Required(ErrorMessage = "Name of School is Required")]
        [Display(Name = "Name of School")]
        public string NameOfSchool { get; set; }

        [Display(Name = "Questions/Comments/Concerns")]
        public string Concerns { get; set; }

        [Required(ErrorMessage = "Previous Subjects is Required")]
        [Display(Name = "What subjects have you taken up to now?")]
        public string PreviousSubjects { get; set; }
        public DateTime? CreatedDate { get; set; }

        public ApplicationUser User { get; set; }

        [Required]
        public List<TuitionSubjectsViewModel> TuitionSubjects { get; set; }
        public List<TutorAvailabilityViewModel> TutorAvailability { get; set; }
        public List<SessionRequestsViewModel> SessionRequests { get; set; }
    }
}