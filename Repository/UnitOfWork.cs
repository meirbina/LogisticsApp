using SMS.DataContext;
using SMS.IRepository;

namespace SMS.Repository
{
   public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IVehicleRepository VehicleRepository { get; }
        public ICompanyRepository CompanyRepository { get; }
        public IEmployeeRepository EmployeeRepository {get;}  
        
        public IStateRepository StateRepository { get; }
        public IDriverRepository DriverRepository { get; }
        public ILocationRepository LocationRepository { get; }
        public ICustomerTypeRepository CustomerTypeRepository { get; }
        public IServiceTypeRepository ServiceTypeRepository { get; }
        public IPackagingPriceRepository PackagingPriceRepository { get; }
        public IInsuranceRepository InsuranceRepository { get; }
        public ICouponRepository CouponRepository { get; }
        public IMerchantRepository MerchantRepository { get; }
        public IWeightPriceRepository WeightPriceRepository { get; }
        public IWalletRepository WalletRepository { get; }
        public IGenericPackageRepository GenericPackageRepository { get; }
        
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            CompanyRepository = new CompanyRepository(_context);
            EmployeeRepository = new EmployeeRepository(_context);
            DriverRepository = new DriverRepository(_context);
            StateRepository = new StateRepository(_context);
            LocationRepository = new LocationRepository(_context);
            CustomerTypeRepository = new CustomerTypeRepository(_context);
            ServiceTypeRepository = new ServiceTypeRepository(_context);
            PackagingPriceRepository = new PackagingPriceRepository(_context);
            InsuranceRepository = new InsuranceRepository(_context);
            CouponRepository = new CouponRepository(_context);
            MerchantRepository = new MerchantRepository(_context);
            WeightPriceRepository = new WeightPriceRepository(_context);
            WalletRepository = new WalletRepository(_context);
            VehicleRepository = new VehicleRepository(_context);
            GenericPackageRepository = new GenericPackageRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}