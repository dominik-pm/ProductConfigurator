using DatabaseServiceProductConfigurator.Models;
using Microsoft.EntityFrameworkCore;
using Model;

namespace DatabaseServiceProductConfigurator.Services {

    internal struct ConfiguredProductStruct {
        public float price { get; init; }
        public List<Option> options { get; init; }

        public ConfiguredProductStruct( float price, List<Option> options ) {
            this.price = price;
            this.options = options;
        }
    }

    public static class ConfigurationService {

        static product_configuratorContext context = new product_configuratorContext();

        public static List<string> getDefaultConfig(string id, string lang ) {
            List<string> toAdd = new List<string>();

            List<Option>? options =  (
                from conf in context.Configurations
                where conf.ProductNumber == id && conf.Customer == null
                let opts = GetOptionsByConfigId(conf.Id, lang)
                select opts.options
            ).FirstOrDefault();

            if ( options == null )
                return toAdd;

            toAdd.AddRange(options.Select(o => o.Id).ToList());

            return toAdd;
        }

        public static List<ConfiguredProduct> GetConfiguredProducts( string lang ) {
            return (
                from conf in context.Configurations
                let opts = GetOptionsByConfigId(conf.Id, lang)
                select new ConfiguredProduct {
                    ConfigurationName = conf.ProductNumber,
                    Options = opts.options,
                    Price = opts.price
                }
            ).ToList();
        }

        public static ConfiguredProduct? GetConfiguredProductById( string id ) => GetConfiguredProducts(id).FirstOrDefault();

        private static ConfiguredProductStruct GetOptionsByConfigId( int configId, string lang ) {
            product_configuratorContext localContext = new product_configuratorContext();

            List<ConfigurationHasOptionField> fields = new List<ConfigurationHasOptionField>();
            foreach ( var item in (
                from op in localContext.Configurations
                where op.Id == configId
                select op.ConfigurationHasOptionFields
            ).ToList() ) {
                fields.AddRange(item.ToList());
            }

            List<Option> toReturn = new List<Option>();
            float price = 0;

            foreach ( var item in fields ) {

                foreach ( var el in item.ProductNumbers ) {
                    price += el.Price;

                    ProductHasLanguage? infos = LanguageService.GetProductWithLanguage(el.ProductNumber, lang);
                    toReturn.Add(
                        new Option {
                            Id = el.ProductNumber,
                            Name = infos == null ? "" : infos.Name,
                            Description = infos == null ? "" : infos.Description
                        }
                    );
                }
            }

            return new ConfiguredProductStruct(price, toReturn);
        }

        public static bool SaveConfiguredProduct( ConfiguredProduct toSave ) {
            bool worked = true;

            try {
                Configuration added = context.Configurations.Add(
                    new Configuration {
                        ProductNumber = toSave.ConfigurationName,
                        Customer = null
                    }
                ).Entity;

                List<string> products = ( from p in context.Products where toSave.Options.Select(c => c.Id).Contains(p.ProductNumber) select p.ProductNumber ).ToList();

                List<ConfigurationHasOptionField> fields = (
                    from of in context.OptionFields
                    join pof in context.ProductsHasOptionFields on of.Id equals pof.OptionFields
                    where products.Contains(pof.ProductNumber)
                    select new ConfigurationHasOptionField {
                        ConfigId = added.Id,
                        OptionFieldId = of.Id,
                        ProductNumbers = ( from pof1 in context.ProductsHasOptionFields where pof1.OptionFields == of.Id && products.Contains(pof.ProductNumber) select pof1.ProductNumberNavigation ).ToList()
                    }
                ).ToList();

                context.ConfigurationHasOptionFields.AddRange(fields);
                context.SaveChanges();
            }
            catch ( Exception ex ) {
                Console.WriteLine(ex.Message);
                context.Dispose();
                worked = false;
            }

            return worked;

        }


    }
}