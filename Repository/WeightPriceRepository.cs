using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMS.DataContext;
using SMS.IRepository;
using SMS.Models;

namespace SMS.Repository
{
    public class WeightPriceRepository : Repository<WeightPrice>, IWeightPriceRepository
    {
        public WeightPriceRepository(AppDbContext context) : base(context)
        {
        }
    }
}