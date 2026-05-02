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

        /// <summary>Liste tous les projets de l'utilisateur connecté.</summary>
        /// <response code="200">Liste retournée avec succès</response>
        /// <response code="401">Non authentifie</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();
            var projects = await _projectService.GetAllAsync(userId);
            return Ok(projects);
        }

        /// <summary>Crée un nouveau projet.</summary>
        /// <response code="201">Projet crée avec succès</response>
        /// <response code="400">Données invalides</response>
        /// <response code="401">Non authentifié</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody] ProjectCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var created = await _projectService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>Récupère un projet par son ID.</summary>
        /// <response code="200">Projet trouvé</response>
        /// <response code="401">Non authentifié</response>
        /// <response code="404">Projet introuvable</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var project = await _projectService.GetByIdAsync(id, userId);
            if (project == null) return NotFound(new { message = $"Projet {id} introuvable." });

            return Ok(project);
        }

        /// <summary>Met à jour un projet existant.</summary>
        /// <response code="200">Projet mis à jour</response>
        /// <response code="400">Données invalides</response>
        /// <response code="401">Non authentifié</response>
        /// <response code="404">Projet introuvable ou accès refusé</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] ProjectUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var updated = await _projectService.UpdateAsync(id, dto, userId);
            if (updated == null) return NotFound(new { message = $"Projet {id} introuvable ou accès refusé." });

            return Ok(updated);
        }

        /// <summary>Supprime un projet (uniquement pour le propriétaire).</summary>
        /// <response code="204">Projet supprimé</response>
        /// <response code="401">Non authentifié</response>
        /// <response code="404">Projet introuvable ou accès refusé</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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