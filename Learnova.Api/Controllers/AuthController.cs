using Learnova.Application.Command.Authentication.Login;
using Learnova.Application.Command.Authentication.Register;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
        public async Task<IActionResult> StudentRegister([FromBody] RegisterCommand command)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _mediator.Send(command);

            return Ok(new { email = result.Email, Messege = result.Messege});
        }

    //      if (!result.IsAuthenticated)
    //        {
    //            return BadRequest(result.Messege);
    //}

    //        if (!string.IsNullOrEmpty(result.RefreshToken))
    //            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpireOn);
    [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsAuthenticated)
                return BadRequest(result);

            return Ok(result);
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
