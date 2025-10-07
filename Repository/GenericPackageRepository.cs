using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMS.DataContext;
using SMS.IRepository;
using SMS.Models;

namespace SMS.Repository
{
    public class GenericPackageRepository : Repository<GenericPackage>, IGenericPackageRepository
    {
        public GenericPackageRepository(AppDbContext context) : base(context)
        {
        }
    }
}