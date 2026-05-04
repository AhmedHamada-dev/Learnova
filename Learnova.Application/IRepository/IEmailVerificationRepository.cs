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
    }
}
