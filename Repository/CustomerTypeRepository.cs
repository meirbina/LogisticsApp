using SMS.DataContext;
using SMS.IRepository;
using SMS.Models;

namespace SMS.Repository;

public class CustomerTypeRepository : Repository<CustomerType>, ICustomerTypeRepository
{
    public CustomerTypeRepository(AppDbContext context) : base(context)
    {
    }
}