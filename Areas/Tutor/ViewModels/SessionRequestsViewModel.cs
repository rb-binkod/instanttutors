using InstantTutors.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InstantTutors.Areas.Tutor.ViewModels
{
    public class SessionRequestsViewModel
    {
        public Sessions Session { get; set; }
        public string TutorUserId { get; set; }
        public string StudentUserId { get; set; }

        public ApplicationUser StudentUser { get; set; }
    }
}