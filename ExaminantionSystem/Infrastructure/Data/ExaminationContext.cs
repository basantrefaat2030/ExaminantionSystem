using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Infrastructure.Configurations;
using ExaminantionSystem.Infrastructure.Configurations.SeedsData;
using Microsoft.EntityFrameworkCore;

namespace ExaminantionSystem.Infrastructure.Data
{
    public class ExaminationContext :DbContext
    {
        public ExaminationContext(DbContextOptions<ExaminationContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<ExamResult> ExamResult { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new StudentCourseConfiguration());
            modelBuilder.ApplyConfiguration(new ExamQuestionConfiguration());
            modelBuilder.ApplyConfiguration(new ExamResultConfiguration());

            UserSeedData.Seed(modelBuilder);


        }


    }
}
