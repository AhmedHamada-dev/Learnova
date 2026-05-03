using Learnova.Application.Authentication.Command.Register;
using Learnova.Application.DTOS.RegisterDto;
using Learnova.Application.IRepository;
using Learnova.Application.IServices;
using Learnova.Application.Settings;
using Learnova.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Services
{
    public class AuthService:IAuthService
    {
       private readonly JwtSettings _Jwt;
       private readonly IUserRepository _userRepository;
       private readonly IAuthRepository _authRepository;
        public AuthService(IOptions<JwtSettings> jwtOptions, IUserRepository userRepository, IAuthRepository authRepository)
        {
            _Jwt = jwtOptions.Value;   
            _userRepository = userRepository;
            _authRepository = authRepository;
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
            };

           
            await _userRepository.AddAsync(user, request.Password, cancellationToken);
            
            await _authRepository.AddToRoleAsync(user, "Student");
            var stringToken = await CreateJwtTokenAsync(user);

          
            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(refreshToken);
            await _userRepository.UpdateAsync(user, cancellationToken);

           
            var roles = await _authRepository.GetRolesAsync(user);

            return new AuthModel
            {
                IsAuthenticated = true,
                Email = user.Email,
                Messege = "User registered successfully",
                Username = user.Email,
                Role = roles.ToList(),
                Token = stringToken,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpireOn = refreshToken.ExpireOn,
                ExpireOn = DateTime.UtcNow.AddDays(_Jwt.DurationInDays)

            };
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

            return  new RefreshToken
            {
                Token = Convert.ToBase64String(RAndomNumber),
                ExpireOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow

            };
        }
    }
}
