using ExaminantionSystem.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities.Dtos.Course
{
    public class UpdateEnrollmentStatusDto
    {
        [Required(ErrorMessage = "Status is required")]
        public RequestStatus Status { get; set; }
    }
}
