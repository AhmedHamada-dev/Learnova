using Learnova.Application.Command.Authentication.ForgetPassword;
using Learnova.Application.Command.Authentication.Login;
using Learnova.Application.Command.Authentication.RefreshToken;
using Learnova.Application.Command.Authentication.Register;
using Learnova.Application.Command.Authentication.ResetPassword;
using Learnova.Application.Command.Authentication.RevokeToken;
using Learnova.Application.Command.Authentication.VerifyEmail;
using Learnova.Application.DTOS.RegisterDto;
using Learnova.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Learnova.Application.IServices
{
    public interface IAuthService
    {
        Task<AuthModel> StudentRegister(RegisterCommand request, CancellationToken cancellationToken);
        Task<AuthModel> Login(LoginCommand request, CancellationToken cancellationToken);
        Task<String> CreateJwtTokenAsync(ApplicationUser user);
        Task<AuthModel> RefreshTokenAsync(RefreshTokenCommand command);
        Task<bool> RevokeTokenAsync(RevokeTokenCommand command);

        Task<bool> ResetPasswordAsync(ResetPasswordCommand resetPasswordCommand);
        Task ForgetPasswordAsync(ForgetPasswordCommand command,CancellationToken cancellationToken);

        Task<AuthModel> VerifyEmailAsync(VerifyEmailCommand request, CancellationToken cancellationToken);
       // Task ForgetPasswordAsync(string Email);

        // Task<bool> ResetPasswordAsync(ResetPasswordDTO dto);

        // Task<AuthModel> ChangePasswordAsync(string userId, ChangePasswordDto dto);

        // Task<AuthModel> LoginWithGoogleAsync(string email);
    }
}
