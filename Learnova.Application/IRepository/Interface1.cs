using Learnova.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.IRepository
{
    public interface IUserRepository
    {
        Task AddAsync(ApplicationUser user);
        Task<bool> ExistsByEmail(string Email);
        Task<ApplicationUser?> GetByEmail(string Email);

    }
}
