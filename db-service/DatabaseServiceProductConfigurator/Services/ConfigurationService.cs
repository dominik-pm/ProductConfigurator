using DatabaseServiceProductConfigurator.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseServiceProductConfigurator.Services {

    public class ConfigurationService {

        static product_configuratorContext context = new product_configuratorContext();

        private static IQueryable<Configuration> getConfigs() => context.Configurations
            .Include(c => c.ConfigurationHasOptionFields.Where(cof => cof.ParentOptionFieldId.Equals(null) || cof.ParentConfigId.Equals(null)))
            .Include("ConfigurationHasOptionFields.ProductNumbers")
            .Include("ConfigurationHasOptionFields.InverseParent.ProductNumbers");

        public static Configuration? GetById( int id ) => getConfigs()
            .Where(c => c.Id.Equals(id))
            .FirstOrDefault();

        public static List<Configuration> GetConfigurationsByCustomer( int? customerID ) => getConfigs()
            .Where(c => c.Customer.Equals(customerID))
            .ToList();

        public static List<Configuration> GetByProductNumber( string productNumber ) => GetConfigurationsByCustomer(null)
            .Where(c => c.ProductNumber.Equals(productNumber))
            .ToList();

    }
}
