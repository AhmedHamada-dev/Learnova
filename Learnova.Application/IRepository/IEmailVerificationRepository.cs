using Learnova.Application.Command.Authentication.ResendOTP;
using Learnova.Application.DTOS.RegisterDto;
using Learnova.Domain.Entities;
using Learnova.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.IRepository
{
    public interface IEmailVerificationRepository
    {
        Task AddEmailVerificationToUserAsync(EmailVerification verification, CancellationToken cancellationToken);

        Task<EmailVerification?> GetEmailVerfication(ApplicationUser user, string Code, CancellationToken cancellationToken);

        Task<int> CountUserCodesSinceAsync(string userId, DateTime since, CancellationToken cancellationToken);
    }
}
