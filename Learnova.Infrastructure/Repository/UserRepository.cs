using Learnova.Application.IRepository;
using Learnova.Domain.Identity;
using Learnova.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Infrastructure.Repository
{
    public class UserRepository 
    {
        private readonly LearnovaDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(LearnovaDbContext dbContext)
        {
            _dbContext = dbContext;
            int x = 5;
        }
        public async Task AddUserAsync(ApplicationUser user,string password)
        {
            var result = await _userManager.CreateAsync(user, password);
          
        }

        public async Task<bool> ExistsByEmail(string Email)
        {
            return await _dbContext.Users.AnyAsync(u=>u.Email == Email);
            
        }

        public async Task<ApplicationUser?> GetByEmail(string Email)
        {
           return await _dbContext.Users.FirstOrDefaultAsync(u=>u.Email == Email);
        }
    }
}
