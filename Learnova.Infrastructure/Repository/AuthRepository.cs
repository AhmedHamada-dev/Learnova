using Learnova.Application.Command.Authentication.Register;
using Learnova.Application.DTOS.RegisterDto;
using Learnova.Application.IRepository;
using Learnova.Application.Settings;
using Learnova.Domain.Enums;
using Learnova.Domain.Identity;
using Learnova.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Learnova.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtSettings _jwt;
    private readonly LearnovaDbContext _context;

    public AuthRepository(
        UserManager<ApplicationUser> userManager,
        IOptions<JwtSettings> jwtOptions,
        LearnovaDbContext context)  
    {
        _userManager = userManager;
        _jwt = jwtOptions.Value;
        _context = context;
    }
    public async Task<IList<Claim>> GetClaimsAsync(ApplicationUser user)
    {
       return await _userManager.GetClaimsAsync(user);
    }
    public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
    {
       return await _userManager.GetRolesAsync(user);
    }

    public async Task AddToRoleAsync(ApplicationUser user, string Role)
    {
        await _userManager.AddToRoleAsync(user, Role);
    }

    public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
    {
       return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<bool> ResetPasswordAsync(ApplicationUser user,string decodedToken,string NewPassword)
    {
        var result = await _userManager.ResetPasswordAsync(
                user,
                decodedToken,
                NewPassword
            );
        return result.Succeeded;
    }

    public async Task CancelTokenAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}