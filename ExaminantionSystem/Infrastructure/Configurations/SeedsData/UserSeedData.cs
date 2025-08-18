using ExaminantionSystem.Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExaminantionSystem.Infrastructure.Configurations.SeedsData
{
    public static class UserSeedData
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                             new User
                             {
                                 Id = 1,
                                 FullName = "Admin",
                                 EmailAddress = "Admin123@school.com",
                                 PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123"),
                                 Country = "Egypt",
                                 City = "Cairo",
                                 CreatedAt = DateTime.Now,
                                 IsActive = true,
                                 IsDeleted = false,
                             }
            ); ;

            // Seed initial data
            modelBuilder.Entity<Instructor>().HasData(
                              new Instructor
                              {
                                  Id = 2,
                                  FullName = "Instructor1",
                                  City = "Cairo",
                                  Country = "Egypt",
                                  CreatedAt = DateTime.UtcNow,
                                  HireDate = DateTime.Now,
                                  PoheNumber = "01065692974",
                                  EmailAddress = "Instructor1@gmail.com",
                                  IsActive = true,
                                  YearOfExperience = 3,
                                  PasswordHash = BCrypt.Net.BCrypt.HashPassword("Instructor1")
                              },

                              new Instructor
                              {
                                  Id = 3,
                                  FullName = "Instructor2",
                                  City = "Cairo",
                                  Country = "Egypt",
                                  CreatedAt = DateTime.UtcNow,
                                  HireDate = DateTime.Now,
                                  PoheNumber = "01065692974",
                                  EmailAddress = "Instructor2@gmail.com",
                                  IsActive = true,
                                  YearOfExperience = 4,
                                  PasswordHash = BCrypt.Net.BCrypt.HashPassword("Instructor2")

                              }
            );


            // Seed initial data
            modelBuilder.Entity<Student>().HasData(
                              new Student
                              {
                                  Id = 4,
                                  FullName = "Basant Refaat ",
                                  City = "Cairo",
                                  Country = "Egypt",
                                  CreatedAt = DateTime.UtcNow,
                                  PoheNumber = "01065692974",
                                  EmailAddress = "basant2030@gmail.com",
                                  IsActive = true,
                                  IsDeleted = false,
                                  PasswordHash = BCrypt.Net.BCrypt.HashPassword("basant2030")
                              },

                              new Student
                              {
                                  Id = 5,
                                  FullName = "Muhammed Metwally",
                                  City = "Cairo",
                                  Country = "Egypt",
                                  CreatedAt = DateTime.UtcNow,
                                  PoheNumber = "01065692974",
                                  EmailAddress = "MuhammedMetwally2@gmail.com",
                                  IsActive = true,
                                  IsDeleted = false,
                                  PasswordHash = BCrypt.Net.BCrypt.HashPassword("MuhammedMetwally2")
                              },

                              new Student
                              {
                                  Id = 6,
                                  FullName = "Mai Mostafa",
                                  City = "Cairo",
                                  Country = "Egypt",
                                  CreatedAt = DateTime.UtcNow,
                                  PoheNumber = "01068692974",
                                  EmailAddress = "MaiMostafa20@gmail.com",
                                  IsActive = true,
                                  IsDeleted = false,
                                  PasswordHash = BCrypt.Net.BCrypt.HashPassword("MaiMostafa20")
                              }
            );
        }
    }
}
