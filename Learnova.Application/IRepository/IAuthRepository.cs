using Learnova.Application.Authentication.Command.Register;
using Learnova.Application.DTOS.RegisterDto;
using Learnova.Domain.Identity;

namespace Learnova.Application.IRepository;

public interface IAuthRepository
{
    Task<AuthModel> RegisterAsync(RegisterCommand command, CancellationToken cancellationToken = default);
    Task<RefreshToken> GenerateRefreshToken();
    Task<string> CreateJwtTokenAsync(ApplicationUser applicationUser);
    Task<AuthModel> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<AuthModel> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}