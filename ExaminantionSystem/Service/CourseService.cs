using ExaminantionSystem.Entities.Dtos;
using ExaminantionSystem.Entities.Dtos.Courcse;
using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Entities.Wrappers;
using ExaminantionSystem.Infrastructure;
using ExaminantionSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

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
          ExamRepository examRepository)
        {
            _courseRepository = courseRepository;
            _instructorRepository = instructorRepository;
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
        public async Task<Response<CourseDto>> UpdateCourseAsync(UpdateCourseDto dto , int courseId)
        {
            try
            {
                var course = await _courseRepository.GetByIdTrackingAsync(courseId);
                if (course == null)
                {
                    return Response<CourseDto>.Fail(
                        ErrorType.NotFound,
                        new ErrorDetail("COURSE_NOT_FOUND", "Course not found", $"Course with ID {courseId} not found")
                    );
                }

                // Check for duplicate title (excluding current course)
                var titleExists = await _courseRepository.CourseTitleExistsAsync(dto.Title, course.InstructorId);
                if (titleExists)
                {
                    return Response<CourseDto>.Fail(
                        ErrorType.Conflict,
                        new ErrorDetail("COURSE_TITLE_EXISTS", "Course title exists", $"Course with title '{dto.Title}' already exists", "title")
                    );
                }


                //if (course.InstructorId != entity.InstructorId)
                //{
                //    return Response<CourseDto>.Fail(
                //        ErrorType.Forbidden,
                //        new ErrorDetail("NOT_COURSE_OWNER", "Not the course owner", $"User {instrcutorId} is not the owner of course {courseId}")
                //    );
                //}
                // Update course properties
                course.Title = dto.Title;
                course.Description = dto.Description;
                course.Hours = dto.Hours;
                course.Budget = dto.Budget;
                course.InstructorId = dto.InstructorId;
             

                await _courseRepository.UpdateAsync(course);
                await _courseRepository.SaveChangesAsync();

                var updatedCourse = await _courseRepository.GetByIdAsync(course.Id);

                var courseDto = new CourseDto
                {
                    Id = updatedCourse.Id,
                    Title = updatedCourse.Title,
                    Description = updatedCourse.Description,
                    Hours = updatedCourse.Hours.Value,
                    Budget = updatedCourse.Budget,
                    InstructorId = updatedCourse.InstructorId,

                };

                return Response<CourseDto>.Success(courseDto);
            }
            catch (Exception ex)
            {
                return Response<CourseDto>.Fail(
                    ErrorType.Critical,
                    new ErrorDetail("COURSE_STATUS_ERROR", "Failed to toggle course status", ex.Message)
                );
            }
        }


        public async Task<Response<PagedResponse<CourseInformationDto>>> GetPaginatedCoursesAsync(int? instructorId, int pageNumber, int pageSize)
        {

            if (instructorId.HasValue)
            {
                var instructor = await _instructorRepository.GetByIdAsync(instructorId.Value);
                if (instructor == null)
                {
                    return Response<PagedResponse<CourseInformationDto>>.Fail(
                        ErrorType.NotFound,
                        new ErrorDetail("INSTRUCTOR_NOT_FOUND", "Instructor not found", $"Instructor with ID {instructorId} not found")
                    );
                }
            }

            var query = _courseRepository.GetAll(
                c => c.InstructorId == instructorId && !c.IsDeleted);

            //If pageNumber = 1 → Skip(0) → start from first record
            //If pageNumber = 2 → Skip(pageSize) → skip first 10, take next 10
            //If pageNumber = 3 → Skip(2 * pageSize) → skip first 20, take next 10
            var totalRecords = await query.CountAsync();
            var courses = await query
                .OrderBy(c => c.Title)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var courseDtos = courses.Select(c => new CourseInformationDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Hours = c.Hours.Value,
                Budget = c.Budget,
                InstructorId = c.InstructorId,
                InstructorName = c.Instructor?.FullName,
                IsActive = c.IsActive,

            }).ToList();

            var pagedResponse = new PagedResponse<CourseInformationDto>(courseDtos, pageNumber, pageSize, totalRecords);
            return Response<PagedResponse<CourseInformationDto>>.Success(pagedResponse);
        }


        public async Task<Response<CourseDetailsDto>> GetCourseDetailsAsync(int courseId)
        {
            try
            {
                var course = await _courseRepository.GetByIdAsync(courseId);
                if (course == null)
                {
                    return Response<CourseDetailsDto>.Fail(
                        ErrorType.NotFound,
                        new ErrorDetail("COURSE_NOT_FOUND", "Course not found", $"Course with ID {courseId} not found")
                    );
                }

                // Get enrollment statistics
                var totalEnrollments = await _courseRepository.GetTotalEnrollments(courseId);
                var activeEnrollments = await _courseRepository.GetTotalEnrollments(courseId);

                // Get exam statistics
                var totalExams = await _examRepository.GetTotalExam(courseId);

                var courseDetails = new CourseDetailsDto
                {
                    Id = course.Id,
                    Title = course.Title,
                    Description = course.Description,
                    Hours = course.Hours.Value,
                    Budget = course.Budget,
                    InstructorId = course.InstructorId,
                    InstructorName = course.Instructor?.FullName,
                    IsActive = course.IsActive,
                    TotalEnrollments = totalEnrollments,
                    ActiveEnrollments = activeEnrollments,
                    TotalExams = totalExams,
                };

                return Response<CourseDetailsDto>.Success(courseDetails);
            }
            catch (Exception ex)
            {
                return Response<CourseDetailsDto>.Fail(
                    ErrorType.Critical,
                    new ErrorDetail("COURSE_DETAILS_ERROR", "Failed to fetch course details", ex.Message)
                );
            }
        }
    }
}
    

