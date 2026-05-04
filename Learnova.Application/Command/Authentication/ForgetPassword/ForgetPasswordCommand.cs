using MediatR;

namespace Learnova.Application.Command.Authentication.ForgetPassword
{
    public class ForgetPasswordCommand : IRequest
    {
        public string Email { get; set; }

    }
}
