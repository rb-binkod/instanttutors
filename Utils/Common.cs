using InstantTutors.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InstantTutors.Utils
{
    public static class Common
    {
        public static List<TimeViewModel> GetTiming()
        {
            return new List<TimeViewModel>()
            {
                new TimeViewModel { AvailabilityTime=10, Time ="10:00 AM", Selected = false },
                new TimeViewModel { AvailabilityTime=11,Time="11:00 AM", Selected = false },
                new TimeViewModel { AvailabilityTime=12,Time="12:00 PM", Selected = false },
                new TimeViewModel { AvailabilityTime=01,Time="01:00 PM", Selected = false },
                new TimeViewModel { AvailabilityTime=02,Time="02:00 PM", Selected = false },
                new TimeViewModel { AvailabilityTime=03,Time="03:00 PM", Selected = false },
                new TimeViewModel { AvailabilityTime=04,Time="04:00 PM", Selected = false },
                new TimeViewModel { AvailabilityTime=05,Time="05:00 PM", Selected = false },
                new TimeViewModel { AvailabilityTime=06,Time="06:00 PM", Selected = false },
                new TimeViewModel { AvailabilityTime=07,Time="07:00 PM", Selected = false },
            };
        }

        public static string GetTime(int time)
        {
            return time == 10 ? "10:00 AM" : time == 11 ? "11:00 AM" : time == 12 ? "12:00 AM" : time == 01 ? "01:00 PM" :
                time == 02 ? "02:00 PM" : time == 03 ? "03:00 PM" : time == 04 ? "04:00 AM" : time == 05 ? "05:00 PM" :
                time == 06 ? "06:00 PM" : "07:00 PM";
        }
    }
}