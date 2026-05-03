using Learnova.Application.Authentication.Command.Login;
using Learnova.Application.Authentication.Command.Register;
using Learnova.Application.DTOS.RegisterDto;
using Learnova.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.IServices
{
    public interface IAuthService
    {
        Task<AuthModel> StudentRegister(RegisterCommand request, CancellationToken cancellationToken);
        Task<AuthModel> Login(LoginCommand request, CancellationToken cancellationToken);
        Task<String> CreateJwtTokenAsync(ApplicationUser user);
        Task<AuthModel> RefreshTokenAsync(string token);
        Task<bool> RevokeTokenAsync(string token);

       // Task ForgetPasswordAsync(string Email);

       // Task<bool> ResetPasswordAsync(ResetPasswordDTO dto);

       // Task<AuthModel> ChangePasswordAsync(string userId, ChangePasswordDto dto);

       // Task<AuthModel> LoginWithGoogleAsync(string email);
    }
}
