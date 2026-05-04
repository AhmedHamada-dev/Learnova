using Learnova.Application.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Command.Authentication.ResetPassword
{
    public class ResetPasswordCommandHandler
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
