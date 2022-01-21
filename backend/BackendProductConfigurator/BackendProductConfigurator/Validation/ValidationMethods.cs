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
        public static EValidationResult ValidateConfiguration (ConfiguredProduct product, Configurator configurator, List<OptionGroup> optionsGroups, Rules dependencies)
        {
            EValidationResult validationResult = EValidationResult.ValidationPassed;
            List<string> allowedOptions = new List<string>();
            foreach (var group in configurator.OptionGroups)
            {
                foreach (var item in dependencies.GroupRequirements)
                {
                    if ()
                        allowedOptions = (List<string>)allowedOptions.Concat(item.Value);
                }
                foreach (Option option in product.Options)
                {
                }
                if (group.Required)
                {
                    bool valid = false;
                    foreach(string optionGroupId in dependencies.GroupRequirements[group.Id])
                    {
                        if (group.OptionIds.Select(x => x).Intersect(product.Options).Any())
                            valid = true; //hier valid setzen = required ungültig machen
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
    }
}
