using ExaminantionSystem.Entities.Dtos.Courcse;
using ExaminantionSystem.Entities.Wrappers;

namespace ExaminantionSystem.Service
{
    public interface ICourceService
    {
        Task<PagedResponse<CourseDTO>> GetPagedAsync(int pageNumber, int pageSize);
        Task<Response<CourseDTO>> GetByIdAsync(int id);
        Task<Response<CourseDTO>> CreateAsync(CourseDTO entity, int instructorId);
        Task<Response<CourseDTO>> UpdateAsync(int id, CourseDTO entity);
        Task<Response<string>> DeleteAsync(int id);
    }
    public class CourceService
    {
    }
}
