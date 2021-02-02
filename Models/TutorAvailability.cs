using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstantTutors.Models
{
    [Table("TutorAvailability")]
    public class TutorAvailability
    {
        [Key][Required]
        public int Id { get; set; }
        public DayOfWeek Day { get; set; }
        public int Time { get; set; }
        [Required]
        public int TutorId { get; set; } //FK
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [ForeignKey("TutorId")]
        public Tutors Tutor { get; set; }
    }
}