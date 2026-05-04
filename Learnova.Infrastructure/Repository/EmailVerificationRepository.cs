using Learnova.Domain.Entities;
using Learnova.Domain.Identity;
using Learnova.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Learnova.Infrastructure.Repository
{
    public class EmailVerificationRepository
    {
        private readonly LearnovaDbContext _context;

        public EmailVerificationRepository(LearnovaDbContext context)
        {
            _context= context;
        }

        public async Task AddEmailVerificationToUserAsync(EmailVerification verification,CancellationToken cancellationToken)
        {
            _context.EmailVerifications.Add(verification);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
