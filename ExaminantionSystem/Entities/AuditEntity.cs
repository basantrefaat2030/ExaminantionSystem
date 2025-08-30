using System.ComponentModel.DataAnnotations;

namespace ExaminantionSystem.Entities
{
    public class AuditEntity
    {
        public bool IsDeleted { get; set; } = false;

        [DataType(DataType.DateTime)]
        public DateTime? DeletedAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? CreatedBy {  get; set; }

        public int? UpdatedAt { get; set; }  

        public bool IsActive { get; set; } = true;

    }
}
