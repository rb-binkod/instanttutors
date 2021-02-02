using InstantTutors.Areas.Student.ViewModels;
using InstantTutors.Areas.Tutor.ViewModels;
using InstantTutors.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InstantTutors.Areas.Admin.ViewModels
{
    public class AdminViewModel
    {
        public string UserId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public ApplicationUser User { get; set; }

        public int CurrentPage { get; set; }
        public int PerPage { get; set; }

        public List<StudentViewModel> StudentsList { get; set; }
        public List<TutorViewModel> TutorsList { get; set; }

    }

}