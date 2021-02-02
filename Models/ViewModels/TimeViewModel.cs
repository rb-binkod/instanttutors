using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InstantTutors.Models.ViewModels
{
    public class TimeViewModel
    {
        public int Id { get; set; }
        public int AvailabilityTime { get; set; }
        public string Time { get; set; }
        public bool Selected { get; set; }
    }
}