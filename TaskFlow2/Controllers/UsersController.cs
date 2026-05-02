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

        [HttpPost("register")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
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

        [HttpPost("login")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var auth = await _userService.AuthenticateAsync(dto);
            if (auth == null)
                return Unauthorized(new { message = "Email ou mot de passe invalide." });

            return Ok(auth);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { message = $"Utilisateur {id} introuvable." });

            return Ok(new { user.Id, user.Name, user.Email, Role = user.Role.ToString() });
        }
    }
}