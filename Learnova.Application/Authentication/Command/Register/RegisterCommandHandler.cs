using Learnova.Application.DTOS.RegisterDto;
using Learnova.Application.IRepository;
using Learnova.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Authentication.Command.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthModel>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthRepository _authRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public RegisterCommandHandler(IUserRepository userRepository, IAuthRepository authRepository,UserManager<ApplicationUser> userManager)
        {
            _userRepository = userRepository;
            _authRepository = authRepository;
            _userManager = userManager;
        }
        public async Task<AuthModel> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (await _userRepository.ExistsByEmailAsync(request.Email,cancellationToken))
                return new AuthModel
                {
                    Messege = "Email already Exist"
                };
            var user = new ApplicationUser
            {
                FullName = request.FullName,
                Email = request.Email,
                UserName = request.Email,
            };
            await _userManager.AddToRoleAsync(user, "Student");
            await _userRepository.AddAsync(user,request.Password,cancellationToken);
            var stringToken= await _authRepository.CreateJwtTokenAsync(user);

            var refreshToken =await _authRepository.GenerateRefreshToken();
            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            return new AuthModel
            {
                IsAuthenticated = true,
                Email = user.Email,
                Messege = "isAuth",
                Username = user.Email,
                Role = roles.ToList(),
                Token = stringToken,
                RefreshToken=refreshToken.Token,
                RefreshTokenExpireOn = refreshToken.ExpireOn
            };
            
        }
    }
}
