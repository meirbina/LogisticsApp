using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMS.DataContext;
using SMS.IRepository;
using SMS.Models;

namespace SMS.Repository
{
    public class PackagingPriceRepository : Repository<PackagingPrice>, IPackagingPriceRepository
    {
        public PackagingPriceRepository(AppDbContext context) : base(context)
        {
        }
    }
}