using Model;

namespace BackendProductConfigurator.Validation
{
    public class ValidationMethods
    {
        public bool ValidatePrice (Product product, Dictionary<string, float> priceList)
        {
            float endPrice = 0;
            for(int i = 0; i < product.Options.Count; i++)
            {
                endPrice += priceList[product.Options[i].Id];
            }
            return product.Price == endPrice;
        }
    }
}
