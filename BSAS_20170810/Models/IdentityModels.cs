using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BSAS_20170810.Models
{
    public class User : IdentityUser<int, UserLogin, UserRole, UserClaim>
    {
        // Added User table columns
        //
        [Display (Name = "First Name")]
        public string FirstName { get; set; }
        [Display (Name = "Surname")]
        public string Surname { get; set; }

        [Display (Name = "Full Name")]
        public string FullName
        {
            get
            {
                return Surname + ", " + FirstName;
            }
        }
        
        public string Rank { get; set; }
        public int StationId { get; set; }
        public Station Station { get; set; }
        [NotMapped]
        [Display (Name = "Station")]
        public string StationName { get; set; }

        public ICollection<Demand> Demands { get; set; }
        //
        // End

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
    //Added by me
    //
    public class UserRole : IdentityUserRole<int> { }
    public class UserClaim : IdentityUserClaim<int> { }
    public class UserLogin : IdentityUserLogin<int> { }

    public class Role : IdentityRole<int, UserRole>
    {
        public Role() { }
        public Role(string name)
        {
            Name = name;
        }
    }

    public class UserStore : UserStore<User, Role, int, UserLogin, UserRole, UserClaim>
    {
        public UserStore(BSASDbContext context) : base(context) { }
    }

    public class RoleStore : RoleStore<Role, int, UserRole>
    {
        public RoleStore(BSASDbContext context) : base(context) { }
    }

    public class BSASDbContext : IdentityDbContext<User, Role, int, UserLogin, UserRole, UserClaim>
    {
        public BSASDbContext()
            : base("DefaultConnection") { }

        public static BSASDbContext Create()
        {
            return new BSASDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("Users").Property(p => p.Id).HasColumnName("UserID");
            modelBuilder.Entity<UserRole>().ToTable("UserRoles");
            modelBuilder.Entity<UserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<UserClaim>().ToTable("UserClaims").Property(p => p.Id).HasColumnName("UserClaimId");
            modelBuilder.Entity<Role>().ToTable("Roles").Property(p => p.Id).HasColumnName("RoleId");
            modelBuilder.Entity<DemandStatus>().ToTable("DemandStatuses");

            modelBuilder.Entity<HoldingCategory>().Property(p => p.Id).HasColumnName("CategoryId");
            modelBuilder.Entity<StationRegion>().Property(p => p.Id).HasColumnName("RegionId");
            modelBuilder.Entity<Demand>().Property(p => p.UserId).IsOptional();
            modelBuilder.Entity<DemandStatus>().Property(p => p.Id).HasColumnName("StatusId");
            modelBuilder.Entity<Demand>().Property(p => p.DateDelivered).IsOptional();
            modelBuilder.Entity<Demand>().Property(p => p.DateDespatched).IsOptional();
            modelBuilder.Entity<DemandItem>().Property(p => p.ItemQuantity).IsOptional();
            
        }
        //
        // End Add
        public DbSet<Holding> Holdings { get; set; }

        public DbSet<HoldingCategory> HoldingCategories { get; set; }

        public DbSet<Demand> Demands { get; set; }

        public DbSet<Station> Stations { get; set; }

        public DbSet<StationRegion> StationRegions { get; set; }

        public DbSet<DemandStatus> DemandStatuses { get; set; }

        public DbSet<DemandItem> DemandItems { get; set; }

        public DbSet<HoldingQuantity> HoldingQuantities { get; set; }
    }
}