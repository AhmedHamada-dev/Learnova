using Learnova.Application.Authentication.Command.Register;
using Learnova.Application.DTOS.RegisterDto;
using Learnova.Domain.Enums;
using Learnova.Domain.Identity;
using System.Security.Claims;

namespace Learnova.Application.IRepository;

public interface IAuthRepository
{
    
    Task<IList<Claim>> GetClaimsAsync(ApplicationUser user);
    Task AddToRoleAsync(ApplicationUser user,string Role);
    Task<IList<string>> GetRolesAsync(ApplicationUser user);
 
}