using SalonApp.Application.DTOs;
using System.Threading.Tasks;

namespace SalonApp.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
    Task<UserDto> GetCurrentUserAsync(int userId);
}
