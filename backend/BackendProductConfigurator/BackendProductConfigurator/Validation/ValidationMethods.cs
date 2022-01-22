using BackendProductConfigurator.MediaProducers;
using Model;
using System.Linq;

namespace BackendProductConfigurator.Validation
{
    public static class ValidationMethods
    {
        public static EValidationResult ValidatePrice (ConfiguredProduct product, Rules dependencies)
        {
            float endPrice = dependencies.BasePrice;
            for(int i = 0; i < product.Options.Count; i++)
            {
                try
                {
                    endPrice += dependencies.PriceList[product.Options[i].Id];
                }
                catch { }
            }
            return (product.Price == endPrice) ? EValidationResult.ValidationPassed : EValidationResult.PriceInvalid;
        }
        public static EValidationResult ValidateConfiguration (ConfiguredProduct product, Configurator configurator)
        {
            EValidationResult validationResult = EValidationResult.ValidationPassed;
            List<string> allowedOptions = new List<string>();
            foreach (var group in configurator.OptionGroups)
            {
                if (group.Required)
                {
                    bool valid = false;
                    foreach(string optionGroupId in configurator.Rules.GroupRequirements[group.Id])
                    {
                        if (configurator.OptionGroups.Where(x => x.Id == optionGroupId).First().OptionIds.Select(x => x).Intersect(product.Options.Select(x => x.Id)).Any())
                            valid = true;
                    }
                    if(valid)
                    {
                        try
                        {
                            validationResult = product.Options.Select(productOption => productOption.Id).Intersect(group.OptionIds).Any() == false ? EValidationResult.ConfigurationInvalid : EValidationResult.ValidationPassed;
                        }
                        catch
                        {

                        }
                        if (validationResult == EValidationResult.ConfigurationInvalid)
                        {
                            break;
                        }
                    }
                }
            }

            return validationResult;
        }
        public static EValidationResult ValidateConfigurator(Configurator configurator)
        {
            if(configurator.Rules.Models.Select(x => x.Name).Count() != configurator.Rules.Models.Select(x => x.Name).Distinct().Count())
                return EValidationResult.ConfiguratorInvalid;

            return EValidationResult.ValidationPassed;
        }
    }
}
