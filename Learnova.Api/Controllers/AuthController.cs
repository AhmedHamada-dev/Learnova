using Learnova.Application.Authentication.Command.Register;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Learnova.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _mediator.Send(command);
            if (!result.IsAuthenticated)
            {
                return BadRequest(result.Messege);
            }

            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpireOn);

            return Ok(new { token = result.Token, expireOn = result.ExpireOn, Role = result.Role });
        }


        private void SetRefreshTokenInCookie(string RefreshToken, DateTime Expires)
        {
            var cookieOption = new CookieOptions
            {
                HttpOnly = true,
                Expires = Expires
            };

            Response.Cookies.Append("refreshToken", RefreshToken, cookieOption);
        }
    }
}
