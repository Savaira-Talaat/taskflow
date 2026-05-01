using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskFlow2.Models.DTOs;
using TaskFlow2.Services;

namespace TaskFlow2.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    // TODO: [Authorize] faudra l'activer quand le JWT sera pret
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        /// <summary>Liste toutes les tâches.</summary>
        /// <response code="200">Liste retournée avec succès</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var tasks = await _taskService.GetAllAsync();
            return Ok(tasks);
        }

        /// <summary>Récupère une tâche par son ID.</summary>
        /// <response code="200">Tâche trouvée</response>
        /// <response code="404">Tâche introuvable</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var task = await _taskService.GetByIdAsync(id);
            if (task == null)
                return NotFound(new { message = $"Tâche {id} introuvable." });

            return Ok(task);
        }

        /// <summary>Crée une nouvelle tâche rattachée à un projet.</summary>
        /// <response code="201">Tâche créée avec succès</response>
        /// <response code="400">Données invalides</response>
        /// <response code="404">Projet associé introuvable</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] TaskCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var task = await _taskService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }

        /// <summary>Met à jour une tâche existante.</summary>
        /// <response code="200">Tâche mise à jour</response>
        /// <response code="400">Données invalides</response>
        /// <response code="404">Tâche introuvable</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] TaskUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var task = await _taskService.UpdateAsync(id, dto);
            if (task == null)
                return NotFound(new { message = $"Tâche {id} introuvable." });

            return Ok(task);
        }

        /// <summary>Supprime une tâche.</summary>
        /// <response code="204">Tâche supprimée</response>
        /// <response code="404">Tâche introuvable</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _taskService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Tâche {id} introuvable." });

            return NoContent();
        }
    }
}