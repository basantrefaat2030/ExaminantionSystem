using ExaminantionSystem.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace ExaminantionSystem.Infrastructure.Configurations
{
    public class ExamResultConfiguration : IEntityTypeConfiguration<ExamResult>
    {
        public void Configure(EntityTypeBuilder<ExamResult> builder)
        {
            // Configure ExamResult relationships
            builder
                .HasOne(er => er.Student)
                .WithMany(s => s.ExamResults)
                .HasForeignKey(er => er.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasOne(er => er.Exam)
                .WithMany(e => e.ExamResults)
                .HasForeignKey(er => er.ExamId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
