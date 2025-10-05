using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SMS.Models;

namespace SMS.DataContext
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<ShipmentCollection> ShipmentCollections { get; set; }
        public DbSet<ManifestShipment> ManifestShipments { get; set; }
        public DbSet<Manifest> Manifests { get; set; }
        public DbSet<MerchantShipment> MerchantShipments { get; set; }
        public DbSet<MerchantShipmentItem> MerchantShipmentItems { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<WeightPrice> WeightPrices { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<ShipmentItem> ShipmentItems { get; set; }
        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<CustomerType> CustomerTypes { get; set; }
        public DbSet<PackagingPrice> PackagingPrices { get; set; }
        public DbSet<ServiceType> ServiceTypes { get; set; }
      
        public DbSet<Employee> Employees { get; set; }
       
        public DbSet<Company> Companies { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Location> Locations { get; set; }
       
        public DbSet<State> States { get; set; }
        public DbSet<GenericPackage> GenericPackages { get; set; }
        public DbSet<GenericShipment> GenericShipments { get; set; }
        public DbSet<GenericShipmentItem> GenericShipmentItems { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // This line is ESSENTIAL for Identity to build its tables.
            base.OnModelCreating(modelBuilder);

           
        }
    }
}