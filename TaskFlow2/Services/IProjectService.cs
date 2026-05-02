using System.Collections.Generic;
using System.Threading.Tasks;
using TaskFlow2.Models.DTOs;

namespace TaskFlow2.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetAllAsync(int userId);
        Task<ProjectDto?> GetByIdAsync(int id, int userId);
        Task<ProjectDto> CreateAsync(ProjectCreateDto dto, int userId);
        Task<ProjectDto?> UpdateAsync(int id, ProjectUpdateDto dto, int userId);
        Task<bool> DeleteAsync(int id, int userId);
    }
}