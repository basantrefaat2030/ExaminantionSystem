
using ExaminantionSystem.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace ExaminantionSystem.Infrastructure.Configurations
{
    public class ExamQuestionConfiguration : IEntityTypeConfiguration<ExamQuestion>
    {
        public void Configure(EntityTypeBuilder<ExamQuestion> builder)
        {
            //builder.HasKey(eq => new {eq.ExamId , eq.QuestionId});

            builder
              .HasOne(eq => eq.Exam)
              .WithMany(e => e.ExamQuestions)
              .HasForeignKey(eq => eq.ExamId)
              .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasOne(eq => eq.Question)
                .WithMany(q => q.ExamQuestions)
                .HasForeignKey(eq => eq.QuestionId)
                .OnDelete(DeleteBehavior.NoAction);


        }
    }
}