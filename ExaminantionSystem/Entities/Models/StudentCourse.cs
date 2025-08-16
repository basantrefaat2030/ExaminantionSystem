using System.ComponentModel.DataAnnotations.Schema;

namespace ExaminantionSystem.Entities.Models
{
    public class StudentCourse :AuditEntity
    {
        //composite key
        [ForeignKey("Student")]
        public int StudentId { get; set; }
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public DateTime? EnrolledAt { get; set; } 


        // Navigation properties
        public virtual Student Student { get; set; }
        public virtual Course Course { get; set; }
    }
}
