using Learnova.Application.Command.Authentication.Register;
using Learnova.Application.DTOS.RegisterDto;
using Learnova.Application.IRepository;
using Learnova.Application.IServices;
using Learnova.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Command.Authentication.Login
{
    public class LoginCommandHandler:IRequestHandler<LoginCommand,AuthModel>
    {
        private readonly IAuthService _authService;

        public LoginCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<AuthModel> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return await _authService.Login(request, cancellationToken);

        }
    }
}
