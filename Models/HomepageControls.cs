using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstantTutors.Models
{
    [Table("HomepageControls")]
    public class HomepageControls
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public string FirstSliderHeading { get; set; }
        public string FirstSliderHeadingText { get; set; }
        public string FirstSliderHeadingImage { get; set; }
        public string SecondSliderHeading { get; set; }
        public string SecondSliderHeadingText { get; set; }
        public string SecondSliderHeadingImage { get; set; }
        public string ThirdSliderHeading { get; set; }
        public string ThirdSliderHeadingText { get; set; }
        public string FacebookLink { get; set; }
        public string TwitterLink { get; set; }
        public string InstagramLink { get; set; }
        public string YoutubeLink { get; set; }//Testimonials
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}