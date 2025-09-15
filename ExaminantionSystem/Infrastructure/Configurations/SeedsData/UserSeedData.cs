using ExaminantionSystem.Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExaminantionSystem.Infrastructure.Configurations.SeedsData
{
    public static class UserSeedData
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            // Seed Users first
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FullName = "Admin",
                    EmailAddress = "admin@edu.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123"),
                    Country = "Egypt",
                    City = "Cairo",
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false,
                },
                new User
                {
                    Id = 2,
                    FullName = "Instructor One",
                    EmailAddress = "instructor1@edu.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Instructor1"),
                    Country = "Egypt",
                    City = "Cairo",
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false,
                },
                new User
                {
                    Id = 3,
                    FullName = "Instructor Two",
                    EmailAddress = "instructor2@edu.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Instructor2"),
                    Country = "Egypt",
                    City = "Alexandria",
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false,
                },
                new User
                {
                    Id = 4,
                    FullName = "Basant Refaat",
                    EmailAddress = "basant2030@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("basant2030"),
                    Country = "Egypt",
                    City = "Cairo",
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false,
                },
                new User
                {
                    Id = 5,
                    FullName = "Muhammed Metwally",
                    EmailAddress = "muhammed@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Muhammed123"),
                    Country = "Egypt",
                    City = "Giza",
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false,
                }
            );

            // Seed Instructors (linked to Users)
            modelBuilder.Entity<Instructor>().HasData(
                new Instructor
                {
                    Id = 1,
                    UserId = 2, // Linked to Instructor One user
                    Bio = "Experienced software engineering instructor with 3+ years of teaching experience",
                    HireDate = DateTime.Now.AddYears(-3),
                    YearOfExperience = 3,
                    Specialization = "Software Engineering, Web Development",
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false
                },
                new Instructor
                {
                    Id = 2,
                    UserId = 3, // Linked to Instructor Two user
                    Bio = "Database specialist and data science enthusiast",
                    HireDate = DateTime.Now.AddYears(-2),
                    YearOfExperience = 6,
                    Specialization = "Database Management, Data Science",
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false
                }
            );

            // Seed Students (linked to Users)
            modelBuilder.Entity<Student>().HasData(
                new Student
                {
                    Id = 1,
                    UserId = 4, // Linked to Basant Refaat user
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false
                },
                new Student
                {
                    Id = 2,
                    UserId = 5, // Linked to Muhammed Metwally user
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false
                }

            );

        }
    }
}

