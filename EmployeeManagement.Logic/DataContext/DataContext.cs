using EmployeeManagement.Model;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace EmployeeManagement.Logic
{
    public class DataContext : DbContext
    {
        public DataContext() : base(CommonFunction.GetConnectionStringname())
        {
        }
        public DbSet<Employee> Employee { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public override int SaveChanges()
        {
            foreach (var changeSet in ChangeTracker.Entries())
            {
                var obj = changeSet.Entity;
            }
            return base.SaveChanges();
        }

    }
}