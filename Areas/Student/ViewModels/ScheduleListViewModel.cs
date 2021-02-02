using InstantTutors.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InstantTutors.Areas.Student.ViewModels
{
    public class ScheduleListViewModel
    {
        public int Id { get; set; }
        public List<SessionViewModel> Sessions { get; set; }
    }
}