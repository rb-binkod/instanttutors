using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstantTutors.Models
{
    [Table("TuitionSubjects")]
    public class TuitionSubjects
    {
        [Key][Required]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public int TutorId { get; set; } //FK
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [ForeignKey("TutorId")]
        public Tutors Tutor { get; set; }
    }
}