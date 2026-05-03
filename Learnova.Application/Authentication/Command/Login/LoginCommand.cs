using Learnova.Application.DTOS.RegisterDto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Authentication.Command.Login
{
    public class LoginCommand:IRequest<AuthModel>
    {
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
