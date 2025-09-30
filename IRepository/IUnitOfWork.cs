using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.IRepository
{
   public interface IUnitOfWork : IDisposable
    {
       
        IStateRepository StateRepository { get; }
        IEmployeeRepository EmployeeRepository { get; }
        ICompanyRepository CompanyRepository { get; }
        IDriverRepository DriverRepository { get; }
        ILocationRepository LocationRepository { get; }
        ICustomerTypeRepository CustomerTypeRepository { get; }
        IServiceTypeRepository ServiceTypeRepository { get; }
        IPackagingPriceRepository PackagingPriceRepository { get; }
        IInsuranceRepository InsuranceRepository { get; }
        ICouponRepository CouponRepository { get; }
        IMerchantRepository MerchantRepository { get; }
        IWeightPriceRepository WeightPriceRepository { get; }
        IWalletRepository WalletRepository { get; }
        IVehicleRepository VehicleRepository { get; }
        Task<int> CompleteAsync(); 
    }
}