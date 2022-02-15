using DatabaseServiceProductConfigurator.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseServiceProductConfigurator.Context {
    public partial class ConfiguratorContext : DbContext {

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "The generated DbContext, do not mark the partial method as static.")]
        partial void OnModelCreatingPartial( ModelBuilder modelBuilder ) {
            modelBuilder.Entity<ConfigurationHasOptionField>().Navigation(c => c.ProductNumbers).AutoInclude();
            modelBuilder.Entity<ConfigurationHasOptionField>().Navigation(c => c.OptionField).AutoInclude();

            modelBuilder.Entity<OptionFieldsHasOptionField>().Navigation(c => c.BaseNavigation).AutoInclude();
            modelBuilder.Entity<OptionFieldsHasOptionField>().Navigation(c => c.OptionFieldNavigation).AutoInclude();

            modelBuilder.Entity<ProductsHasOptionField>().Navigation(c => c.ProductNumberNavigation).AutoInclude();
            modelBuilder.Entity<ProductsHasOptionField>().Navigation(c => c.OptionFieldsNavigation).AutoInclude();

            modelBuilder.Entity<ProductsHasProduct>().Navigation(c => c.BaseProductNavigation).AutoInclude();
            modelBuilder.Entity<ProductsHasProduct>().Navigation(c => c.OptionProductNavigation).AutoInclude();

            //modelBuilder.Entity<OptionField>().Navigation(c => c.ProductsHasOptionFields).AutoInclude();
            //modelBuilder.Entity<OptionField>().Navigation(c => c.OptionFieldsHasOptionFieldOptionFieldNavigations).AutoInclude();
            //modelBuilder.Entity<OptionField>().Navigation(c => c.OptionFieldsHasOptionFieldBaseNavigations).AutoInclude();
            //modelBuilder.Entity<OptionField>().Navigation(c => c.ConfigurationHasOptionFields).AutoInclude();

        }

    }
}
