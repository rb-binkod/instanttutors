using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InstantTutors.Areas.Tutor.ViewModels
{
    public class TuitionSubjectsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Selected { get; set; }
        public int TutorId { get; set; } //FK
        public DateTime CreatedDate { get; set; }
    }
}