using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Infrastructure.Data;
using ExaminantionSystem.Infrastructure.Repositories;
using ExaminantionSystem.Infrastructure;
using ExaminantionSystem.Service;
using Microsoft.EntityFrameworkCore;
using ExaminantionSystem.MappingProfile;
using ExaminantionSystem.Entities.ViewModels.Course.MappingProfile;
using ExaminantionSystem.Entities.Dtos.Course.MappingProfile;
using ExaminantionSystem.Middleware;
using Microsoft.Extensions.DependencyInjection;
using ExaminantionSystem.Entities.Dtos.Instructor.MappingProfile;
using ExaminantionSystem.Entities.Dtos.Student.MappingProfile;
using ExaminantionSystem.Entities.Dtos.Ouestion.MappingProfile;
using ExaminantionSystem.Entities.Dtos.Choice.MappingProfile;
using ExaminantionSystem.Entities.Dtos.Exam.MappingProfile;
using ExaminantionSystem.Helper;

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
                 .LogTo(Console.WriteLine, LogLevel.Information)
                 .EnableSensitiveDataLogging(true));

            // Register all repositories
            services.AddScoped(typeof(Repository<>));
            services.AddScoped<CourseRepository>();
            services.AddScoped<ExamRepository>();
            services.AddScoped<QuestionRepository>();
            services.AddScoped<ChoiceRepository>();
            services.AddScoped<InstructorRepository>();
            services.AddScoped<StudentRepository>();

            // Register all services
            services.AddScoped<CourseService>();
            services.AddScoped<ExamService>();
            services.AddScoped<QuestionService>();
            services.AddScoped<ChoiceService>();
            services.AddScoped<StudentService>();
            services.AddScoped<InstructorService>();
            services.AddScoped<UserService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            //register Auto Mapper for Services
            services.AddAutoMapper(typeof(CourseServiceMappingProfile));


            //registeraion automapper for services(Dtos)
            services.AddAutoMapper(typeof (InstructorServiceMappingProfile));
            services.AddAutoMapper(typeof(StudentServiceAutoMapping));
            services.AddAutoMapper(typeof(QuestionServiceMappingProfile));
            services.AddAutoMapper(typeof(ChoiceServiceMappingProfile));
            services.AddAutoMapper(typeof(ExamServiceMappingProfile));
            services.AddAutoMapper(typeof(CourseServiceMappingProfile));

            //registeraion automapper for controllers
            services.AddAutoMapper(typeof(GeneralMapping));
            services.AddAutoMapper(typeof(CourseMappingProfile));


            // register middleware 
            services.AddScoped<TransactionMiddleware>();


            return services;
        }
    }
}
