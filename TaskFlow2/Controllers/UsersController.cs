using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaskFlow2.Models.DTOs;
using TaskFlow2.Services;

namespace TaskFlow2.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>Inscription d'un nouvel utilisateur.</summary>
        /// <response code="201">Utilisateur créé avec succès</response>
        /// <response code="400">Données invalides ou email déjà utilisé</response>
        [HttpPost("register")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _userService.RegisterAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, new { user.Id, user.Name, user.Email, Role = user.Role.ToString() });
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Connexion et obtention d'un token JWT.</summary>
        /// <response code="200">Connexion réussie, token retourné</response>
        /// <response code="400">Données invalides</response>
        /// <response code="401">Email ou mot de passe incorrect</response>
        [HttpPost("login")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var auth = await _userService.AuthenticateAsync(dto);
            if (auth == null)
                return Unauthorized(new { message = "Email ou mot de passe invalide." });

            return Ok(auth);
        }

        /// <summary>Récupère un utilisateur par son ID.</summary>
        /// <response code="200">Utilisateur trouvé</response>
        /// <response code="404">Utilisateur introuvable</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { message = $"Utilisateur {id} introuvable." });

            return Ok(new { user.Id, user.Name, user.Email, Role = user.Role.ToString() });
        }
    }
}