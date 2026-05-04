using Learnova.Application.DTOS.RegisterDto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Command.Authentication.VerifyEmail
{
    public class VerifyEmailCommand : IRequest<AuthModel>
    {
        public string Email { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
}
