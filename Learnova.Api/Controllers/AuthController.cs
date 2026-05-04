using Learnova.Application.Command.Authentication.Login;
using Learnova.Application.Command.Authentication.Register;
using Learnova.Application.Command.Authentication.VerifyEmail;
using Learnova.Application.Command.Authentication.ForgetPassword;
using Learnova.Application.Command.Authentication.ResetPassword;
using MediatR;
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
        public async Task<IActionResult> StudentRegister( [FromBody] RegisterCommand command,CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new
            {
                email = result.Email,
                messege = result.Messege
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login( [FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsAuthenticated)
                return BadRequest(result);

             if (!string.IsNullOrEmpty(result.RefreshToken))
                 SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpireOn);

            return Ok(result);
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(
            [FromBody] VerifyEmailCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsAuthenticated)
                return BadRequest(result);

            return Ok(result);
        }


        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordCommand command,CancellationToken cancellationToken)
        {
           
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }

        // POST: api/auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword( [FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (!result) 
                return BadRequest(result);

            return Ok(result);
        }

      
        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOption = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOption);
        }
    }
}