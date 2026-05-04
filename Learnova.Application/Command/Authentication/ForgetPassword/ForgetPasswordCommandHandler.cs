using Learnova.Application.IServices;

namespace Learnova.Application.Command.Authentication.ForgetPassword
{
    public class ForgetPasswordCommandHandler
    {
        private readonly IAuthService _authService;

        public ForgetPasswordCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task Handle(ForgetPasswordCommand command, CancellationToken cancellationToken)
        {
             await _authService.ForgetPasswordAsync(command);
        }
    }
}
