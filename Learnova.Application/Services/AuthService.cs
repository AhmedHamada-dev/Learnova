using Learnova.Application.Command.Authentication.ForgetPassword;
using Learnova.Application.Command.Authentication.Login;
using Learnova.Application.Command.Authentication.RefreshToken;
using Learnova.Application.Command.Authentication.Register;
using Learnova.Application.Command.Authentication.ResendOTP;
using Learnova.Application.Command.Authentication.ResetPassword;
using Learnova.Application.Command.Authentication.RevokeToken;
using Learnova.Application.Command.Authentication.VerifyEmail;
using Learnova.Application.DTOS.RegisterDto;
using Learnova.Application.IRepository;
using Learnova.Application.IServices;
using Learnova.Application.Settings;
using Learnova.Domain.Entities;
using Learnova.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly JwtSettings _Jwt;
        private readonly IUserRepository _userRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IEmailService _emailService;
        private readonly IEmailVerificationRepository _emailVerificationRepository;
        private readonly ILogger<AuthService> _logger;
        public AuthService(IOptions<JwtSettings> jwtOptions, IUserRepository userRepository, IAuthRepository authRepository, IEmailService emailService, IEmailVerificationRepository emailVerificationRepository, ILogger<AuthService> logger)
        {
            _Jwt = jwtOptions.Value;
            _userRepository = userRepository;
            _authRepository = authRepository;
            _emailService = emailService;
            _emailVerificationRepository = emailVerificationRepository;
            _logger = logger;
        }
        public async Task<string> CreateJwtTokenAsync(ApplicationUser applicationUser)
        {
            var userClaims = await _authRepository.GetClaimsAsync(applicationUser);
            var roles = await _authRepository.GetRolesAsync(applicationUser);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claims = new List<Claim>
           {
            new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email),
            new Claim(ClaimTypes.NameIdentifier, applicationUser.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString())
            };

            claims.AddRange(roleClaims);
            claims.AddRange(userClaims);

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_Jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                issuer: _Jwt.Issuer,
                audience: _Jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_Jwt.DurationInMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
        public async Task<AuthModel> RefreshTokenAsync(RefreshTokenCommand command)
        {
            var authModel = new AuthModel();
            var user = await _userRepository.GetByRefreshTokenAsync(command.refreshtoken);

            if (user == null)
            {

                authModel.Messege = "Invalid Token";
                return authModel;
            }
            var refreshToken = user.RefreshTokens.Single(t => t.Token == command.refreshtoken);

            if (!refreshToken.IsActive)
            {
                authModel.Messege = "Invalid Token";
                return authModel;
            }
            refreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userRepository.UpdateAsync(user);

            var jwtToken = await CreateJwtTokenAsync(user);


            authModel.IsAuthenticated = true;
            authModel.RefreshToken = newRefreshToken.Token;
            authModel.Token = jwtToken;
            authModel.Email = user.Email;
            authModel.Username = user.Email;
            var Roles = await _authRepository.GetRolesAsync(user);
            authModel.Role = Roles.ToList();
            authModel.RefreshTokenExpireOn = newRefreshToken.ExpireOn;
            authModel.RefreshTokenExpireOn = newRefreshToken.ExpireOn;
            authModel.ExpireOn = DateTime.UtcNow.AddMinutes(_Jwt.DurationInMinutes);

            return authModel;
        }

        public async Task<AuthModel> StudentRegister(RegisterCommand request, CancellationToken cancellationToken)
        {

            if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
                return new AuthModel
                {
                    Messege = "Email already exists",
                    IsAuthenticated = false
                };

            var user = new ApplicationUser
            {
                FullName = request.FullName,
                Email = request.Email,
                UserName = request.Email,
                IsVerified=false,
            };


            await _userRepository.AddAsync(user, request.Password, cancellationToken);

            await _authRepository.AddToRoleAsync(user, "Student");

            var window = TimeSpan.FromMinutes(10);
            var maxCount = 3;

            if (!CanSendOtp(user, maxCount, window))
            {
                return new AuthModel
                {
                    IsAuthenticated = false,
                    Email = user.Email,
                    Messege = "Too many OTP requests. Please try again later."
                };
            }

            await _userRepository.UpdateAsync(user, cancellationToken);

            var code = GenerateOtpCode(6);

            var verification = new EmailVerification
            {
                UserId = user.Id,
                Code = code,
                ExpireAt = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false
            };

           await _emailVerificationRepository.AddEmailVerificationToUserAsync(verification,cancellationToken);
            var subject = "Email Verification Code";
            var body = $"Your verification code is: <b>{code}</b>";

            await _emailService.SendEmail(user.Email!, subject, body);

            return new AuthModel
            {
                IsAuthenticated = false,
                Email = user.Email,
                Messege = "Registration successful. Please verify your email using the code sent to you."
            };
        }

        public async Task<AuthModel> VerifyEmailAsync(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            var authModel = new AuthModel();

            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user is null)
            {

                _logger.LogWarning("VerifyEmail failed: invalid email {Email}", request.Email);
                authModel.IsAuthenticated = false;
                authModel.Messege = "Invalid email";
                return authModel;
            }


            var verification =await _emailVerificationRepository.GetEmailVerfication(user, request.Code, cancellationToken);

            if (verification is null)
            {

                _logger.LogWarning("VerifyEmail failed: invalid code for user {UserId}", user.Id);
                authModel.IsAuthenticated = false;
                authModel.Messege = "Invalid verification code";
                return authModel;
            }

            if (verification.IsUsed || verification.ExpireAt < DateTime.UtcNow)
            {

                _logger.LogWarning("VerifyEmail failed: expired/used code for user {UserId}", user.Id);
                authModel.IsAuthenticated = false;
                authModel.Messege = "Verification code has expired or already used";
                return authModel;
            }


            _logger.LogInformation("VerifyEmail success for user {UserId}", user.Id);


            verification.IsUsed = true;
            user.IsVerified = true;

            await _authRepository.CancelTokenAsync(cancellationToken);
            await _userRepository.UpdateAsync(user, cancellationToken);

           
            var token = await CreateJwtTokenAsync(user);
            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(refreshToken);
            await _userRepository.UpdateAsync(user, cancellationToken);

            var roles = await _authRepository.GetRolesAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Email = user.Email!;
            authModel.Username = user.Email;
            authModel.Role = roles.ToList();
            authModel.Token = token;
            authModel.RefreshToken = refreshToken.Token;
            authModel.RefreshTokenExpireOn = refreshToken.ExpireOn;
            authModel.ExpireOn = DateTime.UtcNow.AddMinutes(_Jwt.DurationInMinutes);
            authModel.Messege = "Email verified successfully";

            return authModel;
        }
        public async Task<AuthModel> Login(LoginCommand request, CancellationToken cancellationToken)
        {
            var authModel = new AuthModel();
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (user is null)
            {
                _logger.LogWarning("Login failed: user with email {Email} not found", request.Email);
                authModel.IsAuthenticated = false;
                authModel.Messege = "Invalid email or password";
                return authModel;
            }

            // لو الحساب مقفول بسبب محاولات فاشلة
            if (await _userRepository.IsLockedOutAsync(user))
            {
                authModel.IsAuthenticated = false;
                authModel.Messege = "Account is locked. Please try again later.";
                return authModel;
            }

            var passwordValid = await _userRepository.CheckPasswordAsync(user, request.Password);
            if (!passwordValid)
            {

                _logger.LogWarning("Login failed: invalid password for user {UserId}", user.Id);

                await _userRepository.AccessFailedAsync(user);

                authModel.IsAuthenticated = false;
                authModel.Messege = "Invalid email or password";
                return authModel;
            }

            // لو الباسورد صح، صفّر عداد الفشل
            await _userRepository.ResetAccessFailedCountAsync(user);

            if (!user.IsVerified)
            {
                _logger.LogInformation("Login blocked: user {UserId} email not verified", user.Id);

                authModel.IsAuthenticated = false;
                authModel.Messege = "Account is not verified yet";
                return authModel;
            }



            _logger.LogInformation("Login success for user {UserId}", user.Id);

            var stringToken = await CreateJwtTokenAsync(user);

            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(refreshToken);
            await _userRepository.UpdateAsync(user, cancellationToken);

            var roles = await _authRepository.GetRolesAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Email = user.Email!;
            authModel.Username = user.Email;
            authModel.Role = roles.ToList();
            authModel.Token = stringToken;
            authModel.RefreshToken = refreshToken.Token;
            authModel.RefreshTokenExpireOn = refreshToken.ExpireOn;
            authModel.ExpireOn = DateTime.UtcNow.AddMinutes(_Jwt.DurationInMinutes);
            authModel.Messege = "Login completed successfully";

            return authModel;
        }
        public async Task<bool> RevokeTokenAsync(RevokeTokenCommand command)
        {

            var user = await _userRepository.GetByRefreshTokenAsync(command.Token);

            if (user == null)

                return false;

            var refreshToken = user.RefreshTokens.Single(t => t.Token == command.Token);

            if (!refreshToken.IsActive)
                return false;

            refreshToken.RevokedOn = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return true;
        }
        private RefreshToken GenerateRefreshToken()
        {
            var RAndomNumber = new byte[32];

            using var generator = new RNGCryptoServiceProvider();

            generator.GetBytes(RAndomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(RAndomNumber),
                ExpireOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow

            };
        }

        public async Task ForgetPasswordAsync(ForgetPasswordCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);
            if (user is null)
            {
                _logger.LogWarning("ForgetPassword requested for non-existing email {Email}", command.Email);
                return;
            }

            if (!user.IsVerified)
            {

                _logger.LogInformation("ForgetPassword blocked for unverified user {UserId}", user.Id);
                return;
            }


           
            var window = TimeSpan.FromMinutes(10);
            var maxCount = 3;

            if (!CanSendForgetPassword(user, maxCount, window))
            {
                return;
            }
            _logger.LogInformation("ForgetPassword email sent to user {UserId}", user.Id);
            await _userRepository.UpdateAsync(user, cancellationToken);

            var token = await _authRepository.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);

            var resetLink = $"http://localhost:5173/reset-password?email={command.Email}&token={encodedToken}";

            await _emailService.SendEmail(
                user.Email!,
                "Reset Password",
                $"Reset your password from here: <a href=\"{resetLink}\">Reset Password</a>");
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordCommand resetPasswordCommand)
        {
            if (resetPasswordCommand.NewPassword != resetPasswordCommand.ConfirmPassword)
                return false;

            var user = await _userRepository.GetByEmailAsync(resetPasswordCommand.Email);
            if (user == null)
                return false;

            var decodedToken = WebUtility.UrlDecode(resetPasswordCommand.Token);


            var result =  _authRepository.ResetPasswordAsync(user, decodedToken, resetPasswordCommand.NewPassword);
            return await result;
        }

        private string GenerateOtpCode(int length = 6)
        {
            var random = new Random();
            return random.Next(0, (int)Math.Pow(10, length)).ToString($"D{length}");

        }

        public async Task<AuthModel> ResendOtpAsync(ResendOTPCommand command, CancellationToken cancellationToken)
        {
            var authModel = new AuthModel();

            var user = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);
            if (user is null)
            {

                authModel.IsAuthenticated = false;
                authModel.Messege = "Invalid email";
                return authModel;
            }

            if (user.IsVerified)
            {
                authModel.IsAuthenticated = false;
                authModel.Messege = "Account is already verified";
                return authModel;
            }

            var windowStart = DateTime.UtcNow.AddMinutes(-10);
            var recentCodesCount = await _emailVerificationRepository
                .CountUserCodesSinceAsync(user.Id, windowStart, cancellationToken);

            if (recentCodesCount >= 3)
            {

                _logger.LogWarning("ResendOtp rate limit exceeded for user {UserId}", user.Id);
                authModel.IsAuthenticated = false;
                authModel.Messege = "Too many OTP requests. Please try again later.";
                return authModel;
            }

            var code = GenerateOtpCode(6);

            var verification = new EmailVerification
            {
                UserId = user.Id,
                Code = code,
                ExpireAt = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false
            };

            _logger.LogInformation("ResendOtp sent to user {UserId}", user.Id);
            await _emailVerificationRepository.AddEmailVerificationToUserAsync(verification, cancellationToken);

            var subject = "Email Verification Code";
            var body = $"Your new verification code is: <b>{code}</b>";

            await _emailService.SendEmail(user.Email!, subject, body);

            authModel.IsAuthenticated = false;
            authModel.Email = user.Email;
            authModel.Messege = "A new verification code has been sent to your email.";

            return authModel;
        }
        private bool CanSendOtp(ApplicationUser user, int maxCount, TimeSpan window)
        {
            var now = DateTime.UtcNow;

            if (user.OtpWindowStartedAt == null || (now - user.OtpWindowStartedAt) > window)
            {
                // بداية نافذة جديدة
                user.OtpWindowStartedAt = now;
                user.OtpSendCountInWindow = 0;
            }

            if (user.OtpSendCountInWindow >= maxCount)
            {
                return false;
            }

            user.OtpSendCountInWindow++;
            user.LastOtpSentAt = now;
            return true;
        }

        private bool CanSendForgetPassword(ApplicationUser user, int maxCount, TimeSpan window)
        {
            var now = DateTime.UtcNow;

            if (user.ForgetPasswordWindowStartedAt == null || (now - user.ForgetPasswordWindowStartedAt) > window)
            {
                user.ForgetPasswordWindowStartedAt = now;
                user.ForgetPasswordCountInWindow = 0;
            }

            if (user.ForgetPasswordCountInWindow >= maxCount)
            {
                return false;
            }

            user.ForgetPasswordCountInWindow++;
            user.LastForgetPasswordAt = now;
            return true;
        }
    }
}