using Learnova.Application.Command.Authentication.Login;
using Learnova.Application.Command.Authentication.Register;
using Learnova.Application.Command.Authentication.VerifyEmail;
using Learnova.Application.Command.Authentication.ForgetPassword;
using Learnova.Application.Command.Authentication.ResetPassword;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using Learnova.Application.Command.Authentication.RefreshToken;
using Learnova.Application.Command.Authentication.RevokeToken;
using Learnova.Application.Command.Authentication.ResendOTP;

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

       
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword( [FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (!result) 
                return BadRequest(result);

            return Ok("Password Reset Successfully");
        }

        [HttpGet("refresh-token")]
        public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest("Refresh token is missing.");

            var command = new RefreshTokenCommand { refreshtoken = refreshToken };

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsAuthenticated)
                return BadRequest(result);

            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpireOn);

            return Ok(result);
        }

        [HttpPost("RevokeToken")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenCommand command,CancellationToken cancellationToken)
        {
            var token = command.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is requred!");

            var result = await _mediator.Send(command, cancellationToken);

            if (!result)
                return BadRequest("Token is invalid!");

            return Ok();
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOTPCommand command,CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(command, cancellationToken);

          
            if (result.Messege == "Too many OTP requests. Please try again later.")
                return StatusCode(StatusCodes.Status429TooManyRequests, result); 

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