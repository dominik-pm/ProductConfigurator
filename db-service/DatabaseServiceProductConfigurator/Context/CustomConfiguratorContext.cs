using Microsoft.EntityFrameworkCore;

namespace DatabaseServiceProductConfigurator.Models {
    public partial class ConfiguratorContext : DbContext {

        partial void OnModelCreatingPartial( ModelBuilder modelBuilder ) {
            modelBuilder.Entity<ConfigurationHasOptionField>().Navigation(c => c.ProductNumbers).AutoInclude();
        }

    }
}
