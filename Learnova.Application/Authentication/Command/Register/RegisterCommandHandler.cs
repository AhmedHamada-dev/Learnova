using Learnova.Application.DTOS.RegisterDto;
using Learnova.Application.IRepository;
using Learnova.Application.IServices;
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
     
        private readonly IAuthService _authService;
       
        public RegisterCommandHandler(IUserRepository userRepository, IAuthRepository authRepository,UserManager<ApplicationUser> userManager,IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<AuthModel> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
           return await _authService.StudentRegister(request,cancellationToken);
         
        }
    }
}
