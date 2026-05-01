using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Entities
{
    public class Coupon
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal DiscountPercentage { get; set; }
        public bool MakesFree {  get; set; }

        public DateTime ExpireAt { get; set; }
        public bool IsActive { get; set; }
        public int UsageLimit { get; set; } = 0;
        public int Usedcount { get; set; } = 0;
        public int? CourseId { get; set; }
        public Course Course { get; set; }
    }
}
