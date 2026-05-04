using Learnova.Application.Command.Authentication.ForgetPassword;
using Learnova.Application.Command.Authentication.Login;
using Learnova.Application.Command.Authentication.Register;
using Learnova.Application.Command.Authentication.ResetPassword;
using Learnova.Application.DTOS.RegisterDto;
using Learnova.Application.IRepository;
using Learnova.Application.IServices;
using Learnova.Application.Settings;
using Learnova.Domain.Entities;
using Learnova.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        public AuthService(IOptions<JwtSettings> jwtOptions, IUserRepository userRepository, IAuthRepository authRepository, IEmailService emailService, IEmailVerificationRepository emailVerificationRepository)
        {
            _Jwt = jwtOptions.Value;
            _userRepository = userRepository;
            _authRepository = authRepository;
            _emailService = emailService;
            _emailVerificationRepository = emailVerificationRepository;
        }
        public async Task<string> CreateJwtTokenAsync(ApplicationUser applicationUser)
        {
            var UserClaims = await _authRepository.GetClaimsAsync(applicationUser);
            var Roles = await _authRepository.GetRolesAsync(applicationUser);
            var roleClaims = new List<Claim>();

            foreach (var role in Roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var Claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Email,applicationUser.Email),
            new Claim(ClaimTypes.NameIdentifier,applicationUser.Id),
            }
            .Union(roleClaims)
            .Union(UserClaims);

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_Jwt.Key));
            var singcre = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var JWTToken = new JwtSecurityToken
            (
                issuer: _Jwt.Issuer,
                audience: _Jwt.Audience,
                claims: Claims,
               expires: DateTime.UtcNow.AddDays(_Jwt.DurationInDays),
                signingCredentials: singcre);

            var stringToken = new JwtSecurityTokenHandler().WriteToken(JWTToken);

            return stringToken;

        }
        public async Task<AuthModel> RefreshTokenAsync(string refreshtoken)
        {
            var authModel = new AuthModel();
            var user = await _userRepository.GetByRefreshTokenAsync(refreshtoken);

            if (user == null)
            {

                authModel.Messege = "Invalid Token";
                return authModel;
            }
            var refreshToken = user.RefreshTokens.Single(t => t.Token == refreshtoken);

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
            authModel.ExpireOn = DateTime.UtcNow.AddDays(_Jwt.DurationInDays);

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

            //var stringToken = await CreateJwtTokenAsync(user);


            //var refreshToken = GenerateRefreshToken();
            //user.RefreshTokens.Add(refreshToken);
            //await _userRepository.UpdateAsync(user, cancellationToken);


            //var roles = await _authRepository.GetRolesAsync(user);

            //return new AuthModel
            //{
            //    IsAuthenticated = true,
            //    Email = user.Email,
            //    Messege = "User registered successfully",
            //    Username = user.Email,
            //    Role = roles.ToList(),
            //    Token = stringToken,
            //    RefreshToken = refreshToken.Token,
            //    RefreshTokenExpireOn = refreshToken.ExpireOn,
            //    ExpireOn = DateTime.UtcNow.AddDays(_Jwt.DurationInDays)

            //};
        }

        public async Task<AuthModel> Login(LoginCommand request, CancellationToken cancellationToken)
        {

            var authModel = new AuthModel();
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user is null)
            {
                authModel.IsAuthenticated = false;
                authModel.Messege = "No account registered with this email";
                return authModel;
            }

            var passwordValid = await _userRepository.CheckPasswordAsync(user, request.Password);
            if (!passwordValid)
            {
                authModel.IsAuthenticated = false;
                authModel.Messege = "Invalid email or password";
                return authModel;
            }


            if (!user.IsVerified)
            {
                authModel.IsAuthenticated = false;
                authModel.Messege = "Account is not verified yet";
                return authModel;
            }

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
            authModel.ExpireOn = DateTime.UtcNow.AddDays(_Jwt.DurationInDays);
            authModel.Messege = "Login completed successfully";

            return authModel;
        }
        public async Task<bool> RevokeTokenAsync(string token)
        {

            var user = await _userRepository.GetByRefreshTokenAsync(token);

            if (user == null)

                return false;

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

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

        public async Task ForgetPasswordAsync(ForgetPasswordCommand command)
        {
            var User = await _userRepository.GetByEmailAsync(command.Email);
            if (User == null)
                return;

            if (User.IsVerified == false)
                return;

            var token = await _authRepository.GeneratePasswordResetTokenAsync(User);
            var encodedToken = WebUtility.UrlEncode(token);

            var resetLink = $"http://localhost:5173/reset-password?email={command.Email}&token={encodedToken}";

           // BackgroundJob.Enqueue<IEmailService>(x => x.SendEmail(result.Email, "ResetPassword", $"Reset your password from here: {resetLink}"));
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
    }
}