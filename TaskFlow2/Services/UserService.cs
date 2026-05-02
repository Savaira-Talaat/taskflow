using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TaskFlow.Data;
using TaskFlow.Models;
using TaskFlow2.Helpers;
using TaskFlow2.Models.DTOs;

namespace TaskFlow2.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public UserService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<AuthResponseDto?> AuthenticateAsync(UserLoginDto dto)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return null;

            if (!PasswordHasher.Verify(dto.Password, user.PasswordHash))
                return null;

            var token = JwtHelper.GenerateToken(_config, user.Id.ToString(), user.Email, user.Role.ToString());
            var expiresAt = DateTime.UtcNow.AddMinutes(int.TryParse(_config["Jwt:DurationMinutes"], out var m) ? m : 60);

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        public async Task<User> RegisterAsync(UserRegisterDto dto)
        {
            var exists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
            if (exists)
                throw new ArgumentException("Un utilisateur avec cet email existe déjà.");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = PasswordHasher.Hash(dto.Password),
                Role = User.UserRole.User
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return user;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _db.Users.SingleOrDefaultAsync(u => u.Id == id);
        }
    }
}