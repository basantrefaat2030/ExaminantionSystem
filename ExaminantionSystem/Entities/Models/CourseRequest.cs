using ExaminantionSystem.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExaminantionSystem.Entities.Models
{
    public class CourseRequest : BaseEntity
    {
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [ForeignKey("Instructor")]
        public int InstructorId {  get; set; }

        [ForeignKey("Course")]
        public int CourseId {  get; set; }

        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public DateTime? EnrolledAt { get; set; }

        public RequestStatus RequestStatus { get; set; }
        // Navigation properties
        public virtual Student Student { get; set; }
        public virtual Instructor Instructor { get; set; }  
        public virtual Course Course { get; set; }


    }
}
