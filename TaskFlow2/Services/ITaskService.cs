
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskFlow.Models;
using TaskFlow2.Models.DTOs;


namespace TaskFlow2.Services
{
    public interface ITaskService
    {
        Task<List<Models.Task>> GetAllAsync();
        Task<Models.Task?> GetByIdAsync(int id);
        Task<Models.Task> CreateAsync(TaskCreateDto dto);
        Task<Models.Task?> UpdateAsync(int id, TaskUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
