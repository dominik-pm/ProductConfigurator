using DatabaseServiceProductConfigurator.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseServiceProductConfigurator.Context {
    public partial class ConfiguratorContext : DbContext {

        partial void OnModelCreatingPartial( ModelBuilder modelBuilder ) {
            modelBuilder.Entity<ConfigurationHasOptionField>().Navigation(c => c.ProductNumbers).AutoInclude();
        }

    }
}
