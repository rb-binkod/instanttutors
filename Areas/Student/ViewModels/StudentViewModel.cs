using InstantTutors.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InstantTutors.Areas.Student.ViewModels
{
    public class StudentViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } //FK

        public DateTime? CreatedDate { get; set; }

        public ApplicationUser User { get; set; }

        public int CurrentPage { get; set; }
        public int PerPage { get; set; }

        public List<Sessions> SessionsList { get; set; }
        public List<StudentSessions> SessionsVMList { get; set; }
    }
    public class StudentSessions
    {
        public Sessions Session { get; set; }
        public string TutorUserId { get; set; }

        public ApplicationUser TutorUser { get; set; }

        public string StudentUserId { get; set; }
        public ApplicationUser StudentUser { get; set; }
    }
}