using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InstantTutors.Models.ViewModels
{

    public class ContactUsViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Mobile { get; set; }

        [Required]
        [Display(Name = "Comment")]
        public string Comment { get; set; }
    }
}