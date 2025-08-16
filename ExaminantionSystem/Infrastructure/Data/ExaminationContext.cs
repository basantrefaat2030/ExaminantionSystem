using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Infrastructure.Configurations;
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
        public DbSet<StudentExam> StudentExams { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new StudentCourseConfiguration());
            modelBuilder.ApplyConfiguration(new ExamQuestionConfiguration());
            modelBuilder.ApplyConfiguration(new StudentExamConfiguration());

            // Seed initial data
            modelBuilder.Entity<Instructor>().HasData(
                              new Instructor
                              {
                                  Id = 1,
                                  FullName = "Instructor1",
                                  City = "cairo",
                                  Country = "Egypt",
                                  CreatedAt = DateTime.UtcNow,
                                  HireDate = DateTime.Now,
                                  PoheNumber = "01065692974",
                                  EmailAddress = "Fiction@gmail.com",
                                  IsActive = true,
                                  YearOfExperience = 3
                              },

                                new Instructor
                                {
                                    Id = 2,
                                    FullName = "Instructor2",
                                    City = "cairo",
                                    Country = "Egypt",
                                    CreatedAt = DateTime.UtcNow,
                                    HireDate = DateTime.Now,
                                    PoheNumber = "01065692974",
                                    EmailAddress = "Ins2@gmail.com",
                                    IsActive = true,
                                    YearOfExperience = 4
                                }
            );


            // Seed initial data
            modelBuilder.Entity<Student>().HasData(
                              new Student
                              {
                                  Id = 1,
                                  FullName = "Basant Refaat ",
                                  City = "cairo",
                                  Country = "Egypt",
                                  CreatedAt = DateTime.UtcNow,
                                  PoheNumber = "01065692974",
                                  EmailAddress = "basant@gmail.com",
                                  IsActive = true,
                                  IsDeleted = false
                              },

                                new Student
                                {
                                    Id = 2,
                                    FullName = "Muhammed Metwally",
                                    City = "cairo",
                                    Country = "Egypt",
                                    CreatedAt = DateTime.UtcNow,
                                    PoheNumber = "01065692974",
                                    EmailAddress = "Muhammed2@gmail.com",
                                    IsActive = true,
                                    IsDeleted = false,
                                }
            );
        }


    }
}
