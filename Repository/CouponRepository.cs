using SMS.DataContext;
using SMS.IRepository;
using SMS.Models;

namespace SMS.Repository
{
    public class CouponRepository : Repository<Coupon>, ICouponRepository
    {
        public CouponRepository(AppDbContext context) : base(context)
        {
            
        }
    }
}

