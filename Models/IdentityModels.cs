using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Security.Claims;
using System.Threading.Tasks;
using InstantTutors.Migrations;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace InstantTutors.Models
{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public string CurrentUserId { get; set; }
        public ApplicationDbContext()
            : base("ITutorsConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
        }


        public DbSet<Tutors> Tutors { get; set; }
        public DbSet<TuitionSubjects> TuitionSubjects { get; set; }
        public DbSet<TutorAvailability> TutorAvailability { get; set; }
        public DbSet<Sessions> Sessions { get; set; }
        public DbSet<SessionSchedule> SessionSchedule { get; set; }

        public DbSet<HomepageControls> HomepageControls { get; set; }
        public DbSet<AboutControls> AboutControls { get; set; }
        public DbSet<Configurations> Configurations { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }
}
