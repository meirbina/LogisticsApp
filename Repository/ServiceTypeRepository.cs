using SMS.DataContext;
using SMS.IRepository;
using SMS.Models;

namespace SMS.Repository;

public class ServiceTypeRepository : Repository<ServiceType>, IServiceTypeRepository
{
    public ServiceTypeRepository(AppDbContext context) : base(context)
    {
    }
}