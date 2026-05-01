using Learnova.Application.Authentication.Command.Register;
using Learnova.Application.DTOS.RegisterDto;
using Learnova.Application.IRepository;
using Learnova.Application.Settings;
using Learnova.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Learnova.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtSettings _jwt;

    public AuthRepository(
        UserManager<ApplicationUser> userManager,
        IOptions<JwtSettings> jwtOptions)
    {
        _userManager = userManager;
        _jwt = jwtOptions.Value;
    }
    public async Task<AuthModel> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var authModel = new AuthModel();

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, password))
        {
            authModel.Messege = "Email or password is incorrect";
            return authModel;
        }

        var jwtSecurityToken = await CreateJwtToken(user);
        

        var refreshToken = GenerateRefreshToken();
        user.RefreshTokens.Add(refreshToken);
        await _userManager.UpdateAsync(user);

        authModel.IsAuthenticated = true;
        authModel.Email = user.Email;
        authModel.Username = user.UserName;
        authModel.Token = stringToken;
        authModel.RefreshToken = refreshToken.Token;
        authModel.RefreshTokenExpireOn = refreshToken.ExpireOn;

        var roles = await _userManager.GetRolesAsync(user);
        authModel.Role = roles.ToList();

        return authModel;
    }

    public async Task<AuthModel> RefreshTokenAsync(string Token, CancellationToken cancellationToken = default)
    {
        var authModel = new AuthModel();

        var user = await _userManager.Users
            .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == Token), cancellationToken);

        if (user == null)
        {
            authModel.Messege = "Invalid token";
            return authModel;
        }

        var refreshToken = user.RefreshTokens.Single(t => t.Token == Token);

        if (!refreshToken.IsActive)
        {
            authModel.Messege = "Invalid token";
            return authModel;
        }

        refreshToken.RevokedOn = DateTime.UtcNow;

        var newRefreshToken =await GenerateRefreshToken();
        user.RefreshTokens.Add(newRefreshToken);
        await _userManager.UpdateAsync(user);

        var stringjwtToken = await CreateJwtTokenAsync(user);

        authModel.IsAuthenticated = true;
        authModel.RefreshToken = newRefreshToken.Token;
        authModel.Token = stringjwtToken;
        authModel.Email = user.Email;
        authModel.Username = user.Email;
        var roles = await _userManager.GetRolesAsync(user);
        authModel.Role = roles.ToList();
        authModel.RefreshTokenExpireOn = newRefreshToken.ExpireOn;

        return authModel;
    }

    public async Task<bool> RevokeTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users
            .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token), cancellationToken);

        if (user == null)
            return false;

        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

        if (!refreshToken.IsActive)
            return false;

        refreshToken.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);
        return true;
    }

    // ===== Helpers =====
    private async Task<string> CreateJwtTokenAsync(ApplicationUser applicationUser)
    {
        var userClaims = await _userManager.GetClaimsAsync(applicationUser);
        var roles = await _userManager.GetRolesAsync(applicationUser);
        var roleClaims = new List<Claim>();

        foreach (var role in roles)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email),
            new Claim(ClaimTypes.NameIdentifier, applicationUser.Id),
        }
        .Union(roleClaims)
        .Union(userClaims);

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwt.Key));
        var singCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwtToken = new JwtSecurityToken(
        issuer: _jwt.Issuer,
        audience: _jwt.Audience,
        claims: claims,
        expires: DateTime.UtcNow.AddDays(_jwt.DurationInDays),
        signingCredentials: singCred);

        var stringToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        return stringToken;
    }

    public async Task<RefreshToken> GenerateRefreshToken()
    {
        var randomNumber = new byte[32];

        using var generator = new RNGCryptoServiceProvider();
        generator.GetBytes(randomNumber);

        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomNumber),
            ExpireOn = DateTime.UtcNow.AddDays(10),
            CreatedOn = DateTime.UtcNow
        };
    }
}