using Azure.Core;
using Learnova.Application.IRepository;
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
    public class EmailVerificationRepository:IEmailVerificationRepository
    {
        private readonly LearnovaDbContext _context;

        public EmailVerificationRepository(LearnovaDbContext context)
        {
            _context= context;
        }

        public async Task<int> CountUserCodesSinceAsync(string userId, DateTime since, CancellationToken cancellationToken)
        {
            return await _context.EmailVerifications
                .Where(v => v.UserId == userId && v.ExpireAt >= since)
                .CountAsync(cancellationToken);
        }
        public async Task AddEmailVerificationToUserAsync(EmailVerification verification,CancellationToken cancellationToken)
        {
            _context.EmailVerifications.Add(verification);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<EmailVerification?> GetEmailVerfication(ApplicationUser user,string Code,CancellationToken cancellationToken)
        {
            return await _context.EmailVerifications
                .Where(v => v.UserId == user.Id && v.Code == Code)
                .OrderByDescending(v => v.ExpireAt)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
