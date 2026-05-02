using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Models;
using TaskFlow2.Models.DTOs;

namespace TaskFlow2.Services
{
    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _db;

        public ProjectService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ProjectDto>> GetAllAsync(int userId)
        {
            return await _db.Projects
                .Where(p => p.UserId == userId)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreationDate = p.CreationDate,
                    UserId = p.UserId
                })
                .ToListAsync();
        }

        public async Task<ProjectDto?> GetByIdAsync(int id, int userId)
        {
            var p = await _db.Projects.SingleOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (p == null) return null;

            return new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreationDate = p.CreationDate,
                UserId = p.UserId
            };
        }

        public async Task<ProjectDto> CreateAsync(ProjectCreateDto dto, int userId)
        {
            var project = new Project
            {
                Name = dto.Name,
                Description = dto.Description,
                UserId = userId,
                CreationDate = DateTime.UtcNow
            };

            _db.Projects.Add(project);
            await _db.SaveChangesAsync();

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreationDate = project.CreationDate,
                UserId = project.UserId
            };
        }

        public async Task<ProjectDto?> UpdateAsync(int id, ProjectUpdateDto dto, int userId)
        {
            var project = await _db.Projects.SingleOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (project == null) return null;

            project.Name = dto.Name;
            project.Description = dto.Description;
            _db.Projects.Update(project);
            await _db.SaveChangesAsync();

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreationDate = project.CreationDate,
                UserId = project.UserId
            };
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var project = await _db.Projects.Include(p => p.Tasks).SingleOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (project == null) return false;

            _db.Projects.Remove(project);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}