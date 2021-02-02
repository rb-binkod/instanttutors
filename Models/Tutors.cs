using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstantTutors.Models
{
    [Table("Tutors")]
    public class Tutors
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; } //FK
        public string Experience { get; set; }
        public string GradeLevel { get; set; }
        public string NameOfSchool { get; set; }
        public string Concerns { get; set; }
        public string PreviousSubjects { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public ICollection<TuitionSubjects> TuitionSubjects { get; set; }
        public ICollection<TutorAvailability> TutorAvailability { get; set; }
    }
}