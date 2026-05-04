using Learnova.Application.DTOS;
using Learnova.Application.IServices;
using MediatR;

namespace Learnova.Application.Command.Authentication.ForgetPassword
{
    public class ForgetPasswordCommandHandler : IRequestHandler<ForgetPasswordCommand,ForgetPasswordResult>
    {
        private readonly IAuthService _authService;

        public ForgetPasswordCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<ForgetPasswordResult> Handle(ForgetPasswordCommand command, CancellationToken cancellationToken)
        {
            await _authService.ForgetPasswordAsync(command, cancellationToken);
            return  new ForgetPasswordResult { Message = "If this email is registered, a reset link has been sent." };
        }
    }
    
}
