using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstantTutors.Models
{
    [Table("SessionSchedule")]
    public class SessionSchedule
    {
        [Key][Required]
        public int Id { get; set; }
        [Required]
        public int SessionId { get; set; } //FK
        public DayOfWeek Day { get; set; }
        public int Time { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [ForeignKey("SessionId")]
        public Sessions Session { get; set; }
    }
}