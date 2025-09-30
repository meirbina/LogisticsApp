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

        public DbSet<ControlClass> ControlClasses { get; set; }
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
        public DbSet<BookCategory> BookCategories { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookIssue> BookIssues { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<HostelMaster> HostelMasters { get; set; }
        public DbSet<VehicleMaster> VehicleMasters { get; set; }
        public DbSet<Stoppage> Stoppages { get; set; }
        public DbSet<RouteMaster> RouteMasters { get; set; }
        public DbSet<HostelRoom> HostelRooms { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<AssignVehicle> AssignVehicles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<HostelCategory> HostelCategories { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<FeesType> FeesTypes { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<ControlClassSection> ControlClassSection { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<FeesGroup> FeesGroups { get; set; }
        public DbSet<ExamTerm> ExamTerms { get; set; }
        public DbSet<ExamHall> ExamHalls { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<MarkDistribution> MarkDistributions { get; set; }
        public DbSet<FeesGroupDetail> FeesGroupDetails { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Accounting> Accountings { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<VoucherHead> VoucherHeads { get; set; }
        public DbSet<Deposit> Deposits { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExamMarkDistribution> ExamMarkDistributions { get; set; }
        public DbSet<SectionSubject> SectionSubjects { get; set; }
        public DbSet<TeacherSubjectAssignment> TeacherSubjectAssignments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // This line is ESSENTIAL for Identity to build its tables.
            base.OnModelCreating(modelBuilder);

            // Add the Parent-User relationship
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(a => a.Parent)
                .WithOne(p => p.ApplicationUser)
                .HasForeignKey<Parent>(p => p.ApplicationUserId);

            // Your existing configuration for the Class-Section relationship (Correct)
            modelBuilder.Entity<ControlClassSection>().HasKey(cs => new { cs.ControlClassId, cs.SectionId });

            modelBuilder.Entity<ControlClassSection>()
                .HasOne(cs => cs.ControlClass)
                .WithMany()
                .HasForeignKey(cs => cs.ControlClassId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ControlClassSection>()
                .HasOne(cs => cs.Section)
                .WithMany()
                .HasForeignKey(cs => cs.SectionId)
                .OnDelete(DeleteBehavior.NoAction);

            // *** THIS IS THE MISSING PIECE THAT FIXES THE ERROR ***
            // This tells EF Core that the primary key for SectionSubject is the combination
            // of SectionId and SubjectId.
            modelBuilder.Entity<SectionSubject>()
                .HasKey(ss => new { ss.SectionId, ss.SubjectId });
            
            // ADD THIS NEW CONFIGURATION for the Teacher-Subject-Section assignment
            // This ensures you cannot create duplicate assignments.
            modelBuilder.Entity<TeacherSubjectAssignment>()
                .HasIndex(tsa => new { tsa.EmployeeId, tsa.SectionId, tsa.SubjectId })
                .IsUnique();
            
            modelBuilder.Entity<ExamMarkDistribution>()
                .HasKey(emd => new { emd.ExamId, emd.MarkDistributionId });
        }
    }
}