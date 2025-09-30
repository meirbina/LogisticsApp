using SMS.DataContext;
using SMS.IRepository;
using SMS.Models;

namespace SMS.Repository;

public class CompanyRepository : Repository<Company>, ICompanyRepository
{
    public CompanyRepository(AppDbContext context) : base(context)
    {
    }
}