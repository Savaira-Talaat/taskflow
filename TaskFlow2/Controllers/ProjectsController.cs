using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow2.Models.DTOs;
using TaskFlow2.Services;

namespace TaskFlow2.Controllers
{
    [ApiController]
    [Route("api/projects")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(idClaim, out var id) ? id : 0;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();
            var projects = await _projectService.GetAllAsync(userId);
            return Ok(projects);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProjectCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var created = await _projectService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var project = await _projectService.GetByIdAsync(id, userId);
            if (project == null) return NotFound(new { message = $"Projet {id} introuvable." });

            return Ok(project);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProjectUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var updated = await _projectService.UpdateAsync(id, dto, userId);
            if (updated == null) return NotFound(new { message = $"Projet {id} introuvable ou accès refusé." });

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var deleted = await _projectService.DeleteAsync(id, userId);
            if (!deleted) return NotFound(new { message = $"Projet {id} introuvable ou accès refusé." });

            return NoContent();
        }
    }
}