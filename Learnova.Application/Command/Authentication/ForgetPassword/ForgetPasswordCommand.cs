using Learnova.Application.DTOS;
using MediatR;
using Microsoft.AspNetCore.Identity.Data;

namespace Learnova.Application.Command.Authentication.ForgetPassword
{
    public class ForgetPasswordCommand : IRequest<ForgetPasswordResult>
    {
        public string Email { get; set; } = null!;

    }
}
