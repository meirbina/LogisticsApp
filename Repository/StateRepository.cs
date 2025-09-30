using SMS.DataContext;
using SMS.IRepository;
using SMS.Models;

namespace SMS.Repository
{
    public class StateRepository : Repository<State>, IStateRepository
    {
        public StateRepository(AppDbContext context) : base(context)
        {
        }
    }
}