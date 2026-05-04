using Learnova.Application.Command.Authentication.Register;
using Learnova.Application.DTOS.RegisterDto;
using Learnova.Domain.Enums;
using Learnova.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Learnova.Application.IRepository;

public interface IAuthRepository
{
    
    Task<IList<Claim>> GetClaimsAsync(ApplicationUser user);
    Task AddToRoleAsync(ApplicationUser user,string Role);
    Task<IList<string>> GetRolesAsync(ApplicationUser user);

    Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);

    Task<bool> ResetPasswordAsync(ApplicationUser user, string decodedToken, string NewPassword);

    Task CancelTokenAsync(CancellationToken cancellationToken);

}