using Learnova.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Entities
{
    public class EmailVerification
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
        public string Code { get; set; }
        public DateTime ExpireAt { get; set; }  
        public bool IsUsed { get; set; }

    }
}
