using Learnova.Application.DTOS.RegisterDto;
using Learnova.Application.IServices;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Command.Authentication.RefreshToken
{
    public class RefreshTokenCommandHandler:IRequestHandler<RefreshTokenCommand,AuthModel>
    {
        private readonly IAuthService _authService;

        public RefreshTokenCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AuthModel> Handle(RefreshTokenCommand command,CancellationToken cancellationToken)
        {
            return await _authService.RefreshTokenAsync(command);
        }
    }

}
