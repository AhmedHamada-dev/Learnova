using Learnova.Application.Command.Authentication.Login;
using Learnova.Application.DTOS.RegisterDto;
using Learnova.Application.IServices;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Command.Authentication.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, bool>
    {
        private readonly IAuthService _authService;

        public ResetPasswordCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<bool> Handle(ResetPasswordCommand command,CancellationToken cancellationToken)
        {
            return await _authService.ResetPasswordAsync(command);
        }
    }
    }
