using BackendProductConfigurator.MediaProducers;
using Model;
using System.Linq;

namespace BackendProductConfigurator.Validation
{
    public static class ValidationMethods
    {
        public static EValidationResult ValidatePrice (ConfiguredProduct product, RulesExtended dependencies)
        {
            float endPrice = dependencies.BasePrice;
            for(int i = 0; i < product.Options.Count; i++)
            {
                try
                {
                    endPrice += dependencies.PriceList[product.Options[i]];
                }
                catch { }
            }
            return (product.Price == endPrice) ? EValidationResult.ValidationPassed : EValidationResult.PriceInvalid;
        }
        public static EValidationResult ValidateConfiguration (ConfiguredProduct product, Configurator configurator)
        {
            EValidationResult validationResult = EValidationResult.ValidationPassed;
            List<string> allowedOptions = new List<string>();
            bool requiredValid;
            foreach (var group in configurator.OptionGroups)
            {
                if (group.Required)
                {
                    requiredValid = false;
                    try
                    {
                        foreach (string optionGroupId in configurator.Rules.GroupRequirements[group.Id])
                        {
                            if (configurator.OptionGroups.Where(x => x.Id == optionGroupId).First().OptionIds.Select(x => x).Intersect(product.Options).Any())
                                requiredValid = true;
                        }
                    }
                    catch { requiredValid = true; }
                    if(requiredValid)
                    {
                        try
                        {
                            validationResult = product.Options.Select(productOption => productOption).Intersect(group.OptionIds).Any() == false ? EValidationResult.ConfigurationInvalid : EValidationResult.ValidationPassed;
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
        public static EValidationResult ValidateSelectedModel(ConfiguredProduct configuredProduct, Configurator configurator)
        {
            try
            {
                List<string> modelList = configurator.Rules.Models.Where(x => x.Name == configuredProduct.Model).Select(x => x.Options).First();

                if (modelList.Intersect(configuredProduct.Options).Count() >= modelList.Count())
                    return EValidationResult.ModelSelectionInvalid;
            }
            catch
            { }

            return EValidationResult.ValidationPassed;
        }
    }
}
