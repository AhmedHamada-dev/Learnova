using Learnova.Application.DTOS.RegisterDto;
using Learnova.Application.IRepository;
using Learnova.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Http.Features;
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

        public RegisterCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<AuthModel> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (await _userRepository.ExistsByEmail(request.Email))
                return new AuthModel
                {
                    Messege = "Email already Exist"
                };
            var user = new ApplicationUser
            {
                FullName = request.FullName,
                Email = request.Email,
                UserName = request.Email
            };
            await _userRepository.AddAsync(user);
            return new AuthModel
            {
                IsAuthenticated = true,
                Email = user.Email,
                Messege = "isAuth",
                Username = user.UserName,
                //Role = user.Role,
                Token = "dd"

            };
            
        }
    }
}
