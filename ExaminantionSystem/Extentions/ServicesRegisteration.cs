using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Infrastructure.Data;
using ExaminantionSystem.Infrastructure.Repositories;
using ExaminantionSystem.Infrastructure;
using ExaminantionSystem.Service;
using Microsoft.EntityFrameworkCore;
using ExaminantionSystem.MappingProfile;
using ExaminantionSystem.Entities.ViewModels.Course.MappingProfile;

namespace ExaminantionSystem.Extentions
{
    public static class ServicesRegisteration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext (if not already registered)
            services.AddDbContext<ExaminationContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ExaminationConnection"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                 .LogTo(Console.WriteLine, LogLevel.Information));

            // Register all repositories
            services.AddScoped<CourseRepository>();
            services.AddScoped<ExamRepository>();
            services.AddScoped<QuestionRepository>();
            services.AddScoped<ChoiceRepository>();
            services.AddScoped<InstructorRepository>();
            services.AddScoped<StudentRepository>();
            services.AddScoped<Repository<ExamQuestion>>();
            services.AddScoped<Repository<StudentCourse>>();
            services.AddScoped<Repository<ExamResult>>();
            services.AddScoped<Repository<StudentAnswer>>();


            // Register all services
            services.AddScoped<CourseService>();
            services.AddScoped<ExamService>();
            services.AddScoped<QuestionService>();
            services.AddScoped<ChoiceService>();
            services.AddScoped<StudentService>();
            services.AddScoped<InstructorService>();

            //registeraion for automapper
            services.AddAutoMapper(typeof(GeneralMapping));
            services.AddAutoMapper(typeof(CourseMappingProfile));

            return services;
        }
    }
}
