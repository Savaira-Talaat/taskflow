using System.Threading.Tasks;
using TaskFlow.Models;
using TaskFlow2.Models.DTOs;

namespace TaskFlow2.Services
{
    public interface IUserService
    {
        Task<AuthResponseDto?> AuthenticateAsync(UserLoginDto dto);
        Task<User> RegisterAsync(UserRegisterDto dto);
        Task<User?> GetByIdAsync(int id);
    }
}