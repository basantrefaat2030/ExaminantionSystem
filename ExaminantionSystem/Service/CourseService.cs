using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExaminantionSystem.Entities.Dtos.Choice;
using ExaminantionSystem.Entities.Dtos.Course;
using ExaminantionSystem.Entities.Dtos.Ouestion;
using ExaminantionSystem.Entities.Enums.Errors;
using ExaminantionSystem.Entities.Models;
using ExaminantionSystem.Entities.Wrappers;
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
        //private readonly StudentRepository _studentRepository;
        private readonly ExamRepository _examRepository;
        private readonly QuestionRepository _questionRepository;
        private readonly ChoiceRepository _choiceRepository;
        private readonly IMapper _mapper;

        public CourseService(
          CourseRepository courseRepository,
          InstructorRepository instructorRepository,
          ExamRepository examRepository,
          QuestionRepository questionRepository,
          ChoiceRepository choiceRepository,
          IMapper mapper)
        {
            _courseRepository = courseRepository;
            _instructorRepository = instructorRepository;
            _examRepository = examRepository;
            _questionRepository = questionRepository;
            _choiceRepository = choiceRepository;
            _mapper = mapper;
        }

        #region CourseCRUD
        public async Task<Response<CourseDto>> CreateCourseAsync(CreateCourseDto coursedto)
        {

            // Validate instructor exists
            var instructor = await _instructorRepository.GetByIdAsync(coursedto.InstructorId);
                if (instructor == null)
                    return Response<CourseDto>.Fail(
                        ErrorType.INSTRUCTOR_NOT_FOUND,
                        new ErrorDetail("Instructor not found", $"Instructor with ID {coursedto.InstructorId} not found", "instructorId")
                    );
                

                // Check for duplicate course title
                var titleExists = await _courseRepository.CourseTitleExistsAsync(coursedto.Title, coursedto.InstructorId);
                if (titleExists)
                    return Response<CourseDto>.Fail(
                        ErrorType.COURSE_TITLE_EXISTS,
                        new ErrorDetail("Course title exists", $"Course with title '{coursedto.Title}' already exists", "title")
                    );


                var course = _mapper.Map<Course>(coursedto);
                course.CreatedBy = coursedto.InstructorId;

                await _courseRepository.AddAsync(course);
                await _courseRepository.SaveChangesAsync();

                var newCourse = _mapper.Map<CourseDto>(course);

                return Response<CourseDto>.Success(newCourse);
            

        }

        public async Task<Response<bool>> DeleteCourseAsync(int courseId , int currentUserId)
        {
            var courseInfo = await _courseRepository.GetWithTrancking(c => c.Id == courseId)
                 .Select(c => new
                 {
                     Course = c,
                     HasExams = c.Exams.Any( e => e.IsActive && !e.IsDeleted && e.StartDate <= DateTime.Now && e.EndDate >= DateTime.Now),
                     IsOwner = c.InstructorId == currentUserId
                 }).FirstOrDefaultAsync();

            if (courseInfo == null)
            
                return Response<bool>.Fail(
                    ErrorType.COURSE_NOT_FOUND, new ErrorDetail("Course not found", $"Course with ID {courseId} not found"));


            if (!courseInfo.IsOwner)
                return Response<bool>.Fail(ErrorType.NOT_COURSE_OWNER,
                    new ErrorDetail("canot delete this course beacuse this course donot own this instructor"));

            var hasEnrollments = await _courseRepository.CourseHasActiveEnrollmentsAsync(courseId);
            if (hasEnrollments)

                return Response<bool>.Fail(ErrorType.COURSE_HAS_ENROLLMENTS, 
                    new ErrorDetail("Course has active enrollments", "Cannot delete course because it has active enrollments"));


            if (courseInfo.HasExams)

                return Response<bool>.Fail(ErrorType.COURSE_HAS_EXAMS, 
                    new ErrorDetail("Course has exams", $"Cannot delete course because it has associated exams"));

            await _courseRepository.DeleteAsync(courseId);
            await _courseRepository.SaveChangesAsync();

            return Response<bool>.Success(true);

        }

        public async Task<Response<CourseDto>> UpdateCourseAsync(UpdateCourseDto dto, int currentUserId)
        {
                var course = await _courseRepository.GetByIdTrackingAsync(dto.courseId);
                if (course == null)
                {
                    return Response<CourseDto>.Fail(ErrorType.COURSE_NOT_FOUND,
                        new ErrorDetail($"Course with ID {dto.courseId} not found")
                    );
                }

                // Check for duplicate title (excluding current course)
                var titleExists = await _courseRepository.CourseTitleExistsAsync(dto.Title, currentUserId);
                if (titleExists)
                {
                    return Response<CourseDto>.Fail(
                        ErrorType.COURSE_TITLE_EXISTS,
                        new ErrorDetail("Course title exists", $"Course with title '{dto.Title}' already exists", "title")
                    );
                }


            if (course.InstructorId != currentUserId)
            {
                return Response<CourseDto>.Fail(
                    ErrorType.NOT_COURSE_OWNER,
                    new ErrorDetail("Not the course owner", $"User {currentUserId} is not the owner of course {dto.Title}")
                );
            }
                _mapper.Map(dto, course);
                course.UpdatedAt = DateTime.UtcNow;

                await _courseRepository.UpdateAsync(course);
                await _courseRepository.SaveChangesAsync();

                var updatedCourse = await _courseRepository.GetByIdAsync(course.Id);
                var courseDto = _mapper.Map<CourseDto>(updatedCourse);

                return Response<CourseDto>.Success(courseDto);
            }
        

        public async Task<Response<PagedResponse<CourseInformationDto>>> GetPaginatedCoursesAsync(int? currentUserId, int pageNumber, int pageSize)
        {

            if (currentUserId.HasValue)
            {
                var instructor = await _instructorRepository.GetByIdAsync(currentUserId.Value);
                if (instructor == null)
                    return Response<PagedResponse<CourseInformationDto>>.Fail(
                        ErrorType.INSTRUCTOR_NOT_FOUND,
                        new ErrorDetail("Instructor not found", $"Instructor with ID {currentUserId} not found")
                    );
                
            }

            var query = _courseRepository.GetAll(c => c.InstructorId == currentUserId);

            //If pageNumber = 1 → Skip(0) → start from first record
            //If pageNumber = 2 → Skip(pageSize) → skip first 10, take next 10
            //If pageNumber = 3 → Skip(2 * pageSize) → skip first 20, take next 10
            var totalRecords = await query.CountAsync();
            var courses = await query
                .OrderBy(c => c.Title)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var courseDtos = _mapper.Map<List<CourseInformationDto>>(courses);

            var pagedResponse = new PagedResponse<CourseInformationDto>(courseDtos, pageNumber, pageSize, totalRecords);
            return Response<PagedResponse<CourseInformationDto>>.Success(pagedResponse);
        }


        public async Task<Response<CourseDetailsDto>> GetCourseDetailsAsync(int courseId)
        {
            var courseDetailsInfo = await _courseRepository.GetAll(c => c.Id == courseId)
            .Select(c => new CourseDetailsDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                InstructorId = c.InstructorId,
                InstructorName = c.Instructor.User.FullName
            }).FirstOrDefaultAsync();

         
                if (courseDetailsInfo == null)
                    return Response<CourseDetailsDto>.Fail(
                        ErrorType.COURSE_NOT_FOUND,new ErrorDetail( $"Course with ID {courseId} not found"));
                

                var totalEnrollments = await _courseRepository.GetTotalEnrollmentsAsync(courseId);
                var totalExams = await _examRepository.GetTotalExam(courseId);

                courseDetailsInfo.TotalEnrollments = totalEnrollments;
                courseDetailsInfo.TotalExams = totalExams;

                return Response<CourseDetailsDto>.Success(courseDetailsInfo);

        }
        #endregion

        #region QuestionsRelatedCourse
        public async Task<Response<List<QuestionPoolDto>>> GetQuestionPoolByCourseAsync(int courseId, int currentUserId)
        {
            
            var courseInfo = await _courseRepository.GetAll(c => c.Id == courseId).Select(c => new { IsAuthorized = c.InstructorId != currentUserId , Questions = c.Questions.Where(q => q.IsActive && !q.IsDeleted)}).FirstOrDefaultAsync();
            if (courseInfo == null)
                return Response<List<QuestionPoolDto>>.Fail(ErrorType.COURSE_NOT_FOUND,
                    new ErrorDetail("Course not found"));

            if (!courseInfo.IsAuthorized)
                return Response<List<QuestionPoolDto>>.Fail(ErrorType.NOT_COURSE_OWNER,
                    new ErrorDetail("You can only access questions from your own courses"));


            //var questionIds = questions.Select(q => q.Id).ToList();

            //// Get all choices for these questions
            //var choices = await _choiceRepository.GetAll()
            //    .Where(c => questionIds.Contains(c.QuestionId) && !c.IsDeleted)
            //    .ToListAsync();

            var questionPool = _mapper.Map<List<QuestionPoolDto>>(courseInfo.Questions);

            return Response<List<QuestionPoolDto>>.Success(questionPool);

        }
        #endregion
    }
}

    

