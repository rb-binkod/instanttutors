using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstantTutors.Models
{
    [Table("AboutControls")]
    public class AboutControls
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public string FirstHeading { get; set; }
        public string FirstHeadingText { get; set; }
        public string FirstHeadingImage { get; set; }
        public string SecondHeading { get; set; }
        public string SecondHeadingText { get; set; }
        public string SecondHeadingImage { get; set; }
        public string ThirdHeading { get; set; }
        public string ThirdHeadingText { get; set; }
        public string ThirdHeadingImage { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}