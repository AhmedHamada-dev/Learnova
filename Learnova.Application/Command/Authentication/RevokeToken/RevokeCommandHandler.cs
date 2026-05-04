using Learnova.Application.IServices;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Command.Authentication.RevokeToken
{
    public class RevokeCommandHandler:IRequestHandler<RevokeTokenCommand,bool>
    {
        private readonly IAuthService _authService;

        public RevokeCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<bool> Handle(RevokeTokenCommand command,CancellationToken cancellationToken)
        {
            return await _authService.RevokeTokenAsync(command);
        }
    }
}
