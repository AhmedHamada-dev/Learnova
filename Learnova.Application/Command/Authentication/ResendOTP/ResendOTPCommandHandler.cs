using Learnova.Application.Command.Authentication.ResendOTP;
using Learnova.Application.DTOS.RegisterDto;
using Learnova.Application.IServices;
using MediatR;

public class ResendOtpCommandHandler : IRequestHandler<ResendOTPCommand, AuthModel>
{
    private readonly IAuthService _authService;

    public ResendOtpCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthModel> Handle(ResendOTPCommand request, CancellationToken cancellationToken)
    {
        return await _authService.ResendOtpAsync(request, cancellationToken);
    }
}