using ExaminantionSystem.Entities.Dtos.Courcse;
using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Entities.Wrappers;
using ExaminantionSystem.Infrastructure;
using ExaminantionSystem.Infrastructure.Repositories;

namespace ExaminantionSystem.Service
{
    //public interface ICourseService
    //{
    //    Task<PagedResponse<CourseDTO>> GetPaginatedCoursesAsync(int pageNumber, int pageSize);
    //    Task<Response<CourseDTO>> GetCourseByIdAsync(int id);
    //    Task<Response<CourseDTO>> CreateCourseAsync(CourseDTO entity, int instructorId);
    //    Task<Response<CourseDTO>> UpdateCourseAsync(int id, CourseDTO entity);
    //    Task<Response<bool>> DeleteCourseAsync(int id);

    //}
    public class CourseService
    {
        private readonly CourseRepository _courseRepository;
        private readonly InstructorRepository _instructorRepository;
        private readonly StudentRepository _studentRepository;
        public readonly ExamRepository _examRepository;


        public CourseService(
          CourseRepository courseRepository,
          InstructorRepository instructorRepository,
          StudentRepository studentRepository,
          ExamRepository examRepository)
        {
            _courseRepository = courseRepository;
            _instructorRepository = instructorRepository;
            _studentRepository = studentRepository;
            _examRepository = examRepository;
        }

        public async Task<Response<CourseDto>> CreateCourseAsync(CreateCourseDto dto)
        {
            try
            {
                // Validate instructor exists
                var instructor = await _instructorRepository.GetByIdAsync(dto.InstructorId);
                if (instructor == null)
                {
                    return Response<CourseDto>.Fail(
                        ErrorType.NotFound,
                        new ErrorDetail("INSTRUCTOR_NOT_FOUND", "Instructor not found", $"Instructor with ID {dto.InstructorId} not found", "instructorId")
                    );
                }

                // Check for duplicate course title
                var titleExists = await _courseRepository.CourseTitleExistsAsync(dto.Title, dto.InstructorId);
                if (titleExists)
                {
                    return Response<CourseDto>.Fail(
                        ErrorType.Conflict,
                        new ErrorDetail("COURSE_TITLE_EXISTS", "Course title exists", $"Course with title '{dto.Title}' already exists", "title")
                    );
                }

                // Create course logic...
                var course = new Course
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    InstructorId = dto.InstructorId,
                    Hours = dto.Hours,
                    Budget = dto.Budget,
                    CreatedBy = 1,
                    CreatedAt = DateTime.UtcNow
                };

                await _courseRepository.AddAsync(course);
                await _courseRepository.SaveChangesAsync();

                var courseDto = new CourseDto
                {
                    Id = course.Id,
                    Title = course.Title,
                    Description = course.Description,
                    InstructorId = course.InstructorId,
                    Hours = dto.Hours,
                    Budget = dto.Budget,
                };

                return Response<CourseDto>.Success(courseDto);
            }
            catch (Exception ex)
            {
                return Response<CourseDto>.Fail(
                    ErrorType.Critical,
                    new ErrorDetail("COURSE_CREATION_ERROR", "Failed to create course", ex.Message)
                );
            }

        }

        public async Task<Response<bool>> DeleteCourseAsync(int id)
        {
            var IsExist = await _courseRepository.CourseExistsAsync(id);
            if (!IsExist)

                return Response<bool>.Fail(
                            ErrorType.NotFound,
                            new ErrorDetail("COURSE_NOT_FOUND", "course not found", $"Course with ID {id} not found", "CourseId"));

            var hasEnrollments = await _courseRepository.CourseHasActiveEnrollmentsAsync(id);
            if (hasEnrollments)

                return Response<bool>.Fail(ErrorType.BusinessRule,
                    new ErrorDetail("COURSE_HAS_ENROLLMENTS", "Course has active enrollments", $"Cannot delete course because it has active enrollments")
                );


            var hasExams = await _courseRepository.CourseHasExamsAsync(id);
            if (hasExams)

                return Response<bool>.Fail(ErrorType.BusinessRule,
                    new ErrorDetail("COURSE_HAS_EXAMS", "Course has exams", $"Cannot delete course because it has associated exams")
                );

            await _courseRepository.DeleteAsync(id);
            await _courseRepository.SaveChangesAsync();

            return Response<bool>.Success(true);

        }

        public Task<PagedResponse<CourseDto>> GetPaginatedCoursesAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<Response<CourseDto>> UpdateCourseAsync(int courseId, CourseDto entity)
        {
            try
            {
                var course = await _courseRepository.GetByIdAsync(courseId);
                if (course == null)
                {
                    return Response<bool>.Fail(
                        ErrorType.NotFound,
                        new ErrorDetail("COURSE_NOT_FOUND", "Course not found", $"Course with ID {courseId} not found")
                    );
                }

                // Check ownership
                if (course.InstructorId != instructorId)
                {
                    return Response<bool>.Fail(
                        ErrorType.Forbidden,
                        new ErrorDetail("NOT_COURSE_OWNER", "Not the course owner", $"User {instructorId} is not the owner of course {courseId}")
                    );
                }

                // Toggle course status
                course.IsActive = !course.IsActive;
                course.UpdatedAt = DateTime.UtcNow;

                await _courseRepo.UpdateAsync(course);
                await _courseRepo.SaveChangesAsync();

                return Response<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail(
                    ErrorType.Critical,
                    new ErrorDetail("COURSE_STATUS_ERROR", "Failed to toggle course status", ex.Message)
                );
            }
        }
    }
}
    

