using InstantTutors.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InstantTutors.Areas.Student.ViewModels
{
    public class SessionScheduleViewModel
    {
        public int Id { get; set; }
        public DayOfWeek Day { get; set; }
        public int Time { get; set; }
        public List<TimeViewModel> Timing { get; set; }
        public int SessionId { get; set; } //FK
        public string UserId { get; set; } //FK
        public DateTime CreatedDate { get; set; }
    }
}