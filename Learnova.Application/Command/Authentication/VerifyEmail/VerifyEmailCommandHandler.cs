using Learnova.Application.Command.Authentication.Login;
using Learnova.Application.Command.Authentication.ResetPassword;
using Learnova.Application.DTOS.RegisterDto;
using Learnova.Application.IServices;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Command.Authentication.VerifyEmail
{
    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, AuthModel>
    {
        private readonly IAuthService _authService;
        public VerifyEmailCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AuthModel> Handle(VerifyEmailCommand command,CancellationToken cancellationToken)
        {
            return await _authService.VerifyEmailAsync(command, cancellationToken);
        }
    }
}
