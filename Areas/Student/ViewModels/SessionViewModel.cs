using InstantTutors.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InstantTutors.Areas.Student.ViewModels
{
    public class SessionViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Session Title/Subject")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Session Description")]
        public string Description { get; set; }

        [Display(Name = "Questions/Comments/Concerns")]
        public string Concerns { get; set; }
        
        [Required]
        [Display(Name = "Preferred Method of Communication")]
        public string CommunicationMethod { get; set; }

        public string UserId { get; set; } //FK
        public DateTime CreatedDate { get; set; }
        public ApplicationUser User { get; set; }
        public List<SessionScheduleViewModel> SessionSchedules { get; set; }

        [Required]
        [Display(Name = "Tutor")]
        public string TutorUserId { get; set; }
        public List<SelectListItem> TutorsList { get; set; }

        //For Admin
        [Display(Name = "Student")]
        public string StudentUserId { get; set; }
        public List<SelectListItem> StudentsList { get; set; }

        public string CreatedBy { get; set; }
    }

    public class AdminSessionViewModel : SessionViewModel
    {

        //For Admin
        [Required]
        [Display(Name = "Student")]
        public string StudentUserId { get; set; }
        public List<SelectListItem> StudentsList { get; set; }
    }
}