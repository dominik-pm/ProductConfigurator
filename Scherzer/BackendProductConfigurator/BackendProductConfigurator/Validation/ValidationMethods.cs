using BackendProductConfigurator.MediaProducers;
using Model;

namespace BackendProductConfigurator.Validation
{
    public static class ValidationMethods
    {
        public static EValidationResult ValidatePrice (ConfiguredProduct product, ProductDependencies dependencies)
        {
            float endPrice = dependencies.BasePrice;
            for(int i = 0; i < product.Options.Count; i++)
            {
                endPrice += dependencies.PriceList[product.Options[i].ConfigId];
            }
            return (product.Price == endPrice) ? EValidationResult.ValidationPassed : EValidationResult.PriceInvalid;
        }
        public static EValidationResult ValidateConfiguration (ConfiguredProduct product, List<OptionGroup> optionsGroups)
        {
            EValidationResult validationResult = EValidationResult.ValidationPassed;
            foreach (var group in optionsGroups)
            {
                if(group.Required == true)
                {
                    validationResult = product.Options.Select(productOption => productOption.ConfigId).Intersect(group.OptionIds).Any() == false ? EValidationResult.ConfigurationInvalid : EValidationResult.ValidationPassed;
                    if (validationResult == EValidationResult.ConfigurationInvalid)
                    {
                        break;
                    }
                }
            }

            return validationResult;
        }
    }
}
