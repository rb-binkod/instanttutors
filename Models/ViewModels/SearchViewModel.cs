using InstantTutors.Areas.Student.ViewModels;
using InstantTutors.Areas.Tutor.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InstantTutors.Models.ViewModels
{
    public class SearchViewModel
    {
        public string UserId { get; set; } //FK
        [Required]
        [Display(Name = "Search (eg. name)")]
        [StringLength(50, MinimumLength =2, ErrorMessage = "Atleast two characters are required.")]
        public string SearchText { get; set; }
        public string Gender { get; set; }
        public string GradeLevel { get; set; }
        public string Subject { get; set; }
        public string Day { get; set; }

        public int CurrentPage { get; set; }
        public int PerPage { get; set; }

        public List<TutorViewModel> SearchTutorsList { get; set; }
        public List<StudentViewModel> SearchStudentsList { get; set; }
    }
}
