using Learnova.Application.DTOS.RegisterDto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Authentication.Command.Register
{
    public class RegisterCommand:IRequest<AuthModel>
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
