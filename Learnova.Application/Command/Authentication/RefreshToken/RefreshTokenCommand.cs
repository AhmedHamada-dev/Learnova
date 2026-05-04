using Learnova.Application.DTOS.RegisterDto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Command.Authentication.RefreshToken
{
    public class RefreshTokenCommand:IRequest<AuthModel>
    {
        public string refreshtoken {  get; set; }
    }
}
