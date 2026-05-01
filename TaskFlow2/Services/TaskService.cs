using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow2.Models.DTOs;

namespace TaskFlow2.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;

        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Models.Task>> GetAllAsync()
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .ToListAsync();
        }

        public async Task<Models.Task?> GetByIdAsync(int id)
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Models.Task> CreateAsync(TaskCreateDto dto)
        {
            var projectExists = await _context.Projects.AnyAsync(p => p.Id == dto.ProjectId);
            if (!projectExists)
                throw new KeyNotFoundException($"Le projet avec l'ID {dto.ProjectId} n'existe pas.");

            var task = new Models.Task
            {
                Title = dto.Title,
                ProjectId = dto.ProjectId,
                DueDate = dto.DueDate,
                Status = Models.Task.TaskStatus.AFaire
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<Models.Task?> UpdateAsync(int id, TaskUpdateDto dto)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return null;

            task.Title = dto.Title;
            task.Status = (Models.Task.TaskStatus)dto.Status;
            task.DueDate = dto.DueDate;
            task.Comments = dto.Comments;

            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}